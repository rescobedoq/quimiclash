using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Seleccionados : MonoBehaviour
{
    [SerializeField] GameObject objetoDeTabla;

    private int numeroMaximoObjetos = 0;

 

    public void IncluirSeleccionados(Image imagenElemento)
    {
        if ( numeroMaximoObjetos <= 8)
        {
            numeroMaximoObjetos++;
            GameObject elemento = GameObject.Instantiate(objetoDeTabla, Vector2.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("Elementos_select").transform);
            Image imagen = elemento.GetComponent<Image>();
            imagen.sprite = imagenElemento.sprite;
        }
    }
}