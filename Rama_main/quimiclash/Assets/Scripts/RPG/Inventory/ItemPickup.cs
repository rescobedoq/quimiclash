using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Configuración del Item")]
    public Item item;       // Aquí arrastraremos el ScriptableObject (ej: PocionSalud)
    public int amount = 1;  // Cuántos damos (ej: 1 poción, o 5 monedas)
    
    [Header("Referencias Visuales")]
    public GameObject pickupUI; // El texto flotante "Presiona E"

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Al inicio, ocultamos el texto de "Presiona E"
        if(pickupUI != null)
        {
            pickupUI.SetActive(false);
        }

        // AUTO-CONFIGURACIÓN VISUAL (Opcional pero útil):
        // Si el objeto tiene un SpriteRenderer, le ponemos el icono del Item automáticamente.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && item != null)
        {
            sr.sprite = item.icon;
        }
    }

    private void Update()
    {
        // Solo si el jugador está cerca y presiona E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        // 1. Intentamos añadirlo al inventario usando el Singleton que creamos en la Tarea 1
        bool wasPickedUp = Inventory.instance.Add(item, amount);

        // 2. Si se pudo recoger (había espacio)...
        if (wasPickedUp)
        {
            Debug.Log("Recogido: " + item.itemName);
            
            // Destruimos el objeto del suelo
            Destroy(gameObject);
        }
    }

    // --- Detección de colisiones ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if(pickupUI != null) pickupUI.SetActive(true); // Mostrar texto
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if(pickupUI != null) pickupUI.SetActive(false); // Ocultar texto
        }
    }
}