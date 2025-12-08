using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Configuración del Item")]
    public Item item;              // Referencia al Item normal (será NULL si es elemento químico)
    public Plantilla_Objeto elementoQuimico; // Referencia al objeto químico (será NULL si es Item normal)
    public int amount = 1;         // Cantidad a recoger
    
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

        // --- LÓGICA DE AUTO-CONFIGURACIÓN VISUAL MEJORADA ---
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (item != null)
            {
                // Es un Item normal: usamos su 'icon'
                sr.sprite = item.icon;
            }
            else if (elementoQuimico != null)
            {
                // Es un Elemento Químico: usamos su 'imagenObjeto'
                sr.sprite = elementoQuimico.imagenObjeto;
            }
            // NOTA: Si ambos fueran NULL, el objeto no tiene datos y el sprite queda como estaba.
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
        bool wasPickedUp = false;
        string debugName = "Objeto desconocido";

        if (item != null)
        {
            // --- CASO 1: Recoger un ITEM normal ---
            wasPickedUp = Inventory.instance.Add(item, amount);
            debugName = item.itemName;
        }
        else if (elementoQuimico != null)
        {
            // --- CASO 2: Recoger un ELEMENTO QUÍMICO ---
            // Usamos el nuevo método AddElement que creamos en Inventory.cs
            wasPickedUp = Inventory.instance.AddElement(elementoQuimico, amount); 
            debugName = elementoQuimico.nombreObjeto;
        }
        else
        {
            Debug.LogWarning("El objeto de loot no tiene ningún dato asignado (Item ni Elemento Químico).");
            return;
        }

        // 2. Si se pudo recoger (había espacio)...
        if (wasPickedUp)
        {
            Debug.Log("Recogido exitosamente: " + debugName);
            
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