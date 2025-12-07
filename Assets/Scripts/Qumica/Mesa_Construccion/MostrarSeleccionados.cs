using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MostrarSeleccionados : MonoBehaviour
{
    [SerializeField] GameObject objetoDeTabla;
    [SerializeField] private GridSeleccionMoleculas gridController;

    private List<GameObject> elementosInstanciados = new List<GameObject>();
    private Plantilla_Objeto elementoSeleccionadoParaColocar;

    void Start()
    {
        Debug.Log("MOSTRAR: Start ejecutado");
        MostrarListaPersistida();

        if (gridController == null)
        {
            gridController = FindAnyObjectByType<GridSeleccionMoleculas>();
            if (gridController != null)
                Debug.Log("GridController encontrado automáticamente");
        }
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

        Debug.Log($"MOSTRAR: Lista persistida tiene: {lista.Count} elementos");

        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;

        LimpiarElementosUI();

        foreach (Plantilla_Objeto plantilla in lista)
        {
            if (plantilla == null)
            {
                Debug.LogWarning("Plantilla nula encontrada en lista persistida");
                continue;
            }

            CrearElementoUI(plantilla, parent);
        }

        Debug.Log($"Se crearon {elementosInstanciados.Count} elementos seleccionables");
    }

    void CrearElementoUI(Plantilla_Objeto plantilla, Transform parent)
    {
        GameObject elemento = Instantiate(
            objetoDeTabla,
            Vector2.zero,
            Quaternion.identity,
            parent
        );

        ConfigurarElementoSeleccionable(elemento, plantilla);
        elementosInstanciados.Add(elemento);
    }

    void ConfigurarElementoSeleccionable(GameObject elemento, Plantilla_Objeto plantilla)
    {
        Button btn = elemento.GetComponent<Button>();
        if (btn == null)
        {
            btn = elemento.AddComponent<Button>();
        }

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnElementoSeleccionado(plantilla));

        Image imagenUI = elemento.GetComponent<Image>();
        if (imagenUI != null)
        {
            imagenUI.sprite = plantilla.imagenObjeto;
            imagenUI.color = Color.white; // IMPORTANTE: Color normal
            elemento.name = $"Elem_{plantilla.nombreObjeto}_{System.Guid.NewGuid()}";
        }

        // Añadir texto con nombre
        Text texto = elemento.GetComponentInChildren<Text>();
        if (texto == null)
        {
            GameObject textoObj = new GameObject("Texto");
            textoObj.transform.SetParent(elemento.transform);
            textoObj.transform.localPosition = Vector3.zero;
            texto = textoObj.AddComponent<Text>();
            texto.text = plantilla.nombreObjeto;
            texto.fontSize = 10;
            texto.alignment = TextAnchor.MiddleCenter;
            texto.color = Color.black;
        }
        else
        {
            texto.text = plantilla.nombreObjeto;
        }

        Debug.Log($"Elemento listo: '{plantilla.nombreObjeto}'");
    }

    public void OnElementoSeleccionado(Plantilla_Objeto plantilla)
    {
        Debug.Log($"=== ELEMENTO SELECCIONADO PARA COLOCAR ===");
        Debug.Log($"Nombre: '{plantilla.nombreObjeto}'");

        elementoSeleccionadoParaColocar = plantilla;

        if (gridController != null)
        {
            gridController.PrepararParaColocarElemento(plantilla);
        }
        else
        {
            gridController = FindAnyObjectByType<GridSeleccionMoleculas>();
            if (gridController != null)
            {
                gridController.PrepararParaColocarElemento(plantilla);
            }
            else
            {
                Debug.LogError("No se encontró GridSeleccionMoleculas en la escena");
            }
        }
    }

    // Se llama cuando un elemento se coloca en el grid
    public void ElementoColocadoEnGrid(Plantilla_Objeto elementoColocado, Vector2Int coordenadaCelda)
    {
        Debug.Log($"=== ELEMENTO COLOCADO EN GRID ===");
        Debug.Log($"Elemento: '{elementoColocado.nombreObjeto}'");
        Debug.Log($"Celda: ({coordenadaCelda.x}, {coordenadaCelda.y})");

        // 1. Remover el elemento de la lista persistida
        if (PersistenteSeleccionados.Instance != null)
        {
            bool removido = PersistenteSeleccionados.Instance.listaPersistida.Remove(elementoColocado);
            Debug.Log($"Removido de lista persistida: {removido}");
        }

        // 2. Buscar y destruir el GameObject UI correspondiente
        RemoverElementoDeUI(elementoColocado);

        // 3. Resetear selección
        elementoSeleccionadoParaColocar = null;

        Debug.Log($"✓ Elemento '{elementoColocado.nombreObjeto}' eliminado de la lista");
    }

    // NUEVO MÉTODO: Se llama cuando un elemento se devuelve desde el grid
    public void ElementoDevueltoALista(Plantilla_Objeto elementoDevuelto)
    {
        Debug.Log($"=== ELEMENTO DEVUELTO A LA LISTA ===");
        Debug.Log($"Elemento: '{elementoDevuelto.nombreObjeto}'");

        // 1. Añadir el elemento a la lista persistida
        if (PersistenteSeleccionados.Instance != null)
        {
            if (!PersistenteSeleccionados.Instance.listaPersistida.Contains(elementoDevuelto))
            {
                PersistenteSeleccionados.Instance.listaPersistida.Add(elementoDevuelto);
                Debug.Log($"Añadido a lista persistida");
            }
        }

        // 2. Crear un nuevo elemento UI en la lista
        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;
        CrearElementoUI(elementoDevuelto, parent);

        Debug.Log($"✓ Elemento '{elementoDevuelto.nombreObjeto}' devuelto a la lista");
    }

    void RemoverElementoDeUI(Plantilla_Objeto elemento)
    {
        GameObject elementoAEliminar = null;

        foreach (GameObject elemUI in elementosInstanciados)
        {
            if (elemUI != null)
            {
                Text texto = elemUI.GetComponentInChildren<Text>();
                if (texto != null && texto.text == elemento.nombreObjeto)
                {
                    elementoAEliminar = elemUI;
                    break;
                }
            }
        }

        if (elementoAEliminar != null)
        {
            elementosInstanciados.Remove(elementoAEliminar);
            Destroy(elementoAEliminar);
            Debug.Log($"Elemento UI '{elemento.nombreObjeto}' eliminado");
        }
        else
        {
            Debug.LogWarning($"No se encontró el elemento UI para '{elemento.nombreObjeto}'");
        }
    }

    void LimpiarElementosUI()
    {
        foreach (GameObject elemento in elementosInstanciados)
        {
            if (elemento != null)
                Destroy(elemento);
        }
        elementosInstanciados.Clear();

        Transform parent = GameObject.FindGameObjectWithTag("Elementos_select").transform;
        foreach (Transform child in parent)
        {
            if (child != null && child.gameObject != null)
                Destroy(child.gameObject);
        }
    }

    [ContextMenu("Actualizar Lista")]
    public void ActualizarLista()
    {
        MostrarListaPersistida();
    }
}