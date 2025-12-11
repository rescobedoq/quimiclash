// AlbumController.cs (con desactivación simple del botón)
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class AlbumController : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Button botonAbrirAlbum;
    [SerializeField] private GameObject panelAlbumCompleto;
    [SerializeField] private Button botonCerrarAlbum;
    [SerializeField] private Image fondoOverlayAlbum;

    [Header("Configuración de Animación")]
    [SerializeField] private float duracionAnimacion = 0.5f;
    [SerializeField] private AnimationCurve curvaAnimacion = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float escalaInicial = 0.8f;

    [Header("Referencias Componentes")]
    [SerializeField] private Transform contenedorElementosAlbum; // Donde se instanciarán los elementos
    [SerializeField] private GameObject prefabElementoAlbum; // Prefab para cada elemento del álbum

    [Header("Datos del Álbum")]
    [SerializeField] private PlantillaObjetoMolecula[] todasLasMoleculas; // Todas las moléculas del juego

    private bool albumAbierto = false;
    private RectTransform panelTransform;
    private CanvasGroup panelCanvasGroup;
    private AlbumMoleculas albumMoleculasScript; // NUEVO: Referencia al script AlbumMoleculas

    void Awake()
    {
        // Obtener referencias
        if (panelAlbumCompleto != null)
        {
            panelTransform = panelAlbumCompleto.GetComponent<RectTransform>();
            panelCanvasGroup = panelAlbumCompleto.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = panelAlbumCompleto.AddComponent<CanvasGroup>();
        }

        // Buscar el script AlbumMoleculas en el panel o sus hijos
        albumMoleculasScript = panelAlbumCompleto.GetComponentInChildren<AlbumMoleculas>();
        if (albumMoleculasScript == null)
        {
            Debug.LogWarning("No se encontró AlbumMoleculas en el panel del álbum");
        }

        // Configurar botones
        if (botonAbrirAlbum != null)
        {
            botonAbrirAlbum.onClick.RemoveAllListeners();
            botonAbrirAlbum.onClick.AddListener(AbrirAlbum);
            Debug.Log($"✅ Botón '{botonAbrirAlbum.name}' configurado");
        }
        else
        {
            Debug.LogError("❌ botonAbrirAlbum NO está asignado en el Inspector");
        }

        if (botonCerrarAlbum != null)
        {
            botonCerrarAlbum.onClick.RemoveAllListeners();
            botonCerrarAlbum.onClick.AddListener(CerrarAlbum);
        }

        // Configurar fondo overlay
        if (fondoOverlayAlbum != null)
        {
            fondoOverlayAlbum.gameObject.SetActive(false);
            fondoOverlayAlbum.color = new Color(0, 0, 0, 0);
            fondoOverlayAlbum.raycastTarget = true;
        }

        // Ocultar panel al inicio
        if (panelAlbumCompleto != null)
        {
            panelAlbumCompleto.SetActive(false);
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = 0f;
        }

        // Asegurar que el botón esté activo al inicio
        if (botonAbrirAlbum != null)
        {
            botonAbrirAlbum.gameObject.SetActive(true);
            botonAbrirAlbum.interactable = true;
        }
    }

    void Start()
    {
        Debug.Log("AlbumController iniciado correctamente");
        // Cargar datos del álbum si existen
        CargarDatosAlbum();
    }

    public void AbrirAlbum()
    {
        Debug.Log("🎯 MÉTODO AbrirAlbum() EJECUTADO");

        if (albumAbierto)
        {
            Debug.Log("⚠️ Album ya estaba abierto");
            return;
        }

        albumAbierto = true;
        Debug.Log("📖 Abriendo álbum...");

        // 1. DESACTIVAR EL BOTÓN (se oculta completamente)
        if (botonAbrirAlbum != null)
        {
            botonAbrirAlbum.gameObject.SetActive(false);
            Debug.Log("🔘 Botón 'Abrir Album' desactivado");
        }

        // 2. Mostrar panel con animación
        MostrarPanelAlbum();

        // 3. Cargar contenido del álbum (siempre se recrea al abrir)
        CargarContenidoAlbum();
    }

    void MostrarPanelAlbum()
    {
        if (panelAlbumCompleto == null) return;

        // Activar elementos
        panelAlbumCompleto.SetActive(true);

        // Configurar estado inicial
        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        if (panelTransform != null)
            panelTransform.localScale = Vector3.one * escalaInicial;

        if (fondoOverlayAlbum != null)
        {
            fondoOverlayAlbum.gameObject.SetActive(true);
            fondoOverlayAlbum.DOFade(0.7f, duracionAnimacion * 0.5f);
        }

        // Secuencia de animación
        Sequence secuenciaApertura = DOTween.Sequence();

        // Fade in
        if (panelCanvasGroup != null)
        {
            secuenciaApertura.Append(
                panelCanvasGroup.DOFade(1f, duracionAnimacion)
                    .SetEase(curvaAnimacion)
            );
        }

        // Escala
        if (panelTransform != null)
        {
            secuenciaApertura.Join(
                panelTransform.DOScale(Vector3.one, duracionAnimacion)
                    .SetEase(curvaAnimacion)
            );
        }

        // Al terminar
        secuenciaApertura.OnComplete(() => {
            Debug.Log("✅ Álbum abierto completamente");

            // NUEVO: Forzar actualización del álbum de moléculas si existe
            if (albumMoleculasScript != null)
            {
                albumMoleculasScript.RecrearAlbum();
            }
        });
    }

    void CargarContenidoAlbum()
    {
        if (contenedorElementosAlbum == null || prefabElementoAlbum == null)
        {
            Debug.LogWarning("Faltan referencias para cargar el contenido del álbum");
            return;
        }

        // Limpiar contenido previo
        foreach (Transform child in contenedorElementosAlbum)
        {
            Destroy(child.gameObject);
        }

        // Instanciar todas las moléculas
        if (todasLasMoleculas != null && todasLasMoleculas.Length > 0)
        {
            foreach (var molecula in todasLasMoleculas)
            {
                if (molecula == null) continue;

                CrearElementoAlbum(molecula);
            }

            Debug.Log($"✅ Cargadas {todasLasMoleculas.Length} moléculas en el álbum");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay moléculas para mostrar en el álbum");
        }
    }

    void CrearElementoAlbum(PlantillaObjetoMolecula molecula)
    {
        GameObject elemento = Instantiate(prefabElementoAlbum, contenedorElementosAlbum);
        elemento.name = $"AlbumItem_{molecula.nombreMolecula}";

        // Configuración básica del elemento
        // Buscar componentes en el prefab
        Image imagen = elemento.GetComponentInChildren<Image>();
        if (imagen != null && molecula.imagenObjetoMolecula != null)
        {
            imagen.sprite = molecula.imagenObjetoMolecula;
        }

        // Buscar TextMeshPro para el nombre
        TextMeshProUGUI textoNombre = elemento.GetComponentInChildren<TextMeshProUGUI>();
        if (textoNombre != null)
        {
            textoNombre.text = molecula.nombreMolecula;
        }
        else
        {
            // Intentar con Text normal
            Text textoNormal = elemento.GetComponentInChildren<Text>();
            if (textoNormal != null)
            {
                textoNormal.text = molecula.nombreMolecula;
            }
        }

        // Configurar botón si existe
        Button boton = elemento.GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() => OnElementoAlbumClickeado(molecula));
        }
    }

    void OnElementoAlbumClickeado(PlantillaObjetoMolecula molecula)
    {
        Debug.Log($"Elemento de álbum clickeado: {molecula.nombreMolecula}");

        // Aquí puedes agregar funcionalidad cuando se hace clic en un elemento del álbum
        // Por ejemplo: Mostrar detalles, iniciar el nivel, etc.
    }

    public void CerrarAlbum()
    {
        if (!albumAbierto)
        {
            Debug.Log("⚠️ Intento de cerrar álbum cuando ya está cerrado");
            return;
        }

        Debug.Log("📕 Cerrando álbum...");

        // Animación de cierre
        Sequence secuenciaCierre = DOTween.Sequence();

        // Fade out del panel
        if (panelCanvasGroup != null)
        {
            secuenciaCierre.Append(
                panelCanvasGroup.DOFade(0f, duracionAnimacion * 0.7f)
                    .SetEase(curvaAnimacion)
            );
        }

        // Escala hacia abajo
        if (panelTransform != null)
        {
            secuenciaCierre.Join(
                panelTransform.DOScale(Vector3.one * escalaInicial, duracionAnimacion * 0.7f)
                    .SetEase(curvaAnimacion)
            );
        }

        // Fade out del fondo
        if (fondoOverlayAlbum != null)
        {
            secuenciaCierre.Join(
                fondoOverlayAlbum.DOFade(0f, duracionAnimacion * 0.5f)
            );
        }

        // Al terminar
        secuenciaCierre.OnComplete(() => {
            // 1. Ocultar panel del álbum
            panelAlbumCompleto.SetActive(false);

            // 2. Ocultar fondo overlay si existe
            if (fondoOverlayAlbum != null)
                fondoOverlayAlbum.gameObject.SetActive(false);

            // 3. REACTIVAR EL BOTÓN
            if (botonAbrirAlbum != null)
            {
                botonAbrirAlbum.gameObject.SetActive(true);
                Debug.Log("🔘 Botón 'Abrir Album' reactivado");
            }

            // 4. Resetear estado
            albumAbierto = false;
            Debug.Log("✅ Álbum cerrado correctamente");
        });
    }

    void CargarDatosAlbum()
    {
        // Aquí puedes cargar datos guardados del progreso del álbum
        Debug.Log("Cargando datos del álbum...");
        // TODO: Implementar sistema de guardado
    }

    // Métodos públicos para otros scripts
    public bool EstaAbierto() => albumAbierto;

    public void ActualizarAlbum()
    {
        if (albumAbierto)
        {
            CargarContenidoAlbum();

            // También actualizar el álbum de moléculas si existe
            if (albumMoleculasScript != null)
            {
                albumMoleculasScript.RecrearAlbum();
            }
        }
    }

    // Método para notificar cuando se completa una molécula
    public void MoleculaCompletada(PlantillaObjetoMolecula molecula)
    {
        Debug.Log($"Molécula completada: {molecula.nombreMolecula}");
        // Aquí podrías actualizar el estado en el álbum

        if (albumAbierto)
        {
            ActualizarAlbum();
        }
    }

    // Método para forzar recreación del álbum
    public void ForzarRecreacionAlbum()
    {
        if (albumMoleculasScript != null)
        {
            albumMoleculasScript.RecrearAlbum();
        }
    }

    // Métodos para debug
    [ContextMenu("Abrir Album (Debug)")]
    public void DebugAbrirAlbum()
    {
        AbrirAlbum();
    }

    [ContextMenu("Cerrar Album (Debug)")]
    public void DebugCerrarAlbum()
    {
        CerrarAlbum();
    }

    [ContextMenu("Forzar Reactivar Botón")]
    public void ForzarReactivarBoton()
    {
        if (botonAbrirAlbum != null && !botonAbrirAlbum.gameObject.activeSelf)
        {
            botonAbrirAlbum.gameObject.SetActive(true);
            Debug.Log("Botón reactivado manualmente");
        }
    }

    [ContextMenu("Forzar Recrear Álbum Moléculas")]
    public void DebugForzarRecrearAlbum()
    {
        ForzarRecreacionAlbum();
    }

    void OnDestroy()
    {
        // Limpiar DOTween
        DOTween.Kill(this);
    }
}