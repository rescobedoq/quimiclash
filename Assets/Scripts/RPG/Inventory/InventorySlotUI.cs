using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image icon;
    public TextMeshProUGUI amountText;
    public Button removeButton;

    // YA NO ES SOLO ITEM. Ahora guardamos las dos referencias que vienen del InventorySlot.
    Item item; 
    Plantilla_Objeto elementoQuimico; // <--- AGREGADO

    // Este método lo llamará el inventario para "pintar" la casilla
    // Ahora recibe el objeto completo del slot.
    public void AddSlotData(InventorySlot slotData) // <--- CAMBIO DE NOMBRE DEL MÉTODO Y PARÁMETRO
    {
        // 1. Asignamos ambas referencias del Slot
        item = slotData.item;
        elementoQuimico = slotData.elementoQuimico;
        int amount = slotData.amount;
        
        // 2. Determinar qué pintar y qué debuggear
        Sprite itemSprite = null;
        string debugName = "Slot Vacío";

        if (item != null)
        {
            itemSprite = item.icon;
            debugName = item.itemName;
        }
        else if (elementoQuimico != null)
        {
            itemSprite = elementoQuimico.imagenObjeto; // <--- Usa la imagen del Plantilla_Objeto
            debugName = elementoQuimico.nombreObjeto;
        }
        
        Debug.Log("DEBUG: Intentando pintar slot: " + debugName);

        // 3. Configuración visual
        icon.sprite = itemSprite;
        
        if (icon.sprite == null && (item != null || elementoQuimico != null)) 
            Debug.LogError("DEBUG: ¡El objeto " + debugName + " no tiene sprite o no se asignó!");
        
        icon.enabled = true;
        icon.preserveAspect = true; // Mantener el ajuste de 16x16 que hicimos antes

        // 4. Configuración de cantidad
        if (amount > 1)
        {
            amountText.text = amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            amountText.enabled = false;
        }
        removeButton.interactable = true;
    }

    // Limpia la casilla (Método ClearSlot es el mismo)
    public void ClearSlot()
    {
        item = null;
        elementoQuimico = null; // <--- LIMPIAR TAMBIÉN ESTO

        icon.sprite = null;
        icon.enabled = false;
        amountText.enabled = false;
        removeButton.interactable = false;
    }

    // El método OnRemoveButton necesita ser adaptado para ambos si usas Remove(Item).
    // Si la lógica de 'Remove' es compleja para elementos químicos, necesitarás un 'RemoveElement'.
    public void OnRemoveButton()
    {
        if (item != null)
        {
            Inventory.instance.Remove(item);
        }
        else if (elementoQuimico != null)
        {
            // Deberías crear un método RemoveElement(Plantilla_Objeto) en Inventory.cs
            // Por ahora, solo debuggearemos:
            Debug.LogWarning("Funcionalidad de remover Elemento Químico no implementada.");
        }
    }
    
    // El método OnUseButton también necesita ser adaptado.
    public void OnUseButton()
    {
        if (item != null)
        {
            PlayerStatsIntegration.instance.UseItemEffect(item);
        }
        else if (elementoQuimico != null)
        {
             // Deberías crear un método UseElementEffect(Plantilla_Objeto) en PlayerStatsIntegration.cs
             Debug.LogWarning("Funcionalidad de usar Elemento Químico no implementada. Objeto: " + elementoQuimico.nombreObjeto);
        }
    }
}