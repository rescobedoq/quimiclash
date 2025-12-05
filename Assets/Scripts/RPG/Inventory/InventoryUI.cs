using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    Inventory inventory;
    InventorySlotUI[] slots;

    private void Start()
    {
        inventory = Inventory.instance;
        if (inventory == null) Debug.LogError("ERROR: No se encontró Inventory.instance");

        inventory.onInventoryChangedCallback += UpdateUI;

        // Verificamos si asignaste el itemsParent
        if (itemsParent == null) 
        {
            Debug.LogError("ERROR: 'itemsParent' no está asignado en el Inspector.");
            return;
        }

        slots = itemsParent.GetComponentsInChildren<InventorySlotUI>();
        
        // Verificamos si asignaste el panel
        if (inventoryUI == null)
        {
             Debug.LogError("ERROR: 'inventoryUI' no está asignado en el Inspector.");
             return;
        }

        inventoryUI.SetActive(false);
        Debug.Log("InventoryUI iniciado correctamente. Inventario oculto.");
        
        UpdateUI(); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Tecla 'I' detectada.");
            
            if (inventoryUI != null)
            {
                inventoryUI.SetActive(!inventoryUI.activeSelf);
                Debug.Log("Estado del inventario cambiado a: " + inventoryUI.activeSelf);
            }
        }
    }

    void UpdateUI()
    {
        Debug.Log("PUENTE: Se ha llamado a UpdateUI."); // 1. ¿Llega la señal?

        if (inventory == null)
        {
            Debug.LogError("PUENTE ERROR: ¡La UI no encuentra el Inventario (referencia nula)!");
            return;
        }

        Debug.Log("PUENTE: El inventario tiene " + inventory.items.Count + " objetos."); // 2. ¿Hay datos?
        Debug.Log("PUENTE: La UI tiene " + slots.Length + " casillas (slots)."); // 3. ¿Hay botones?

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                Debug.Log("PUENTE: Pintando slot " + i + " con " + inventory.items[i].item.itemName);
                slots[i].AddItem(inventory.items[i].item, inventory.items[i].amount);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}