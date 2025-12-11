using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIControladorMensajes : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject ventanaInformacion;
    [SerializeField] private TextMeshProUGUI textoDescripcion;
    [SerializeField] private TextMeshProUGUI textoTitulo;
    [SerializeField] private Button botonCerrar;
    [SerializeField] private Image fondoInformacion;

    [Header("Configuración")]
    [SerializeField] private bool mostrarDebug = true;

    private static UIControladorMensajes instancia;
    private VentanaFelicitaciones ventanaFelicitacionesCache;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (ventanaInformacion != null)
        {
            ventanaInformacion.SetActive(false);
        }
        else
        {
            Debug.LogError("UIControladorMensajes: No se asignó el Panel_Informacion!");
        }

        if (botonCerrar != null)
        {
            botonCerrar.onClick.AddListener(CerrarVentana);
        }
        else
        {
            Debug.LogError("UIControladorMensajes: No se asignó el botón de cerrar!");
        }

        // Buscar la ventana de felicitaciones al inicio para mejor performance
        InicializarVentanaFelicitaciones();

        if (mostrarDebug)
        {
            Debug.Log("UIControladorMensajes inicializado correctamente");
        }
    }

    void InicializarVentanaFelicitaciones()
    {
        // Buscar la ventana de felicitaciones al inicio
        ventanaFelicitacionesCache = ObtenerVentanaFelicitaciones();

        if (ventanaFelicitacionesCache != null && mostrarDebug)
        {
            Debug.Log($"VentanaFelicitaciones inicializada: {ventanaFelicitacionesCache.gameObject.name}");
        }
        else if (mostrarDebug)
        {
            Debug.LogWarning("VentanaFelicitaciones no encontrada durante la inicialización");
        }
    }

    public static UIControladorMensajes Instancia => instancia;

    public void MostrarMensajeParaMolecula(PlantillaObjetoMolecula molecula)
    {
        if (molecula == null)
        {
            Debug.LogError("UIControladorMensajes: Molécula nula!");
            return;
        }

        if (mostrarDebug)
        {
            Debug.Log($"=== MostrarMensajeParaMolecula ===");
            Debug.Log($"Nombre: {molecula.nombreMolecula}");
            Debug.Log($"Estado: {(molecula.desbloqueada ? "DESBLOQUEADA" : "BLOQUEADA")}");
        }

        // VERIFICAR DIRECTAMENTE EL BOOLEANO DE LA PLANTILLA
        if (molecula.desbloqueada)
        {
            if (mostrarDebug)
                Debug.Log("La molécula está DESBLOQUEADA - Mostrando ventana de felicitaciones");

            // MOLÉCULA YA DESBLOQUEADA - Mostrar ventana de felicitaciones
            MostrarVentanaFelicitaciones(molecula);
        }
        else
        {
            if (mostrarDebug)
                Debug.Log("La molécula NO está desbloqueada - Mostrando ventana emergente");

            // MOLÉCULA NO DESBLOQUEADA - Mostrar ventana emergente con elementos necesarios
            MostrarInformacionElementos(molecula);
        }
    }

    private void MostrarInformacionElementos(PlantillaObjetoMolecula molecula)
    {
        if (ventanaInformacion == null)
        {
            Debug.LogError("UIControladorMensajes: Panel_Informacion no asignado!");
            return;
        }

        if (textoDescripcion != null)
        {
            string mensaje = $"Molécula: {molecula.nombreMolecula}\n\nElementos necesarios:\n";

            // Obtener elementos únicos necesarios
            List<string> elementosNecesarios = ObtenerElementosUnicos(molecula);

            foreach (string elemento in elementosNecesarios)
            {
                mensaje += $"• {elemento}\n";
            }

            textoDescripcion.text = mensaje;
        }
        else
        {
            Debug.LogError("UIControladorMensajes: Texto de descripción no asignado!");
        }

        // Mostrar ventana
        ventanaInformacion.SetActive(true);

        // Asegurarse de que esté en primer plano
        if (ventanaInformacion.transform.parent != null)
        {
            ventanaInformacion.transform.SetAsLastSibling();
        }

        if (mostrarDebug)
        {
            Debug.Log("Ventana emergente activada (elementos necesarios)");
        }
    }

    private void MostrarVentanaFelicitaciones(PlantillaObjetoMolecula molecula)
    {
        // Obtener o encontrar la ventana de felicitaciones
        VentanaFelicitaciones ventana = ObtenerVentanaFelicitaciones();

        if (ventana != null)
        {
            if (mostrarDebug)
            {
                Debug.Log($"Mostrando ventana de felicitaciones para: {molecula.nombreMolecula} (YA DESBLOQUEADA)");
                Debug.Log($"Ventana encontrada: {ventana.gameObject.name}");
            }

            // Asegurarse de que la ventana esté activa
            if (!ventana.gameObject.activeSelf)
            {
                ventana.gameObject.SetActive(true);
            }

            ventana.MostrarFelicitaciones(molecula);
        }
        else
        {
            Debug.LogError("UIControladorMensajes: ¡No se pudo encontrar ni crear VentanaFelicitaciones!");

            // Mostrar mensaje de error en la ventana emergente como fallback
            MostrarMensajeErrorFelicitaciones(molecula);
        }
    }

    private VentanaFelicitaciones ObtenerVentanaFelicitaciones()
    {
        // 1. Usar caché si está disponible
        if (ventanaFelicitacionesCache != null && ventanaFelicitacionesCache.gameObject != null)
            return ventanaFelicitacionesCache;

        // 2. Buscar en la escena
        VentanaFelicitaciones ventana = FindFirstObjectByType<VentanaFelicitaciones>();

        if (ventana != null)
        {
            ventanaFelicitacionesCache = ventana;
            return ventana;
        }

        // 3. Buscar más exhaustivamente (todos los objetos cargados)
        VentanaFelicitaciones[] todasLasVentanas = Resources.FindObjectsOfTypeAll<VentanaFelicitaciones>();

        if (todasLasVentanas.Length > 0)
        {
            ventana = todasLasVentanas[0];
            ventanaFelicitacionesCache = ventana;

            if (mostrarDebug)
            {
                Debug.Log($"VentanaFelicitaciones encontrada en Resources: {ventana.gameObject.name}");
            }

            return ventana;
        }

        // 4. Intentar cargar desde Resources
        GameObject prefabVentana = Resources.Load<GameObject>("VentanaFelicitaciones");
        if (prefabVentana != null)
        {
            GameObject instancia = Instantiate(prefabVentana);
            ventana = instancia.GetComponent<VentanaFelicitaciones>();
            ventanaFelicitacionesCache = ventana;

            if (mostrarDebug)
                Debug.Log("VentanaFelicitaciones instanciada desde Resources");

            return ventana;
        }

        // 5. Último intento: buscar por nombre
        GameObject ventanaObj = GameObject.Find("VentanaFelicitaciones");
        if (ventanaObj == null) ventanaObj = GameObject.Find("Panel_Felicidades");
        if (ventanaObj == null) ventanaObj = GameObject.Find("FelicitacionesPanel");

        if (ventanaObj != null)
        {
            ventana = ventanaObj.GetComponent<VentanaFelicitaciones>();
            if (ventana != null)
            {
                ventanaFelicitacionesCache = ventana;
                return ventana;
            }
        }

        // No se encontró
        return null;
    }

    private void MostrarMensajeErrorFelicitaciones(PlantillaObjetoMolecula molecula)
    {
        string mensajeError = $"La molécula <b>{molecula.nombreMolecula}</b> ya ha sido desbloqueada.\n\n";
        mensajeError += "(Ventana de felicitaciones no disponible)";

        MostrarMensajeSimple("¡Ya Desbloqueado!", mensajeError);
    }

    private List<string> ObtenerElementosUnicos(PlantillaObjetoMolecula molecula)
    {
        List<string> elementosUnicos = new List<string>();

        if (molecula != null && molecula.elementosEnCoordenadas != null)
        {
            foreach (var elementoCoordenada in molecula.elementosEnCoordenadas)
            {
                if (!elementosUnicos.Contains(elementoCoordenada.nombreElemento))
                {
                    elementosUnicos.Add(elementoCoordenada.nombreElemento);
                }
            }
        }

        return elementosUnicos;
    }

    // Método original para compatibilidad (no verifica estado)
    public void MostrarMensajeElementos(string nombreMolecula, List<string> elementosNecesarios)
    {
        if (ventanaInformacion == null || textoDescripcion == null) return;

        if (textoTitulo != null)
        {
            textoTitulo.text = $"Molécula: {nombreMolecula}";
        }

        string mensaje = $"Molécula: {nombreMolecula}\n\nElementos necesarios:\n";
        foreach (string elemento in elementosNecesarios)
        {
            mensaje += $"• {elemento}\n";
        }

        textoDescripcion.text = mensaje;
        ventanaInformacion.SetActive(true);

        if (ventanaInformacion.transform.parent != null)
        {
            ventanaInformacion.transform.SetAsLastSibling();
        }
    }

    public void MostrarMensajeSimple(string titulo, string mensaje)
    {
        if (ventanaInformacion == null || textoDescripcion == null) return;

        if (textoTitulo != null)
        {
            textoTitulo.text = titulo;
        }

        textoDescripcion.text = mensaje;
        ventanaInformacion.SetActive(true);

        if (ventanaInformacion.transform.parent != null)
        {
            ventanaInformacion.transform.SetAsLastSibling();
        }
    }

    public void CerrarVentana()
    {
        if (ventanaInformacion != null)
        {
            ventanaInformacion.SetActive(false);
            if (mostrarDebug) Debug.Log("Ventana de información cerrada");
        }
    }

    public bool VentanaActiva()
    {
        return ventanaInformacion != null && ventanaInformacion.activeSelf;
    }

    [ContextMenu("Verificar Configuración")]
    public void VerificarConfiguracion()
    {
        Debug.Log("=== VERIFICACIÓN UIControladorMensajes ===");
        Debug.Log($"Panel Información: {(ventanaInformacion != null ? "Asignado" : "NO ASIGNADO")}");
        Debug.Log($"Texto Descripción: {(textoDescripcion != null ? "Asignado" : "NO ASIGNADO")}");
        Debug.Log($"Texto Título: {(textoTitulo != null ? "Asignado" : "NO ASIGNADO")}");
        Debug.Log($"Botón Cerrar: {(botonCerrar != null ? "Asignado" : "NO ASIGNADO")}");
        Debug.Log($"Fondo Información: {(fondoInformacion != null ? "Asignado" : "NO ASIGNADO")}");

        VentanaFelicitaciones ventana = ObtenerVentanaFelicitaciones();
        Debug.Log($"Ventana Felicitaciones: {(ventana != null ? $"ENCONTRADA ({ventana.gameObject.name})" : "NO ENCONTRADA")}");

        Debug.Log("=== FIN VERIFICACIÓN ===");
    }

    [ContextMenu("Debug: Buscar VentanaFelicitaciones")]
    public void DebugBuscarVentanaFelicitaciones()
    {
        Debug.Log("=== BUSCANDO VentanaFelicitaciones ===");

        // USANDO EL NUEVO MÉTODO FindObjectsByType
#if UNITY_6000_0_OR_NEWER
        // Para Unity 2022.3+ / 6000.0+
        VentanaFelicitaciones[] todas = FindObjectsByType<VentanaFelicitaciones>(FindObjectsSortMode.None);
#else
        // Para versiones anteriores
        VentanaFelicitaciones[] todas = FindObjectsOfType<VentanaFelicitaciones>();
#endif

        Debug.Log($"Ventanas encontradas en escena: {todas.Length}");

        foreach (var ventana in todas)
        {
            Debug.Log($"- {ventana.name}");
        }

        if (todas.Length == 0)
        {
            Debug.LogError("¡NO SE ENCONTRÓ NINGUNA VentanaFelicitaciones en la escena!");
        }
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    [ContextMenu("Debug: Test Ventana Felicitaciones")]
    public void DebugTestVentanaFelicitaciones()
    {
        Debug.Log("=== TEST VentanaFelicitaciones ===");

        VentanaFelicitaciones ventana = ObtenerVentanaFelicitaciones();

        if (ventana != null)
        {
            Debug.Log($"✓ VentanaFelicitaciones encontrada: {ventana.gameObject.name}");
            Debug.Log($"  Activa: {ventana.gameObject.activeSelf}");

            // Crear una molécula de prueba
            PlantillaObjetoMolecula testMolecula = ScriptableObject.CreateInstance<PlantillaObjetoMolecula>();
            testMolecula.nombreMolecula = "AGUA_TEST";
            testMolecula.desbloqueada = true;

            Debug.Log("Intentando mostrar felicitaciones para molécula de prueba...");
            ventana.MostrarFelicitaciones(testMolecula);
        }
        else
        {
            Debug.LogError("✗ No se pudo encontrar VentanaFelicitaciones");
        }
    }
}