using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seleccionados : MonoBehaviour
{
    [SerializeField] GameObject objetoDeTabla;
    [SerializeField] Image imagenDescripcionUI;
    private int numeroMaximoObjetos = 0;


    public List<Objeto> listaSeleccionados = new List<Objeto>();


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
        if (listaSeleccionados.Contains(boton.objetoOriginal))
            listaSeleccionados.Remove(boton.objetoOriginal);

        Destroy(boton.gameObject);

        numeroMaximoObjetos--;
        if (numeroMaximoObjetos < 0) numeroMaximoObjetos = 0;
    }
    // AÑADIR: Método para mostrar descripción
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

    // AÑADIR: Método auxiliar para encontrar la plantilla
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

}
