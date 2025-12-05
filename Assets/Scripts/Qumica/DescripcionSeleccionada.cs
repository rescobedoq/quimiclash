using UnityEngine;
using UnityEngine.UI;

public class DescripcionSeleccionada : MonoBehaviour
{
    public Objeto objetoOriginal;
    private Seleccionados seleccionados;

    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();
    }

    public void MostrarDescripcion()
    {
        seleccionados.MostrarSeleccionado(this);
    }

    // AÑADIR: Método para obtener la imagen de descripción del objeto
    public Sprite ObtenerSpriteDescripcion()
    {
        if (objetoOriginal != null)
        {
            // Buscar en los hijos del objeto la imagen de descripción
            Image[] imagenes = objetoOriginal.GetComponentsInChildren<Image>();
            foreach (Image img in imagenes)
            {
                if (img != objetoOriginal.GetComponent<Image>() && img.sprite != null)
                {
                    return img.sprite;
                }
            }
        }
        return null;
    }
}