using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private PlayerHealth playerHealth;

    void Start()
    {
        // Obtiene el componente Slider del objeto actual
        slider = GetComponent<Slider>();

        // Busca al jugador con el tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró un objeto con tag 'Player'.");
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
