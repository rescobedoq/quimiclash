using UnityEngine;

public class CreadorPersistencia : MonoBehaviour
{
    private void Awake()
    {
        if (PersistenteSeleccionados.Instance == null)
        {
            // Crear un GameObject nuevo
            GameObject go = new GameObject("PersistenteSeleccionados");

            // Agregar el script de persistencia
            go.AddComponent<PersistenteSeleccionados>();

            // Evitar que se destruya al cambiar escena
            DontDestroyOnLoad(go);

            Debug.Log(">> [CreadorPersistencia] Se creó el objeto PersistenteSeleccionados automáticamente.");
        }
        else
        {
            Debug.Log(">> [CreadorPersistencia] Ya existe el objeto persistente.");
        }
    }
}
