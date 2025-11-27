using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objeto : MonoBehaviour
{
    [SerializeField] Image imagenObjeto;
    // AÑADIR: Referencia a la imagen de descripción
    [SerializeField] Image imagenDescripcion;

    private Seleccionados seleccionados;

    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();
    }

    public void CrearObjeto(Plantilla_Objeto datosObjeto)
    {
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

        // AÑADIR: Configurar la imagen de descripción si existe
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
        DescripcionSeleccionada managerDescripcion = FindFirstObjectByType<DescripcionSeleccionada>();
        if (managerDescripcion != null)
        {
            managerDescripcion.objetoOriginal = this;
            managerDescripcion.MostrarDescripcion();
        }
    }
    // AÑADIR: Método para obtener la descripción
    public Sprite GetDescripcionSprite()
    {
        return imagenDescripcion != null ? imagenDescripcion.sprite : null;
    }
}