using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necesario para GetActiveScene
using System; // Necesario para Action (el evento OnElementosLoaded)

public class Seleccionados : MonoBehaviour
{
    // EVENTO PÚBLICO: Para notificar a otros scripts (como MostradorCantidad) cuando la lista está lista.
    public static Action OnElementosLoaded; 

    [SerializeField] GameObject objetoDeTabla;
    [SerializeField] Image imagenDescripcionUI;
    private int numeroMaximoObjetos = 0;

    public List<Objeto> listaSeleccionados = new List<Objeto>();
    public List<Plantilla_Objeto> listaSeleccionadosPlantillas = new List<Plantilla_Objeto>();
    
    void Start()
    {
        // ==========================================================
        // LÓGICA DE RECUPERACIÓN DE DATOS PERSISTENTES/TRANSICIÓN
        // ==========================================================

        // 1. Recuperar los datos guardados en el Singleton Persistente (si existe)
        if (PersistenteSeleccionados.Instance != null)
        {
            listaSeleccionadosPlantillas = new List<Plantilla_Objeto>(
                PersistenteSeleccionados.Instance.listaPersistida
            );
            numeroMaximoObjetos = PersistenteSeleccionados.Instance.numeroMaximoObjetosPersistido;
            Debug.Log("[SELECCIONADOS - START] Lista persistida recuperada: " + listaSeleccionadosPlantillas.Count);
        }

        // 2. Sobrescribir/Usar la lista transferida si venimos del GameManager (escena de química)
        if (GameManager.instance != null && SceneManager.GetActiveScene().name == GameManager.instance.chemistrySceneName)
        {
            if (GameManager.InventarioQuimicoTransferido.Count > 0)
            {
                // Limpiar la lista de la escena y usar la lista transferida
                listaSeleccionadosPlantillas.Clear();
                listaSeleccionados.Clear(); 
                
                listaSeleccionadosPlantillas.AddRange(GameManager.InventarioQuimicoTransferido);
                Debug.Log($"[SELECCIONADOS - START] Lista cargada desde GameManager. Elementos: {listaSeleccionadosPlantillas.Count}");
            }
        }
        
        // 3. Reconstruir la UI siempre después de inicializar la lista
        ReconstruirUIDesdePlantillas();
        
        // 4. Disparar el evento de carga para que otros componentes (MostradorCantidad) se actualicen
        if (OnElementosLoaded != null) 
        {
            OnElementosLoaded.Invoke();
            Debug.Log("[SELECCIONADOS - START] Evento OnElementosLoaded disparado.");
        }
    }

    // MÉTODO NUEVO: Guardar antes de cambiar escena
    public void GuardarAntesDeCambioEscena()
    {
        Debug.Log("[SELECCIONADOS - GUARDAR] Guardando antes de cambio de escena...");
        GuardarSeleccionadosEnPersistente();

        if (PersistenteSeleccionados.Instance != null)
        {
            Debug.Log("[SELECCIONADOS - GUARDAR] Elementos guardados en persistente: " +
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
    
    // NOTA: Este método (ReconstruirUI) se mantiene por compatibilidad, 
    // pero ReconstruirUIDesdePlantillas es el método preferido.
    void ReconstruirUI()
    {
        // Limpiar la UI actual
        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;
        if (parent == null) return;
        
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        // Recrear los objetos en la UI basado en la lista
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

        if (numeroMaximoObjetos < 9)
        {
            numeroMaximoObjetos++;
            listaSeleccionados.Add(obj);

            // Guardar también en listaSeleccionadosPlantillas
            if (obj.plantillaOrigen != null)
            {
                listaSeleccionadosPlantillas.Add(obj.plantillaOrigen);
            }

            GuardarSeleccionadosEnPersistente();

            // Instanciar la UI
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
                // Si BotonSeleccionado usa plantillaAsociada:
                // boton.plantillaAsociada = obj.plantillaOrigen; 
            }
            
            DescripcionSeleccionada descComp = elemento.GetComponent<DescripcionSeleccionada>();
            if (descComp != null)
            {
                descComp.plantillaAsociada = obj.plantillaOrigen; 
            }
        }
    }

    public void QuitarSeleccionado(BotonSeleccionado boton)
    {
        Debug.Log("[SELECCIONADOS] Quitanda seleccionado...");

        if (boton.objetoOriginal != null)
        {
            // Remover de listaSeleccionados
            if (listaSeleccionados.Contains(boton.objetoOriginal))
            {
                listaSeleccionados.Remove(boton.objetoOriginal);
                Debug.Log("[SELECCIONADOS] Removido de listaSeleccionados");
            }

            // Remover de listaSeleccionadosPlantillas
            if (boton.objetoOriginal.plantillaOrigen != null)
            {
                // Buscar y remover la primera ocurrencia de esta plantilla
                int index = listaSeleccionadosPlantillas.IndexOf(boton.objetoOriginal.plantillaOrigen);
                if (index >= 0)
                {
                    listaSeleccionadosPlantillas.RemoveAt(index);
                    Debug.Log("[SELECCIONADOS] Removido de listaSeleccionadosPlantillas");
                }
            }
        }

        Destroy(boton.gameObject);
        numeroMaximoObjetos--;
        if (numeroMaximoObjetos < 0) numeroMaximoObjetos = 0;

        // IMPORTANTE: Actualizar la lista persistente después de remover
        GuardarSeleccionadosEnPersistente();

        Debug.Log("[SELECCIONADOS] Después de quitar - listaSeleccionados: " + listaSeleccionados.Count +
                  ", listaSeleccionadosPlantillas: " + listaSeleccionadosPlantillas.Count);
    }

    // MÉTODO ORIGINAL: Usa el componente DescripcionSeleccionada (asumiendo que contiene Objeto)
    public void MostrarSeleccionado(DescripcionSeleccionada descripcion)
{
    // AHORA USAREMOS LA NUEVA VARIABLE: descripcion.plantillaAsociada
    Plantilla_Objeto plantilla = descripcion.plantillaAsociada; // ¡Cambio clave!

    if (plantilla != null && imagenDescripcionUI != null)
    {
        if (plantilla.imagenDescripcion != null)
        {
            Debug.Log("3. Plantilla encontrada: " + plantilla.name);
            imagenDescripcionUI.sprite = plantilla.imagenDescripcion;
            imagenDescripcionUI.color = Color.white;

            MostradorCantidad mostrador = FindFirstObjectByType<MostradorCantidad>();
            if (mostrador != null)
            {
                mostrador.MostrarCantidadDePlantilla(plantilla);
                Debug.Log("4. Cantidad actualizada.");
            }
        }
        else
        {
             Debug.Log("3. ERROR: Plantilla no tiene imagenDescripcion asignada.");
        }
    }
    else
    {
        Debug.Log("2. ERROR: PlantillaAsociada o imagenDescripcionUI es null.");
    }
}
    // MÉTODO NUEVO (Recomendado): Usa la Plantilla_Objeto directamente
    public void MostrarSeleccionadoDesdePlantilla(Plantilla_Objeto plantilla)
    {
        if (plantilla != null && imagenDescripcionUI != null)
        {
            if (plantilla.imagenDescripcion != null)
            {
                imagenDescripcionUI.sprite = plantilla.imagenDescripcion;
                imagenDescripcionUI.color = Color.white;
                Debug.Log("[SELECCIONADOS - DISPLAY] Descripción actualizada con plantilla: " + plantilla.name);

                MostradorCantidad mostrador = FindFirstObjectByType<MostradorCantidad>();
                if (mostrador != null)
                {
                    mostrador.MostrarCantidadDePlantilla(plantilla);
                }
            }
            else
            {
                Debug.LogWarning("[SELECCIONADOS - DISPLAY] Plantilla no tiene imagenDescripcion asignada: " + plantilla.name);
            }
        }
    }

    // Método para encontrar la plantilla a partir de un Objeto (flujo antiguo)
    private Plantilla_Objeto FindPlantillaByObjeto(Objeto obj)
    {
        // Si el objeto tiene una plantilla de origen, úsala
        if (obj.plantillaOrigen != null) return obj.plantillaOrigen;
        
        // Si no, recurre a buscar todos los ScriptableObjects
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

        Debug.Log("[SELECCIONADOS - PERSIST] Lista de seleccionados guardada en persistente. Elementos: " +
                  PersistenteSeleccionados.Instance.listaPersistida.Count);
    }

    public void ReconstruirUIDesdePlantillas()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;
        if (parent == null)
        {
             Debug.LogError("[SELECCIONADOS - UI] No se encontró el padre con el tag 'Elementos_select'.");
             return;
        }

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        Debug.Log($"[SELECCIONADOS - UI] Reconstruyendo UI con {listaSeleccionadosPlantillas.Count} elementos.");

        // Aquí se reconstruyen los objetos visuales (botones) en la tabla
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
            
            // **IMPORTANTE**: Debes asegurarte de que BotonSeleccionado y DescripcionSeleccionada
            // tienen una variable `public Plantilla_Objeto plantillaAsociada;` 
            // y asignarla aquí:
            
            BotonSeleccionado boton = elemento.GetComponent<BotonSeleccionado>();
            if (boton != null)
            {
                 // Si BotonSeleccionado tiene public Plantilla_Objeto plantillaAsociada;
                 // boton.plantillaAsociada = plantilla; 
                 
                 // Para evitar errores si el campo ObjetoOriginal es requerido:
                 // boton.objetoOriginal = null; // o crear un objeto temporal si es necesario
            }
            
            DescripcionSeleccionada descComp = elemento.GetComponent<DescripcionSeleccionada>();
            if (descComp != null)
            {
                // Si DescripcionSeleccionada tiene public Plantilla_Objeto plantillaAsociada;
                // descComp.plantillaAsociada = plantilla;
                
                // Para evitar errores si el campo ObjetoOriginal es requerido:
                // descComp.objetoOriginal = null; 
            }
        }
        
        // --- Forzar actualización de cantidad después de reconstruir ---
        MostradorCantidad mostrador = FindFirstObjectByType<MostradorCantidad>();
        if (mostrador != null)
        {
            if (listaSeleccionadosPlantillas.Count > 0)
            {
                // Si hay elementos, muestra la cantidad del primer elemento en la lista
                mostrador.MostrarCantidadDePlantilla(listaSeleccionadosPlantillas[0]);
                Debug.Log("[SELECCIONADOS - UI] Llamando a MostradorCantidad para primer elemento.");
            }
            else
            {
                // Si no hay elementos, simplemente limpia el mostrador (muestra "0")
                mostrador.ActualizarCantidad(null);
            }
        }
    }
}