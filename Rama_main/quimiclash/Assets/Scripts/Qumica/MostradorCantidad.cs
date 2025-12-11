using UnityEngine;
using TMPro;

public class MostradorCantidad : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoCantidad;
    private Seleccionados seleccionados;

    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();

        if (textoCantidad == null)
        {
            textoCantidad = GetComponent<TextMeshProUGUI>();
            if (textoCantidad == null)
            {
                textoCantidad = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    private void Start()
    {
        // Inicializar con valor por defecto
        ActualizarCantidad(null);
    }

    // Método principal para actualizar la cantidad
    public void ActualizarCantidad(Plantilla_Objeto plantilla)
    {
        if (textoCantidad == null)
        {
            Debug.LogWarning("TextMeshProUGUI no asignado en MostradorCantidad");
            return;
        }

        if (plantilla != null)
        {
            textoCantidad.text = plantilla.cantidad.ToString();
        }
        else
        {
            textoCantidad.text = "0";
        }
    }

    // Método para ser llamado desde otros scripts
    public void MostrarCantidadDePlantilla(Plantilla_Objeto plantilla)
    {
        ActualizarCantidad(plantilla);
    }

    // Método para actualizar basado en un objeto
    public void MostrarCantidadDeObjeto(Objeto objeto)
    {
        if (objeto != null && objeto.plantillaOrigen != null)
        {
            ActualizarCantidad(objeto.plantillaOrigen);
        }
        else
        {
            ActualizarCantidad(null);
        }
    }
}