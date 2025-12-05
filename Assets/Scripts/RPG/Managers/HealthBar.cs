using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar instance; // <--- AGREGADO

    private Slider slider;
    private PlayerHealth playerHealth;

    void Awake() // <--- AGREGADO
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        slider = GetComponent<Slider>();

        // Busca al jugador con el tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.currentHealth;
        }
    }
}