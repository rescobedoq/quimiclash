using UnityEngine;
using UnityEngine.UI;

public class DescripcionSeleccionada : MonoBehaviour
{
    // CAMBIO CLAVE: Usamos Plantilla_Objeto ya que es lo que se transfiere y persiste.
    public Plantilla_Objeto plantillaAsociada; 

    private Seleccionados seleccionados;
    private MostradorCantidad mostradorCantidad;

    private void Awake()
    {
        // Buscar referencias dinámicamente
        seleccionados = FindFirstObjectByType<Seleccionados>();
        mostradorCantidad = FindFirstObjectByType<MostradorCantidad>();

        if (seleccionados == null) 
            Debug.LogError("[DescripcionSeleccionada] No se encontró la instancia de Seleccionados.");
        if (mostradorCantidad == null) 
            Debug.LogError("[DescripcionSeleccionada] No se encontró la instancia de MostradorCantidad.");
    }

    public void MostrarDescripcion()
    {
        // 1. Mostrar la descripción (Imagen/Texto) usando la Plantilla_Objeto
        // Este método en Seleccionados debe ser actualizado para aceptar Plantilla_Objeto
        // o debe buscarse la plantilla en Seleccionados.
        // **OPCIÓN MÁS SEGURA:**
        
        if (seleccionados != null && plantillaAsociada != null)
        {
             // Llamamos al método de Seleccionados que usa la Plantilla_Objeto
             // Necesitas añadir este método a Seleccionados.cs o adaptar MostrarSeleccionado.
             seleccionados.MostrarSeleccionadoDesdePlantilla(plantillaAsociada);
        }
        else
        {
            Debug.LogWarning("[DescripcionSeleccionada] No se puede mostrar la descripción: plantillaAsociada es null.");
        }

        // 2. Actualizar la cantidad del objeto mostrado
        if (mostradorCantidad != null && plantillaAsociada != null)
        {
            mostradorCantidad.MostrarCantidadDePlantilla(plantillaAsociada);
            Debug.Log($"[DescripcionSeleccionada] Cantidad solicitada para: {plantillaAsociada.name}");
        }
    }

    // El método ObtenerSpriteDescripcion ya no es necesario aquí. 
    // Ahora, Seleccionados se encarga de obtener la imagen usando la Plantilla_Objeto directamente
    // (el campo 'imagenDescripcion' del ScriptableObject Plantilla_Objeto).
}