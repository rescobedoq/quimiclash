using UnityEngine;
using UnityEngine.EventSystems;

public class BotonSeleccionado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Objeto objetoOriginal;
    private Seleccionados seleccionados;
    private DescripcionSeleccionada descripcionComponent;

    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();
        descripcionComponent = GetComponent<DescripcionSeleccionada>();
    }

    public void CuandoLoPresiono()
    {
        seleccionados.QuitarSeleccionado(this);
    }

    // AÑADE ESTOS DOS MÉTODOS:
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descripcionComponent != null && objetoOriginal != null)
        {
            descripcionComponent.objetoOriginal = objetoOriginal;
            descripcionComponent.MostrarDescripcion();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Opcional: Ocultar descripción al salir
    }
}