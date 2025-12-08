using UnityEngine;
using TMPro;

public class MostradorCantidad : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoCantidad;
    private Seleccionados seleccionados;
    
    // Referencia al Singleton de Inventario
    private Inventory inventario; 
    private Plantilla_Objeto plantillaActual; // Guardamos la plantilla que se está mostrando

    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();
        inventario = Inventory.instance; 
        
        Debug.Log($"[MOSTRADOR - AWAKE] Inventario asignado: {(inventario != null ? "SÍ" : "NO")}");

        if (textoCantidad == null)
        {
            textoCantidad = GetComponent<TextMeshProUGUI>();
            if (textoCantidad == null)
            {
                textoCantidad = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
    
    private void OnEnable()
    {
        // Suscribir el método de actualización forzada al evento de Seleccionados
        Seleccionados.OnElementosLoaded += MostrarPrimerElemento; 
    }

    private void OnDisable()
    {
        // Desuscribir para evitar errores al destruir objetos
        Seleccionados.OnElementosLoaded -= MostrarPrimerElemento;
    }

    // Método que se llama cuando Seleccionados termina de cargar la escena
    private void MostrarPrimerElemento()
    {
        if (seleccionados != null && seleccionados.listaSeleccionadosPlantillas.Count > 0)
        {
            // Usamos el primer elemento para inicializar el contador
            MostrarCantidadDePlantilla(seleccionados.listaSeleccionadosPlantillas[0]);
            Debug.Log("[MOSTRADOR - EVENTO] Actualización forzada por evento OnElementosLoaded.");
        }
        else
        {
             ActualizarCantidad(null);
        }
    }

    // --- Consulta la cantidad en el Inventario ---
    private int ObtenerCantidadDeElemento(Plantilla_Objeto plantilla)
    {
        // Intentamos reobtener la instancia por si no estaba lista en Awake
        if (inventario == null)
        {
            inventario = Inventory.instance;
        }

        if (inventario == null)
        {
            Debug.LogError("[MOSTRADOR - OBTENER] Error: La instancia de Inventory no está disponible para la consulta.");
            return 0;
        }

        // Recorremos los slots del inventario buscando la Plantilla_Objeto
        foreach (InventorySlot slot in inventario.items)
        {
            if (slot.elementoQuimico == plantilla)
            {
                Debug.Log($"[MOSTRADOR - OBTENER] Cantidad de '{plantilla.name}' en inventario: {slot.amount}");
                return slot.amount; 
            }
        }
        
        Debug.Log($"[MOSTRADOR - OBTENER] Plantilla '{plantilla.name}' no encontrada en Inventory.items.");
        return 0;
    }

    // Método principal para actualizar el texto
    public void ActualizarCantidad(Plantilla_Objeto plantilla)
    {
        if (textoCantidad == null) return;

        if (plantilla != null)
        {
            int cantidadEnInventario = ObtenerCantidadDeElemento(plantilla); 
            
            if (cantidadEnInventario > 0)
            {
                textoCantidad.text = cantidadEnInventario.ToString();
            }
            else
            {
                textoCantidad.text = "0";
            }
        }
        else
        {
            textoCantidad.text = "0";
        }
    }

    // Método para ser llamado desde botones de la UI
    public void MostrarCantidadDePlantilla(Plantilla_Objeto plantilla)
    {
        this.plantillaActual = plantilla; // Guardar para futura referencia
        ActualizarCantidad(plantilla);
    }

    // Método para actualizar basado en un objeto (si aún lo usas)
    public void MostrarCantidadDeObjeto(Objeto objeto)
    {
        if (objeto != null && objeto.plantillaOrigen != null)
        {
            MostrarCantidadDePlantilla(objeto.plantillaOrigen);
        }
        else
        {
            ActualizarCantidad(null);
        }
    }
}