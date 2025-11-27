using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objeto : MonoBehaviour
{
    [SerializeField] Image imagenObjeto;

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
    }
     public void SeleccionarElementos()
    {
        seleccionados.IncluirSeleccionados(gameObject);

    }
}