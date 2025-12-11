﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AlbumMoleculas : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject prefabBotonMolecula;

    [Header("Lista de Moléculas")]
    [SerializeField] private List<PlantillaObjetoMolecula> todasLasMoleculas;

    [Header("Organización")]
    [SerializeField] private float espaciadoHorizontal = 120f;
    [SerializeField] private float espaciadoVertical = 120f;

    private List<GameObject> botonesInstanciados = new List<GameObject>();
    private HashSet<string> moleculasCompletadas = new HashSet<string>();
    private const string PLAYER_PREFS_KEY = "MoleculasCompletadas";
    private const string PLAYER_PREFS_DESBLOQUEOS = "MoleculasDesbloqueadas";

    // Variable para controlar si usamos persistencia ENTRE SESIONES
    [SerializeField] private bool usarPersistencia = true; // <-- CAMBIADO A TRUE por defecto

    void Start()
    {
        Debug.Log("🔄 AlbumMoleculas iniciando...");
        Debug.Log($"Modo persistencia (entre sesiones): {(usarPersistencia ? "ACTIVADO" : "DESACTIVADO")}");

        // Cargar progreso general
        CargarProgreso();
        
        // Cargar desbloqueos según el modo
        if (usarPersistencia)
        {
            // Modo PERSISTENTE: Cargar de PlayerPrefs
            CargarDesbloqueosDesdePlayerPrefs();
            Debug.Log("📥 Desbloqueos cargados desde PlayerPrefs (persistente)");
        }
        else
        {
            // Modo NO PERSISTENTE: Los estados se mantienen en memoria durante la sesión
            Debug.Log("📝 Modo no persistente - Estados guardados solo en memoria");
            
            // IMPORTANTE: NO resetear aquí, dejar los estados actuales en memoria
            // Solo asegurar que PlayerPrefs no interfiera
            if (PlayerPrefs.HasKey(PLAYER_PREFS_DESBLOQUEOS))
            {
                Debug.Log("⚠️ PlayerPrefs encontrado pero IGNORADO (modo no persistente)");
            }
        }
    }

    public void RecrearAlbum()
    {
        Debug.Log("🔄 RECREANDO ÁLBUM DE MOLÉCULAS...");

        // No necesitamos recargar nada, los estados ya están en memoria
        // Solo crear los botones con los estados actuales
        CrearBotonesOrganizadosPorDificultad();
    }

    void CargarDesbloqueosDesdePlayerPrefs()
    {
        // Solo ejecutar si la persistencia está ACTIVADA
        if (!usarPersistencia)
        {
            Debug.Log("❌ Persistencia desactivada - Saltando carga de PlayerPrefs");
            return;
        }

        string datos = PlayerPrefs.GetString(PLAYER_PREFS_DESBLOQUEOS, "");
        if (string.IsNullOrEmpty(datos))
        {
            Debug.Log("📭 No hay datos de desbloqueos guardados en PlayerPrefs");
            return;
        }

        string[] nombresDesbloqueados = datos.Split(',');
        HashSet<string> desbloqueadosSet = new HashSet<string>(nombresDesbloqueados);

        int actualizadas = 0;
        foreach (var molecula in todasLasMoleculas)
        {
            if (molecula != null)
            {
                bool estabaDesbloqueada = molecula.desbloqueada;
                molecula.desbloqueada = desbloqueadosSet.Contains(molecula.nombreMolecula);

                if (estabaDesbloqueada != molecula.desbloqueada && molecula.desbloqueada)
                {
                    actualizadas++;
                    Debug.Log($"📥 Cargada de PlayerPrefs: {molecula.nombreMolecula}");
                }
            }
        }

        Debug.Log($"📊 {actualizadas} moléculas cargadas desde PlayerPrefs");
    }

    void GuardarDesbloqueosEnPlayerPrefs()
    {
        // Solo guardar si la persistencia está ACTIVADA
        if (!usarPersistencia)
        {
            Debug.Log("❌ Persistencia desactivada - No se guarda en PlayerPrefs");
            return;
        }

        List<string> moleculasDesbloqueadas = new List<string>();

        foreach (var molecula in todasLasMoleculas)
        {
            if (molecula != null && molecula.desbloqueada)
            {
                moleculasDesbloqueadas.Add(molecula.nombreMolecula);
            }
        }

        string datos = string.Join(",", moleculasDesbloqueadas);
        PlayerPrefs.SetString(PLAYER_PREFS_DESBLOQUEOS, datos);
        PlayerPrefs.Save();

        Debug.Log($"💾 Guardadas {moleculasDesbloqueadas.Count} moléculas en PlayerPrefs");
        Debug.Log($"Datos: {datos}");
    }

    void CrearBotonesOrganizadosPorDificultad()
    {
        Debug.Log($"Organizando {todasLasMoleculas.Count} moléculas por dificultad");

        LimpiarBotones();

        if (prefabBotonMolecula == null)
        {
            Debug.LogError("¡Falta el prefab Button_Elemento_Album!");
            return;
        }

        var moleculasOrdenadas = todasLasMoleculas
            .Where(m => m != null)
            .OrderBy(m => m.dificultad)
            .ToList();

        Dictionary<int, List<PlantillaObjetoMolecula>> porDificultad = new Dictionary<int, List<PlantillaObjetoMolecula>>();

        foreach (var molecula in moleculasOrdenadas)
        {
            int dificultad = molecula.dificultad;
            if (!porDificultad.ContainsKey(dificultad))
                porDificultad[dificultad] = new List<PlantillaObjetoMolecula>();

            porDificultad[dificultad].Add(molecula);
        }

        float posY = 0f;
        var dificultadesOrdenadas = porDificultad.Keys.OrderBy(d => d).ToList();

        foreach (int dificultad in dificultadesOrdenadas)
        {
            var moleculasEnEstaDificultad = porDificultad[dificultad];
            float anchoTotal = (moleculasEnEstaDificultad.Count - 1) * espaciadoHorizontal;
            float posXInicial = -anchoTotal / 2f;

            for (int i = 0; i < moleculasEnEstaDificultad.Count; i++)
            {
                var molecula = moleculasEnEstaDificultad[i];
                CrearBotonMolecula(molecula, posXInicial + (i * espaciadoHorizontal), posY);
            }

            posY -= espaciadoVertical;
        }

        Debug.Log($"✅ Álbum recreado con {botonesInstanciados.Count} botones");
        Debug.Log($"Modo: {(usarPersistencia ? "PERSISTENTE (entre sesiones)" : "NO PERSISTENTE (solo esta sesión)")}");
    }

    void CrearBotonMolecula(PlantillaObjetoMolecula molecula, float posX, float posY)
    {
        GameObject botonObj = Instantiate(prefabBotonMolecula, this.transform);
        botonObj.name = $"Btn_{molecula.nombreMolecula}";
        botonesInstanciados.Add(botonObj);

        RectTransform rectTransform = botonObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(posX, posY);
        }

        BotonAlbumMolecula botonScript = botonObj.GetComponent<BotonAlbumMolecula>();
        if (botonScript != null)
        {
            botonScript.Configurar(molecula, molecula.desbloqueada);
        }

        Image imagenBoton = botonObj.GetComponent<Image>();
        if (imagenBoton != null)
        {
            if (molecula.desbloqueada && molecula.imagenObjetoMolecula != null)
            {
                imagenBoton.sprite = molecula.imagenObjetoMolecula;
                imagenBoton.color = Color.white;
                Debug.Log($"🎨 Imagen especial para {molecula.nombreMolecula}");
            }
        }

        TextMeshProUGUI texto = botonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
        {
            texto.text = molecula.nombreMolecula;
            texto.color = molecula.desbloqueada ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f);
        }

        Button boton = botonObj.GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() => {
                BotonAlbumMolecula script = botonObj.GetComponent<BotonAlbumMolecula>();
                if (script != null)
                {
                    script.OnClick();
                }
            });
        }
    }

    void LimpiarBotones()
    {
        foreach (var boton in botonesInstanciados)
        {
            if (boton != null) Destroy(boton);
        }
        botonesInstanciados.Clear();
    }

    void CargarProgreso()
    {
        // Este es para progreso general, mantenerlo siempre
        string datos = PlayerPrefs.GetString(PLAYER_PREFS_KEY, "");
        if (!string.IsNullOrEmpty(datos))
        {
            string[] nombres = datos.Split(',');
            foreach (string nombre in nombres)
            {
                if (!string.IsNullOrEmpty(nombre))
                    moleculasCompletadas.Add(nombre);
            }
        }
    }

    void GuardarProgreso()
    {
        // Guardar progreso general siempre
        List<string> lista = new List<string>(moleculasCompletadas);
        PlayerPrefs.SetString(PLAYER_PREFS_KEY, string.Join(",", lista));
        PlayerPrefs.Save();
    }

    public void DesbloquearMolecula(string nombre)
    {
        Debug.Log($"🔓 Desbloquear molécula: {nombre}");
        Debug.Log($"Modo: {(usarPersistencia ? "PERSISTENTE" : "NO PERSISTENTE")}");

        foreach (var molecula in todasLasMoleculas)
        {
            if (molecula.nombreMolecula == nombre)
            {
                if (!molecula.desbloqueada)
                {
                    // 1. Desbloquear en memoria (SIEMPRE)
                    molecula.Desbloquear();
                    Debug.Log($"✅ '{nombre}' desbloqueada en MEMORIA");

                    // 2. Guardar en PlayerPrefs solo si la persistencia está ACTIVADA
                    if (usarPersistencia)
                    {
                        GuardarDesbloqueosEnPlayerPrefs();
                        Debug.Log($"💾 '{nombre}' guardada en PlayerPrefs (persistente)");
                    }
                    else
                    {
                        Debug.Log($"📝 '{nombre}' NO se guarda en PlayerPrefs (solo esta sesión)");
                    }
                }
                else
                {
                    Debug.Log($"ℹ️ '{nombre}' ya estaba desbloqueada");
                }

                // 3. Actualizar la vista
                if (this.gameObject.activeInHierarchy)
                {
                    RecrearAlbum();
                }
                return;
            }
        }

        Debug.LogWarning($"⚠️ No se encontró: {nombre}");
    }

    void OnDestroy()
    {
        // Solo guardar desbloqueos si la persistencia está ACTIVADA
        if (usarPersistencia)
        {
            GuardarDesbloqueosEnPlayerPrefs();
            Debug.Log("💾 Desbloqueos guardados en PlayerPrefs (persistente)");
        }
        else
        {
            Debug.Log("📝 Modo no persistente - Desbloqueos NO guardados");
        }
        
        // Progreso general siempre se guarda
        GuardarProgreso();
    }

    [ContextMenu("Debug: Recrear Álbum")]
    public void DebugRecrearAlbum()
    {
        RecrearAlbum();
    }

    [ContextMenu("Debug: Ver estados de moléculas")]
    public void DebugVerEstados()
    {
        Debug.Log($"=== ESTADOS DE MOLÉCULAS ({todasLasMoleculas.Count}) ===");
        Debug.Log($"Persistencia entre sesiones: {(usarPersistencia ? "ACTIVADA" : "DESACTIVADA")}");

        int desbloqueadas = 0;
        foreach (var molecula in todasLasMoleculas)
        {
            if (molecula != null)
            {
                string estado = molecula.desbloqueada ? "🔓 DESBLOQUEADA" : "🔒 BLOQUEADA";
                Debug.Log($"{molecula.nombreMolecula}: {estado}");
                if (molecula.desbloqueada) desbloqueadas++;
            }
        }

        Debug.Log($"Total desbloqueadas: {desbloqueadas}/{todasLasMoleculas.Count}");
        
        // Verificar PlayerPrefs
        string datosPlayerPrefs = PlayerPrefs.GetString(PLAYER_PREFS_DESBLOQUEOS, "VACÍO");
        Debug.Log($"PlayerPrefs actual: {datosPlayerPrefs}");
    }

    [ContextMenu("Debug: Limpiar PlayerPrefs")]
    public void DebugLimpiarPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PLAYER_PREFS_DESBLOQUEOS))
        {
            PlayerPrefs.DeleteKey(PLAYER_PREFS_DESBLOQUEOS);
            PlayerPrefs.Save();
            Debug.Log("🧹 PlayerPrefs de desbloqueos limpiados");
        }

        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            PlayerPrefs.DeleteKey(PLAYER_PREFS_KEY);
            PlayerPrefs.Save();
            Debug.Log("🧹 PlayerPrefs de progreso limpiados");
        }
        
        // También resetear en memoria
        foreach (var molecula in todasLasMoleculas)
        {
            if (molecula != null)
            {
                molecula.desbloqueada = false;
            }
        }
        
        Debug.Log("🔄 Todos los estados reseteados");
    }

    [ContextMenu("Debug: Alternar Persistencia")]
    public void DebugAlternarPersistencia()
    {
        usarPersistencia = !usarPersistencia;
        Debug.Log($"🔄 Persistencia ENTRE SESIONES ahora: {(usarPersistencia ? "ACTIVADA" : "DESACTIVADA")}");
        
        // Si se activa la persistencia, guardar los estados actuales
        if (usarPersistencia)
        {
            GuardarDesbloqueosEnPlayerPrefs();
            Debug.Log("💾 Estados actuales guardados en PlayerPrefs");
        }
    }

    [ContextMenu("Debug: Desbloquear todas")]
    public void DebugDesbloquearTodas()
    {
        foreach (var molecula in todasLasMoleculas)
        {
            if (molecula != null && !molecula.desbloqueada)
            {
                DesbloquearMolecula(molecula.nombreMolecula);
            }
        }
    }
}