using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seleccionados : MonoBehaviour
{
    [SerializeField] GameObject objetoDeTabla;

    private int numeroMaximoObjetos = 0;


    public List<Objeto> listaSeleccionados = new List<Objeto>();


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

        if (numeroMaximoObjetos < 8)
        {
            numeroMaximoObjetos++;


            listaSeleccionados.Add(obj);

     
            GameObject elemento = Instantiate(
                objetoDeTabla,
                Vector2.zero,
                Quaternion.identity,
                GameObject.FindGameObjectWithTag("Elementos_select").transform
            );

            Image imagen = elemento.GetComponent<Image>();
            imagen.sprite = imagenElemento.sprite;
        }
    }


    public void QuitarSeleccionado(BotonSeleccionado boton)
    {
        if (listaSeleccionados.Contains(boton.objetoOriginal))
            listaSeleccionados.Remove(boton.objetoOriginal);

        Destroy(boton.gameObject);

        numeroMaximoObjetos--;
        if (numeroMaximoObjetos < 0) numeroMaximoObjetos = 0;
    }

}
