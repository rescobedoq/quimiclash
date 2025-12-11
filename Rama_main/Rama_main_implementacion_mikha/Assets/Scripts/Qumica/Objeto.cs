using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Objeto : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] Image imagenObjeto;
    // A�ADIR: Referencia a la imagen de descripci�n
    [SerializeField] Image imagenDescripcion;

    private Seleccionados seleccionados;
    public Plantilla_Objeto plantillaOrigen;




    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();
    }

    public void CrearObjeto(Plantilla_Objeto datosObjeto)
    {
        plantillaOrigen = datosObjeto;
        if (datosObjeto.imagenObjeto == null)
        {
            // Hacer transparente el Image
            imagenObjeto.color = Color.clear;
        }
        else
        {
            // Mostrar la imagen normalmente
            imagenObjeto.sprite = datosObjeto.imagenObjeto;
            imagenObjeto.color = Color.white;
        }

        // A�ADIR: Configurar la imagen de descripci�n si existe
        if (imagenDescripcion != null && datosObjeto.imagenDescripcion != null)
        {
            imagenDescripcion.sprite = datosObjeto.imagenDescripcion;
            imagenDescripcion.color = Color.white;
        }
        else if (imagenDescripcion != null)
        {
            imagenDescripcion.color = Color.clear;
        }
    }

    public void SeleccionarElementos()
    {
        seleccionados.IncluirSeleccionados(gameObject);
        MostrarDescripcion();
    }
  public void MostrarDescripcion()
{
    // Usamos FindFirstObjectByType<DescripcionSeleccionada>() para encontrar la referencia,
    // asumiendo que solo hay una en la escena.
    DescripcionSeleccionada managerDescripcion = FindFirstObjectByType<DescripcionSeleccionada>();
    
    // Asumimos que 'this' tiene una plantilla de origen (Plantilla_Objeto)
    Objeto objetoActual = this.GetComponent<Objeto>(); 

    if (managerDescripcion != null && objetoActual != null && objetoActual.plantillaOrigen != null)
    {
        // CORRECCIÓN CLAVE: Asignar la Plantilla_Objeto al campo actualizado
        managerDescripcion.plantillaAsociada = objetoActual.plantillaOrigen; 
        
        // Llamar al método del Description Manager
        managerDescripcion.MostrarDescripcion();
    }
    else
    {
        Debug.LogWarning("[Objeto/BotonInventario] No se pudo mostrar la descripción. Faltan referencias (DescripcionSeleccionada, Objeto, o plantillaOrigen).");
    }
}
    // A�ADIR: M�todo para obtener la descripci�n
    public void OnPointerEnter(PointerEventData eventData)
    {
        MostrarDescripcion();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Opcional: Ocultar descripci�n al salir
    }

    public Sprite GetDescripcionSprite()
    {
        return imagenDescripcion != null ? imagenDescripcion.sprite : null;
    }

}