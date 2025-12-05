using UnityEngine;
using UnityEngine.UI; // <--- NECESARIO PARA USAR SLIDER

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Reference")]
    public Slider healthSlider; // Arrastra aquí el "HealthSlider" en el inspector
    public GameObject healthBarCanvas; // Opcional: para ocultar la barra si muere

    private void Start()
    {
        currentHealth = maxHealth;

        // Configuración inicial del slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;

            // Opcional: Ocultar la barra si la vida está llena para no saturar la pantalla
            //healthBarCanvas.SetActive(false); 
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Limitar la vida
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Actualizar visualmente la barra
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            // Opcional: Mostrar la barra solo cuando recibe daño
            //if (currentHealth < maxHealth) healthBarCanvas.SetActive(true);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Aquí puedes poner animación de muerte, drops, partículas, etc.
        Destroy(gameObject);
    }
}