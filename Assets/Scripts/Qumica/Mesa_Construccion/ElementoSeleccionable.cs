using UnityEngine;
using UnityEngine.UI;

public class ElementoSeleccionable : MonoBehaviour
{
    private Plantilla_Objeto plantilla;
    private MostrarSeleccionados controlador;
    private bool estaSeleccionado = false;

    public void Inicializar(Plantilla_Objeto plantillaObj, MostrarSeleccionados controladorRef)
    {
        plantilla = plantillaObj;
        controlador = controladorRef;

        // Configurar el botón
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClic);
        }
    }

    void OnClic()
    {
        if (plantilla != null && controlador != null)
        {
            controlador.OnElementoSeleccionado(plantilla);
            estaSeleccionado = true;
            Debug.Log($"Seleccionado: {plantilla.nombreObjeto}");
        }
    }

    public string GetNombreObjeto()
    {
        return plantilla != null ? plantilla.nombreObjeto : "";
    }

    public bool EstaSeleccionado()
    {
        return estaSeleccionado;
    }

    public void Deseleccionar()
    {
        estaSeleccionado = false;
        // Restaurar color normal
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.color = Color.white;
        }
    }
}