using UnityEngine;
using UnityEngine.UI;

public class MostrarSeleccionados : MonoBehaviour
{
    [SerializeField] GameObject objetoDeTabla;

    void Start()
    {
        Debug.Log("MOSTRAR: Start ejecutado");
        MostrarListaPersistida();
    }

    void MostrarListaPersistida()
    {
        if (PersistenteSeleccionados.Instance == null)
        {
            Debug.LogError("No existe PersistenteSeleccionados en la escena.");
            return;
        }

        var lista = PersistenteSeleccionados.Instance.listaPersistida;

        if (lista == null || lista.Count == 0)
        {
            Debug.Log("Lista persistida vacía.");
            return;
        }

        Debug.Log("MOSTRAR: Lista persistida tiene: " + lista.Count);

        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;

        // Limpiar UI previa
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        // Recrear UI basado en PLANTILLAS
        foreach (Plantilla_Objeto plantilla in lista)
        {
            if (plantilla == null) continue;

            // Crear elemento UI
            GameObject elemento = Instantiate(
                objetoDeTabla,
                Vector2.zero,
                Quaternion.identity,
                parent
            );

            // Colocar imagen del ScriptableObject
            Image imagenUI = elemento.GetComponent<Image>();
            if (imagenUI != null)
            {
                imagenUI.sprite = plantilla.imagenObjeto;
                imagenUI.color = Color.white;
            }
        }
    }
}
