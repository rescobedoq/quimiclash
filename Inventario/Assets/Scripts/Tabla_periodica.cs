using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tabla_periodica : MonoBehaviour
{
    [SerializeField] GameObject prefabObjectTabla;
    [SerializeField] int numerMaximoElementos ;
    [SerializeField] Plantilla_Objeto[] listaElementos;

    private Objeto objeto;

    private void Start()
    {
        Transform padre = GameObject.FindGameObjectWithTag("Tabla_periodica").transform;

        for (int i = 0; i < numerMaximoElementos-1; i++)
        {
            GameObject tabla = Instantiate(prefabObjectTabla, Vector2.zero, Quaternion.identity, padre);

            objeto = tabla.GetComponent<Objeto>();

            objeto.CrearObjeto(listaElementos[i]);
        }
    }
}
