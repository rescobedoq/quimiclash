using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seleccionados : MonoBehaviour
{
    [SerializeField] GameObject objetoDeTabla;
    [SerializeField] Image imagenDescripcionUI;
    private int numeroMaximoObjetos = 0;

    public List<Objeto> listaSeleccionados = new List<Objeto>();
    public List<Plantilla_Objeto> listaSeleccionadosPlantillas = new List<Plantilla_Objeto>();

    void Start()
    {
        // Al iniciar en cualquier escena, recuperar las plantillas persistidas
        if (PersistenteSeleccionados.Instance != null)
        {
            listaSeleccionadosPlantillas = new List<Plantilla_Objeto>(
                PersistenteSeleccionados.Instance.listaPersistida
            );

            // También actualizar el contador
            numeroMaximoObjetos = PersistenteSeleccionados.Instance.numeroMaximoObjetosPersistido;

            // Reconstruir UI solo con plantillas
            ReconstruirUIDesdePlantillas();

            Debug.Log("Seleccionados Start - Lista persistida recuperada: " + listaSeleccionadosPlantillas.Count);
        }
    }

    // MÉTODO NUEVO: Guardar antes de cambiar escena
    public void GuardarAntesDeCambioEscena()
    {
        Debug.Log("Guardando antes de cambio de escena...");
        GuardarSeleccionadosEnPersistente();

        if (PersistenteSeleccionados.Instance != null)
        {
            Debug.Log("Elementos guardados en persistente: " +
                      PersistenteSeleccionados.Instance.listaPersistida.Count);
        }
    }

    void OnDestroy()
    {
        if (PersistenteSeleccionados.Instance != null)
        {
            PersistenteSeleccionados.Instance.listaPersistida =
                new List<Plantilla_Objeto>(listaSeleccionadosPlantillas);
        }
    }

    void ReconstruirUI()
    {
        // Limpiar la UI actual
        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        // Recrear los objetos en la UI basado en la lista persistida
        foreach (Objeto obj in listaSeleccionados)
        {
            if (obj != null)
            {
                GameObject elemento = Instantiate(
                    objetoDeTabla,
                    Vector2.zero,
                    Quaternion.identity,
                    parent
                );

                Image imagen = elemento.GetComponent<Image>();
                if (obj.GetComponent<Image>() != null)
                {
                    imagen.sprite = obj.GetComponent<Image>().sprite;
                }

                BotonSeleccionado boton = elemento.GetComponent<BotonSeleccionado>();
                if (boton != null)
                {
                    boton.objetoOriginal = obj;
                }

                DescripcionSeleccionada descComp = elemento.GetComponent<DescripcionSeleccionada>();
                if (descComp != null)
                {
                    descComp.objetoOriginal = obj;
                }
            }
        }
    }

    public void IncluirSeleccionados(GameObject objetoGO)
    {
        Objeto obj = objetoGO.GetComponent<Objeto>();

        if (obj == null)
        {
            Debug.Log("El objeto seleccionado no tiene script Objeto.");
            return;
        }

        Image imagenElemento = objetoGO.GetComponent<Image>();

        if (imagenElemento == null || imagenElemento.sprite == null)
        {
            Debug.Log("Imagen vacía, no se incluye.");
            return;
        }

        if (numeroMaximoObjetos < 8)
        {
            numeroMaximoObjetos++;
            listaSeleccionados.Add(obj);

            // AÑADIR: Guardar también en listaSeleccionadosPlantillas
            if (obj.plantillaOrigen != null)
            {
                listaSeleccionadosPlantillas.Add(obj.plantillaOrigen);
            }

            GuardarSeleccionadosEnPersistente();

            GameObject elemento = Instantiate(
                objetoDeTabla,
                Vector2.zero,
                Quaternion.identity,
                GameObject.FindGameObjectWithTag("Elementos_select").transform
            );

            Image imagen = elemento.GetComponent<Image>();
            imagen.sprite = imagenElemento.sprite;
            BotonSeleccionado boton = elemento.GetComponent<BotonSeleccionado>();
            if (boton != null)
            {
                boton.objetoOriginal = obj;
            }
            DescripcionSeleccionada descComp = elemento.GetComponent<DescripcionSeleccionada>();
            if (descComp != null)
            {
                descComp.objetoOriginal = obj;
            }
        }
    }

    public void QuitarSeleccionado(BotonSeleccionado boton)
    {
        Debug.Log("Quitando seleccionado...");

        if (boton.objetoOriginal != null)
        {
            // Remover de listaSeleccionados
            if (listaSeleccionados.Contains(boton.objetoOriginal))
            {
                listaSeleccionados.Remove(boton.objetoOriginal);
                Debug.Log("Removido de listaSeleccionados");
            }

            // Remover de listaSeleccionadosPlantillas
            if (boton.objetoOriginal.plantillaOrigen != null)
            {
                // Buscar y remover la primera ocurrencia de esta plantilla
                int index = listaSeleccionadosPlantillas.IndexOf(boton.objetoOriginal.plantillaOrigen);
                if (index >= 0)
                {
                    listaSeleccionadosPlantillas.RemoveAt(index);
                    Debug.Log("Removido de listaSeleccionadosPlantillas");
                }
            }
        }

        Destroy(boton.gameObject);
        numeroMaximoObjetos--;
        if (numeroMaximoObjetos < 0) numeroMaximoObjetos = 0;

        // IMPORTANTE: Actualizar la lista persistente después de remover
        GuardarSeleccionadosEnPersistente();

        Debug.Log("Después de quitar - listaSeleccionados: " + listaSeleccionados.Count +
                  ", listaSeleccionadosPlantillas: " + listaSeleccionadosPlantillas.Count);
    }

    public void MostrarSeleccionado(DescripcionSeleccionada descripcion)
    {
        Debug.Log("1. MostrarSeleccionado llamado");

        if (descripcion != null && descripcion.objetoOriginal != null && imagenDescripcionUI != null)
        {
            Debug.Log("2. Todos los componentes existen");

            // Buscar la plantilla del objeto para obtener la imagen de descripción
            Plantilla_Objeto plantilla = FindPlantillaByObjeto(descripcion.objetoOriginal);

            if (plantilla != null && plantilla.imagenDescripcion != null)
            {
                Debug.Log("3. Plantilla encontrada: " + plantilla.name);
                imagenDescripcionUI.sprite = plantilla.imagenDescripcion;
                imagenDescripcionUI.color = Color.white;
            }
            else
            {
                Debug.Log("3. ERROR: No se encontró plantilla o imagenDescripcion es null");
            }
        }
        else
        {
            Debug.Log("2. ERROR: Algo es null - descripcion: " + descripcion +
                     ", objetoOriginal: " + descripcion?.objetoOriginal +
                     ", imagenDescripcionUI: " + imagenDescripcionUI);
        }
    }

    private Plantilla_Objeto FindPlantillaByObjeto(Objeto obj)
    {
        // Busca directamente en todos los ScriptableObjects de Plantilla_Objeto
        Plantilla_Objeto[] todasPlantillas = Resources.FindObjectsOfTypeAll<Plantilla_Objeto>();

        foreach (Plantilla_Objeto plantilla in todasPlantillas)
        {
            if (plantilla.imagenObjeto == obj.GetComponent<Image>().sprite)
            {
                return plantilla;
            }
        }
        return null;
    }

    public void GuardarSeleccionadosEnPersistente()
    {
        if (PersistenteSeleccionados.Instance == null)
        {
            Debug.LogError("No hay instancia persistente para guardar");
            return;
        }

        // Limpiar y rellenar la lista persistente
        PersistenteSeleccionados.Instance.listaPersistida.Clear();

        // Guardar desde listaSeleccionadosPlantillas que es más confiable
        foreach (Plantilla_Objeto plantilla in listaSeleccionadosPlantillas)
        {
            if (plantilla != null)
            {
                PersistenteSeleccionados.Instance.listaPersistida.Add(plantilla);
            }
        }

        PersistenteSeleccionados.Instance.numeroMaximoObjetosPersistido = numeroMaximoObjetos;

        Debug.Log("Lista de seleccionados guardada en persistente. Elementos: " +
                  PersistenteSeleccionados.Instance.listaPersistida.Count);
    }

    void ReconstruirUIDesdePlantillas()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        foreach (Plantilla_Objeto plantilla in listaSeleccionadosPlantillas)
        {
            if (plantilla == null) continue;

            GameObject elemento = Instantiate(
                objetoDeTabla,
                Vector2.zero,
                Quaternion.identity,
                parent
            );

            Image imagen = elemento.GetComponent<Image>();
            if (imagen != null)
            {
                imagen.sprite = plantilla.imagenObjeto;
                imagen.color = Color.white;
            }
        }
    }
}