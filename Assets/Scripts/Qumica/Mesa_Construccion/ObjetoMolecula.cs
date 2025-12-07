using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjetoMolecula : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoNombre; // Asignar manualmente en el prefab
    [SerializeField] private GridSeleccionMoleculas gridController;

    public string nombreMolecula;
    private PlantillaObjetoMolecula datosMolecula;

    // Variable para rastrear si ya configuramos el botón
    private bool botonConfigurado = false;

    void Start()
    {
        // Configurar automáticamente al inicio
        if (!botonConfigurado)
        {
            ConfigurarBoton();
        }

        Debug.Log($"ObjetoMolecula '{gameObject.name}' inicializado");
    }

    public void CrearObjeto(PlantillaObjetoMolecula datos, GridSeleccionMoleculas controller = null)
    {
        if (datos == null)
        {
            Debug.LogError($"Intento de crear objeto con datos nulos en {gameObject.name}");
            return;
        }

        datosMolecula = datos;
        nombreMolecula = datos.nombreMolecula;

        // BUSCAR EL TEXTO SI NO ESTÁ ASIGNADO
        if (textoNombre == null)
        {
            BuscarTextoAutomáticamente();
        }

        // ASIGNAR EL NOMBRE AL TEXTO
        if (textoNombre != null)
        {
            textoNombre.text = nombreMolecula;
            Debug.Log($"Texto asignado a '{textoNombre.gameObject.name}': '{nombreMolecula}'");
        }
        else
        {
            Debug.LogError($"No se pudo encontrar TextMeshProUGUI en {gameObject.name}");

            // Intentar crear uno automáticamente (solución de emergencia)
            CrearTextoDeEmergencia();
        }

        if (controller != null)
        {
            gridController = controller;
            Debug.Log($"GridController asignado para '{nombreMolecula}'");
        }

        if (!botonConfigurado)
        {
            ConfigurarBoton();
        }

        Debug.Log($"✓ Molécula '{nombreMolecula}' lista para usar");
    }

    void BuscarTextoAutomáticamente()
    {
        // Buscar TextMeshProUGUI en este objeto o en sus hijos
        textoNombre = GetComponentInChildren<TextMeshProUGUI>(true);

        if (textoNombre != null)
        {
            Debug.Log($"Texto encontrado automáticamente en {textoNombre.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"No se encontró TextMeshProUGUI en {gameObject.name} o sus hijos");

            // Buscar más profundamente
            TextMeshProUGUI[] todosTextos = GetComponentsInChildren<TextMeshProUGUI>(true);
            if (todosTextos.Length > 0)
            {
                textoNombre = todosTextos[0];
                Debug.Log($"Texto encontrado (profundo): {textoNombre.gameObject.name}");
            }
        }
    }

    void CrearTextoDeEmergencia()
    {
        // Crear un objeto de texto como último recurso
        GameObject textoObj = new GameObject("TextoNombre_Emergencia");
        textoObj.transform.SetParent(this.transform);
        textoObj.transform.localPosition = Vector3.zero;

        textoNombre = textoObj.AddComponent<TextMeshProUGUI>();
        textoNombre.text = nombreMolecula;
        textoNombre.fontSize = 14;
        textoNombre.alignment = TextAlignmentOptions.Center;
        textoNombre.color = Color.black;

        Debug.LogWarning($"Texto de emergencia creado para {nombreMolecula}");
    }

    void ConfigurarBoton()
    {
        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            // Limpiar cualquier listener previo
            boton.onClick.RemoveAllListeners();

            // Añadir nuestro listener DESDE EL CÓDIGO
            boton.onClick.AddListener(OnMoleculaClickeada);

            botonConfigurado = true;
            Debug.Log($"Botón configurado para '{nombreMolecula}' - Listeners: {boton.onClick.GetPersistentEventCount()}");
        }
        else
        {
            Debug.LogError($"No se encontró componente Button en {gameObject.name}");
            // Intentar añadirlo automáticamente
            gameObject.AddComponent<Button>();
            ConfigurarBoton();
        }
    }

    // IMPORTANTE: Este método es PRIVADO, solo se usa desde el código
    void OnMoleculaClickeada()
    {
        if (datosMolecula == null)
        {
            Debug.LogWarning($"No hay datos de molécula asignados en {gameObject.name}");
            return;
        }

        Debug.Log($"🎯 Clic detectado en molécula: {nombreMolecula}");

        if (gridController == null)
        {
            Debug.LogError($"GridController no asignado para {nombreMolecula}");

            // Intentar encontrar automáticamente
            gridController = FindAnyObjectByType<GridSeleccionMoleculas>();
            if (gridController == null)
            {
                Debug.LogError("No se encontró GridSeleccionMoleculas en la escena");
                return;
            }
        }

        // Enviar la molécula al grid
        gridController.OnMoleculaSeleccionada(datosMolecula);
    }

    // MÉTODO PÚBLICO para el Inspector - ¡ESTE SÍ APARECE!
    public void SeleccionarEstaMolecula()
    {
        Debug.Log($"SeleccionarEstaMolecula() llamado para {nombreMolecula}");
        OnMoleculaClickeada();
    }

    public void AsignarGridController(GridSeleccionMoleculas controller)
    {
        gridController = controller;
        Debug.Log($"GridController asignado para {nombreMolecula}");
    }

    // Método para debug desde el Inspector
    [ContextMenu("Buscar Texto Automáticamente")]
    public void DebugBuscarTexto()
    {
        BuscarTextoAutomáticamente();
        if (textoNombre != null)
        {
            Debug.Log($"Texto encontrado: {textoNombre.gameObject.name}");
            Debug.Log($"Texto actual: '{textoNombre.text}'");
        }
        else
        {
            Debug.LogError("No se encontró TextMeshProUGUI");
        }
    }

    [ContextMenu("Test Asignar Nombre")]
    public void TestAsignarNombre()
    {
        if (textoNombre != null)
        {
            textoNombre.text = "TEST_NOMBRE";
            Debug.Log($"Texto asignado: 'TEST_NOMBRE'");
        }
        else
        {
            Debug.LogError("textoNombre es null");
        }
    }
}