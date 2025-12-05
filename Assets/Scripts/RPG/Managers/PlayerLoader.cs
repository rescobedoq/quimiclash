using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    public GameObject playerPrefab;

    void Awake()
    {
        // Ejecutar ANTES que los Start() de otros scripts
        if (PlayerController2D.instance == null)
        {
            GameObject newPlayer = Instantiate(
                playerPrefab,
                transform.position,
                Quaternion.identity
            );

            PlayerController2D.instance = newPlayer.GetComponent<PlayerController2D>();
        }
        else
        {
            // Si ya existe (por DontDestroyOnLoad), solo reubícalo
            PlayerController2D.instance.transform.position = transform.position;
        }
    }
}
