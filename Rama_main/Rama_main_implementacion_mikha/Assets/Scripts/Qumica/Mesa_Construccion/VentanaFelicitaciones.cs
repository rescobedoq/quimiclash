using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class VentanaFelicitaciones : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject panelFelicitaciones;
    [SerializeField] private Image imagenBaseFelicitacion;
    [SerializeField] private Image imagenDescripcionMolecula;
    [SerializeField] private Button botonCerrar;
    [SerializeField] private ParticleSystem particulasConfetti; // Opcional

    [Header("Fondo Overlay (Oscuro)")]
    [SerializeField] private Image fondoOverlay;
    [SerializeField] private Color colorOverlay = new Color(0f, 0f, 0f, 0.7f);
    [SerializeField] private float duracionFadeOverlay = 0.3f;
    [SerializeField] private AnimationCurve curvaFadeOverlay = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Configuración Canvas")]
    [SerializeField] private int sortOrderAlto = 9999;

    [Header("Configuración Animaciones")]
    [SerializeField] private float duracionEntrada = 0.8f;
    [SerializeField] private float duracionSalida = 0.5f;
    [SerializeField] private AnimationCurve curvaEntrada = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve curvaSalida = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float duracionAparecerPanel = 0.5f; // Nueva duración específica para el panel
    [SerializeField] private AnimationCurve curvaAparecerPanel = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool usarEfectosParticulas = true;
    [SerializeField] private bool usarEfectoSonido = true;

    [Header("Configuración Flotación")]
    [SerializeField] private float flotacionIntensidad = 0.02f; // MUCHO más pequeño para que no salga de cámara
    [SerializeField] private float flotacionVelocidad = 1.5f;

    [Header("Referencias para Animación")]
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private CanvasGroup canvasGroupContenido;

    private PlantillaObjetoMolecula moleculaActual;
    private Canvas canvasComponent;
    private Vector3 escalaOriginalPanel;
    private Sequence secuenciaFlotante;
    private CanvasGroup panelCanvasGroup; // CanvasGroup específico para el panel

    void Awake()
    {
        // Obtener referencias
        canvasComponent = GetComponent<Canvas>();

        if (panelTransform == null && panelFelicitaciones != null)
            panelTransform = panelFelicitaciones.GetComponent<RectTransform>();

        if (canvasGroupContenido == null && panelFelicitaciones != null)
            canvasGroupContenido = panelFelicitaciones.GetComponent<CanvasGroup>() ?? panelFelicitaciones.AddComponent<CanvasGroup>();

        // CanvasGroup específico para el panel (solo para fade)
        if (panelFelicitaciones != null)
        {
            panelCanvasGroup = panelFelicitaciones.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = panelFelicitaciones.AddComponent<CanvasGroup>();
        }

        // Guardar escala original del panel
        if (panelTransform != null)
        {
            escalaOriginalPanel = panelTransform.localScale;
        }

        // Ocultar al inicio
        OcultarVentanaInmediato();

        // Configurar botón
        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(OcultarVentanaConAnimacion);
        }

        // Configurar fondo overlay
        if (fondoOverlay != null)
        {
            fondoOverlay.color = new Color(colorOverlay.r, colorOverlay.g, colorOverlay.b, 0f);
            fondoOverlay.gameObject.SetActive(false);
            fondoOverlay.raycastTarget = true;
        }
    }

    public void MostrarFelicitaciones(PlantillaObjetoMolecula molecula)
    {
        if (molecula == null)
        {
            Debug.LogError("Intento de mostrar felicitaciones con molécula nula");
            return;
        }

        moleculaActual = molecula;

        // Marcar como desbloqueada en memoria
        molecula.Desbloquear();

        // Buscar AlbumMoleculas para notificar del desbloqueo
        // Esto manejará la persistencia automáticamente según su configuración
        AlbumMoleculas album = FindFirstObjectByType<AlbumMoleculas>();
        if (album != null)
        {
            album.DesbloquearMolecula(molecula.nombreMolecula);
        }
        else
        {
            Debug.LogWarning("No se encontró AlbumMoleculas en la escena - Solo se desbloqueó en memoria");
        }

        // Asegurar que el Canvas esté adelante
        TraerAdelante();

        // Configurar imágenes (inicialmente invisibles)
        if (imagenDescripcionMolecula != null && molecula.imagenDescripcionMolecula != null)
        {
            imagenDescripcionMolecula.sprite = molecula.imagenDescripcionMolecula;
            imagenDescripcionMolecula.color = new Color(1f, 1f, 1f, 0f); // Invisible al inicio
            imagenDescripcionMolecula.preserveAspect = true;
            imagenDescripcionMolecula.gameObject.SetActive(true);
        }

        if (imagenBaseFelicitacion != null)
        {
            imagenBaseFelicitacion.color = new Color(1f, 1f, 1f, 0f); // Invisible al inicio
            imagenBaseFelicitacion.gameObject.SetActive(true);
        }

        // Ocultar el botón inicialmente
        if (botonCerrar != null)
        {
            CanvasGroup botonCanvasGroup = botonCerrar.GetComponent<CanvasGroup>();
            if (botonCanvasGroup == null)
                botonCanvasGroup = botonCerrar.gameObject.AddComponent<CanvasGroup>();
            botonCanvasGroup.alpha = 0f;
        }

        // Reproducir sonido de éxito
        if (usarEfectoSonido)
            ReproducirSonidoExito();

        // Mostrar con animación
        MostrarConAnimacion();

        Debug.Log($"Ventana de felicitaciones mostrada para: {molecula.nombreMolecula}");
        Debug.Log($"Estado en memoria: {(molecula.desbloqueada ? "DESBLOQUEADA" : "BLOQUEADA")}");
    }

    void MostrarConAnimacion()
    {
        // Asegurar que el panel esté activo pero invisible
        if (panelFelicitaciones != null)
            panelFelicitaciones.SetActive(true);

        // Configurar el panel como invisible
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }

        // Configurar elementos internos del panel como invisibles
        if (canvasGroupContenido != null)
        {
            canvasGroupContenido.alpha = 0f;
            canvasGroupContenido.interactable = false;
            canvasGroupContenido.blocksRaycasts = false;
        }

        // Asegurar que las imágenes estén invisibles
        if (imagenBaseFelicitacion != null)
        {
            imagenBaseFelicitacion.color = new Color(1f, 1f, 1f, 0f);
        }

        if (imagenDescripcionMolecula != null)
        {
            imagenDescripcionMolecula.color = new Color(1f, 1f, 1f, 0f);
        }

        if (botonCerrar != null)
        {
            CanvasGroup botonCanvasGroup = botonCerrar.GetComponent<CanvasGroup>();
            if (botonCanvasGroup != null)
            {
                botonCanvasGroup.alpha = 0f;
            }
        }

        // Restaurar escala original del panel
        if (panelTransform != null)
        {
            panelTransform.localScale = escalaOriginalPanel;
        }

        // Detener animaciones previas
        if (secuenciaFlotante != null && secuenciaFlotante.IsActive())
            secuenciaFlotante.Kill();

        // 1. ANIMACIÓN DEL FONDO OVERLAY (independiente)
        if (fondoOverlay != null)
        {
            fondoOverlay.gameObject.SetActive(true);
            fondoOverlay.DOFade(colorOverlay.a, duracionFadeOverlay)
                .SetEase(curvaFadeOverlay);
        }

        // 2. ANIMACIÓN PROGRESIVA SOLO DEL PANEL (0.5 segundos)
        DOVirtual.DelayedCall(duracionFadeOverlay * 0.3f, () => {
            if (panelCanvasGroup != null)
            {
                // Solo el panel tiene animación progresiva de fade in
                panelCanvasGroup.DOFade(1f, duracionAparecerPanel)
                    .SetEase(curvaAparecerPanel)
                    .OnComplete(() => {
                        // Una vez que el panel es visible, animar sus elementos internos
                        AnimarElementosInternos();
                    });
            }
            else
            {
                // Fallback si no hay CanvasGroup en el panel
                AnimarElementosInternos();
            }
        });
    }

    void AnimarElementosInternos()
    {
        // ANIMAR ELEMENTOS INTERNOS DEL PANEL
        // (imagenBaseFelicitacion, imagenDescripcionMolecula, botonCerrar)
        // con animación suave y sin agrandarse demasiado

        // Detener animaciones previas
        if (secuenciaFlotante != null && secuenciaFlotante.IsActive())
            secuenciaFlotante.Kill();

        // Secuencia para elementos internos - aparecen secuencialmente
        Sequence secuenciaInterna = DOTween.Sequence();

        // 1. Fade in de los elementos con retraso entre ellos
        if (imagenBaseFelicitacion != null)
        {
            // Fade in de la imagen base
            secuenciaInterna.Append(
                imagenBaseFelicitacion.DOFade(1f, duracionEntrada * 0.3f).SetEase(Ease.OutQuad));
        }

        if (imagenDescripcionMolecula != null)
        {
            // Fade in de la imagen descripción - después de la base
            secuenciaInterna.AppendInterval(0.1f);
            secuenciaInterna.Append(
                imagenDescripcionMolecula.DOFade(1f, duracionEntrada * 0.3f).SetEase(Ease.OutQuad));
        }

        if (botonCerrar != null)
        {
            CanvasGroup botonCanvasGroup = botonCerrar.GetComponent<CanvasGroup>();
            if (botonCanvasGroup != null)
            {
                // Fade in del botón - al final
                secuenciaInterna.AppendInterval(0.1f);
                secuenciaInterna.Append(
                    botonCanvasGroup.DOFade(1f, duracionEntrada * 0.3f).SetEase(Ease.OutQuad));
            }
        }

        // 2. Efectos finales
        secuenciaInterna.AppendCallback(() => {
            if (canvasGroupContenido != null)
            {
                canvasGroupContenido.interactable = true;
                canvasGroupContenido.blocksRaycasts = true;
            }

            // Efecto de partículas
            if (usarEfectosParticulas && particulasConfetti != null)
            {
                particulasConfetti.Play();
            }

            // Efecto de brillo en los bordes SOLO de imagenBaseFelicitacion
            StartCoroutine(EfectoBrilloBordes());

            // Iniciar animación flotante suave del PANEL completo
            IniciarAnimacionFlotanteSuave();
        });

        // Efecto de destello inicial SUAVE
        secuenciaInterna.AppendCallback(() => {
            StartCoroutine(EfectoDestelloSuave());
        });
    }

    IEnumerator EfectoBrilloBordes()
    {
        if (imagenBaseFelicitacion != null)
        {
            float tiempo = 0f;
            float duracionBrillo = 2f;

            while (tiempo < duracionBrillo)
            {
                tiempo += Time.deltaTime;
                float intensidad = Mathf.PingPong(tiempo * 2f, 1f) * 0.3f + 0.7f; // Más sutil

                // Cambiar color para efecto de brillo
                Color colorBrillo = Color.Lerp(Color.white, new Color(1f, 0.95f, 0.8f, 1f), intensidad);
                imagenBaseFelicitacion.color = new Color(colorBrillo.r, colorBrillo.g, colorBrillo.b, imagenBaseFelicitacion.color.a);

                yield return null;
            }

            imagenBaseFelicitacion.color = new Color(1f, 1f, 1f, imagenBaseFelicitacion.color.a);
        }
    }

    IEnumerator EfectoDestelloSuave()
    {
        if (imagenBaseFelicitacion == null) yield break;

        // Destello más sutil (solo 1 vez y menos intenso)
        imagenBaseFelicitacion.color = new Color(1f, 1f, 0.9f, imagenBaseFelicitacion.color.a);
        yield return new WaitForSecondsRealtime(0.15f);
        imagenBaseFelicitacion.color = new Color(1f, 1f, 1f, imagenBaseFelicitacion.color.a);
    }

    void IniciarAnimacionFlotanteSuave()
    {
        // Detener animación flotante previa si existe
        if (secuenciaFlotante != null && secuenciaFlotante.IsActive())
            secuenciaFlotante.Kill();

        if (panelTransform == null) return;

        // Crear nueva animación flotante MUY SUAVE para TODO el PANEL
        secuenciaFlotante = DOTween.Sequence();

        // Animación flotante MUY sutil del panel completo (todos los elementos flotan juntos)
        // Usando una escala MUY pequeña para no salir de cámara
        secuenciaFlotante.Append(
                panelTransform.DOScale(escalaOriginalPanel * (1f + flotacionIntensidad), flotacionVelocidad)
                .SetEase(Ease.InOutSine))
            .Append(
                panelTransform.DOScale(escalaOriginalPanel, flotacionVelocidad)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OcultarVentanaConAnimacion()
    {
        // Desactivar interacción inmediatamente
        if (canvasGroupContenido != null)
        {
            canvasGroupContenido.interactable = false;
            canvasGroupContenido.blocksRaycasts = false;
        }

        // Detener animación flotante
        if (secuenciaFlotante != null && secuenciaFlotante.IsActive())
            secuenciaFlotante.Kill();

        // 1. Primero ocultar elementos internos
        Sequence secuenciaSalida = DOTween.Sequence();

        // Fade out de elementos (más rápido)
        if (imagenBaseFelicitacion != null)
        {
            secuenciaSalida.Insert(0f,
                imagenBaseFelicitacion.DOFade(0f, duracionSalida * 0.3f).SetEase(Ease.OutQuad));
        }

        if (imagenDescripcionMolecula != null)
        {
            secuenciaSalida.Insert(0.1f,
                imagenDescripcionMolecula.DOFade(0f, duracionSalida * 0.3f).SetEase(Ease.OutQuad));
        }

        if (botonCerrar != null)
        {
            CanvasGroup botonCanvasGroup = botonCerrar.GetComponent<CanvasGroup>();
            if (botonCanvasGroup != null)
            {
                secuenciaSalida.Insert(0.2f,
                    botonCanvasGroup.DOFade(0f, duracionSalida * 0.3f).SetEase(Ease.OutQuad));
            }
        }

        // 2. Luego ocultar el panel completo progresivamente
        secuenciaSalida.AppendInterval(duracionSalida * 0.2f);
        secuenciaSalida.AppendCallback(() => {
            if (panelCanvasGroup != null)
            {
                // Solo el panel tiene animación progresiva de fade out
                panelCanvasGroup.DOFade(0f, duracionAparecerPanel * 0.6f)
                    .SetEase(curvaSalida)
                    .OnComplete(() => {
                        // 3. Finalmente ocultar el fondo overlay
                        OcultarFondoOverlay();
                    });
            }
            else
            {
                OcultarFondoOverlay();
            }
        });
    }

    void OcultarFondoOverlay()
    {
        // 1. Animación de fade out del fondo overlay (independiente)
        if (fondoOverlay != null)
        {
            fondoOverlay.DOFade(0f, duracionFadeOverlay)
                .SetEase(curvaFadeOverlay)
                .OnComplete(() => {
                    OcultarVentanaInmediato();

                    // Reproducir sonido de cierre
                    if (usarEfectoSonido)
                        ReproducirSonidoCierre();
                });
        }
        else
        {
            OcultarVentanaInmediato();

            // Reproducir sonido de cierre
            if (usarEfectoSonido)
                ReproducirSonidoCierre();
        }
    }

    void OcultarVentanaInmediato()
    {
        if (panelFelicitaciones != null)
        {
            panelFelicitaciones.SetActive(false);
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }

        if (canvasGroupContenido != null)
        {
            canvasGroupContenido.alpha = 0f;
            canvasGroupContenido.interactable = false;
            canvasGroupContenido.blocksRaycasts = false;
        }

        // Restaurar opacidad de imágenes
        if (imagenBaseFelicitacion != null)
        {
            imagenBaseFelicitacion.color = new Color(1f, 1f, 1f, 0f);
        }

        if (imagenDescripcionMolecula != null)
        {
            imagenDescripcionMolecula.color = new Color(1f, 1f, 1f, 0f);
        }

        if (botonCerrar != null)
        {
            CanvasGroup botonCanvasGroup = botonCerrar.GetComponent<CanvasGroup>();
            if (botonCanvasGroup != null)
            {
                botonCanvasGroup.alpha = 0f;
            }
        }

        // Restaurar escala original del panel
        if (panelTransform != null)
        {
            panelTransform.localScale = escalaOriginalPanel;
        }

        // Ocultar fondo overlay
        if (fondoOverlay != null)
        {
            fondoOverlay.gameObject.SetActive(false);
            fondoOverlay.color = new Color(colorOverlay.r, colorOverlay.g, colorOverlay.b, 0f);
        }

        // Detener partículas
        if (particulasConfetti != null)
            particulasConfetti.Stop();

        // Detener animación flotante
        if (secuenciaFlotante != null && secuenciaFlotante.IsActive())
            secuenciaFlotante.Kill();

        moleculaActual = null;
    }

    // MÉTODO QUE FALTABA - Para que GridSeleccionMoleculas pueda llamarlo
    public void TraerAdelante()
    {
        if (canvasComponent == null)
            canvasComponent = GetComponent<Canvas>();

        if (canvasComponent != null)
        {
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.sortingOrder = sortOrderAlto;
            canvasComponent.overrideSorting = true;

            Debug.Log($"Canvas traído adelante: Sort Order = {sortOrderAlto}");
        }
    }

    // Método alternativo por si necesitas configurar desde otro script
    public void ConfigurarCanvasAdelante(int nuevoSortOrder = -1)
    {
        if (canvasComponent == null)
            canvasComponent = GetComponent<Canvas>();

        if (canvasComponent != null)
        {
            if (nuevoSortOrder > 0)
                sortOrderAlto = nuevoSortOrder;

            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.sortingOrder = sortOrderAlto;
            canvasComponent.overrideSorting = true;
        }
    }

    void ReproducirSonidoExito()
    {
        // Buscar AudioSource o crear uno
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Cargar sonido de Resources o usar uno por defecto
        AudioClip clip = Resources.Load<AudioClip>("Sonidos/Felicidades");
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.Log("No se encontró sonido de felicitaciones. Creando tono simple...");
            // Podrías generar un tono simple aquí
        }
    }

    void ReproducirSonidoCierre()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        // Opcional: reproducir sonido de cierre
        AudioClip clipCierre = Resources.Load<AudioClip>("Sonidos/Cierre");
        if (clipCierre != null)
        {
            audioSource.PlayOneShot(clipCierre);
        }
    }

    // Método para verificar si está activa
    public bool EstaActiva()
    {
        return panelFelicitaciones != null && panelFelicitaciones.activeSelf;
    }

    // Métodos para debug
    [ContextMenu("Test: Animación Entrada")]
    public void TestAnimacionEntrada()
    {
        TraerAdelante();

        if (panelFelicitaciones != null)
            panelFelicitaciones.SetActive(true);

        MostrarConAnimacion();
    }

    [ContextMenu("Test: Animación Salida")]
    public void TestAnimacionSalida()
    {
        OcultarVentanaConAnimacion();
    }

    [ContextMenu("Test: Traer Adelante")]
    public void TestTraerAdelante()
    {
        TraerAdelante();
    }

    void OnDestroy()
    {
        // Limpiar DOTween al destruir
        if (secuenciaFlotante != null && secuenciaFlotante.IsActive())
            secuenciaFlotante.Kill();
    }
}