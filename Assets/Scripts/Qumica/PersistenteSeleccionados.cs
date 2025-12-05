using System.Collections.Generic;
using UnityEngine;

public class PersistenteSeleccionados : MonoBehaviour
{
    public static PersistenteSeleccionados Instance;

    // ?? LA NUEVA LISTA PERSISTENTE
    public List<Plantilla_Objeto> listaPersistida = new List<Plantilla_Objeto>();


    public int numeroMaximoObjetosPersistido = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
