using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scene Names")]
    public string chemistrySceneName = "Quemistry Table";

    private string previousSceneName;
    private bool isSwitching = false;

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
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
        yield return null;

        // Manejamos la visibilidad (Jugador y Barra de vida)
        HandleVisibility(sceneName);

        UIFade.instance.FadeFromBlack();
        isSwitching = false;
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