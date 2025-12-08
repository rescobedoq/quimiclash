using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scene Names")]
    public string chemistrySceneName = "Inventario";

    private string previousSceneName;
    private bool isSwitching = false;
    public static List<Plantilla_Objeto> InventarioQuimicoTransferido = new List<Plantilla_Objeto>();
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void GuardarInventarioParaTransferencia(List<Plantilla_Objeto> listaDelJugador)
    {
        InventarioQuimicoTransferido.Clear();
        // Creamos una COPIA de la lista del jugador para no modificarla
        InventarioQuimicoTransferido.AddRange(listaDelJugador);
        
        Debug.Log($"GameManager: Inventario Químico guardado para transferencia. Elementos: {InventarioQuimicoTransferido.Count}");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isSwitching)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == chemistrySceneName)
            {
                // Volver a la escena anterior
                StartCoroutine(SwitchSceneRoutine(previousSceneName));
            }
            else
            {
                // Ir a la mesa de alquimia
                previousSceneName = currentScene;
                StartCoroutine(SwitchSceneRoutine(chemistrySceneName));
            }
        }
    }

IEnumerator SwitchSceneRoutine(string sceneName)
    {
        
        isSwitching = true;
        
        // 3. LLAMAR A LA FUNCIÓN DE GUARDADO ANTES DE LA TRANSICIÓN
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != chemistrySceneName)
        {
            // Solo guardamos si venimos de una escena de juego
            PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                // Guarda la lista del jugador antes de que la escena destruya PlayerHealth
                GuardarInventarioParaTransferencia(playerHealth.inventario);
            }
        }
        // ------------------------------------------------
        
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
        yield return null;

        // Manejamos la visibilidad (Jugador y Barra de vida)
        HandleVisibility(sceneName);
        
        // --- NUEVO: Llamar a la actualización después de llegar a la escena de Inventario ---
        if (sceneName == chemistrySceneName)
        {
            // Esperar un frame o dos para que todos los Start/Awake de la nueva escena terminen
            yield return new WaitForEndOfFrame(); 
            
            // Forzar la actualización de la UI de Elementos/Cantidad si existe en la nueva escena
            ForceElementUIUpdate();
        }
        // ---------------------------------------------------------------------------------

        UIFade.instance.FadeFromBlack();
        isSwitching = false;
    }

    // --- NUEVO MÉTODO AUXILIAR ---
    void ForceElementUIUpdate()
    {
        // 1. Forzar a Seleccionados a reconstruir la UI
        Seleccionados seleccionadosManager = FindFirstObjectByType<Seleccionados>();
        if (seleccionadosManager != null)
        {
             // Asumo que ReconstruirUIDesdePlantillas llama a la lógica de pintado
             seleccionadosManager.ReconstruirUIDesdePlantillas(); 
        }

        // 2. Forzar al MostradorCantidad a actualizarse (aunque esto podría ser redundante si la reconstrucción lo hace)
        MostradorCantidad mostrador = FindFirstObjectByType<MostradorCantidad>();
        if (mostrador != null)
        {
            // Necesitamos la Plantilla_Objeto actualmente seleccionada para actualizar.
            // Dado que no la tenemos aquí, confiaremos en que ReconstruirUIDesdePlantillas 
            // y los botones de la UI manejen el estado inicial.
            Debug.Log("GameManager: Intentando forzar actualización de MostradorCantidad (si ya hay elemento seleccionado)");
        }
    }

    void HandleVisibility(string sceneName)
    {
        // Verificar si estamos en la escena de la mesa
        bool isChemistryScene = (sceneName == chemistrySceneName);

        // 1. CONTROL DEL JUGADOR
        if (PlayerController2D.instance != null)
        {
            // Si es la mesa, false. Si es el juego, true.
            PlayerController2D.instance.gameObject.SetActive(!isChemistryScene);

            if (!isChemistryScene) // Solo si volvemos al juego
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    var camController = cam.GetComponent<CameraController>();
                    if (camController != null) camController.target = PlayerController2D.instance.transform;
                }
            }
        }

        // 2. CONTROL DE LA BARRA DE VIDA (HealthBar)
        if (HealthBar.instance != null)
        {
            // Si es la mesa, ocultamos la barra. Si no, la mostramos.
            HealthBar.instance.gameObject.SetActive(!isChemistryScene);
        }
    }
    
}