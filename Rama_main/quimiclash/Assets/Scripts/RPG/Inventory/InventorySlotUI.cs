using UnityEngine;
using UnityEngine.UI; // Necesario para imágenes y botones
using TMPro;          // Necesario para el texto

public class InventorySlotUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image icon;              // La imagen del objeto
    public TextMeshProUGUI amountText; // El texto de cantidad (x5)
    public Button removeButton;     // El botoncito "X" para tirar el objeto

    Item item;  // El objeto que guardamos en esta casilla

    // Este método lo llamará el inventario para "pintar" la casilla
    public void AddItem(Item newItem, int amount)
    {
        Debug.Log("DEBUG: Intentando pintar item: " + newItem.itemName); // <--- NUEVO

        item = newItem;
        icon.sprite = item.icon;
        
        if (icon.sprite == null) 
            Debug.LogError("DEBUG: ¡El item no tiene sprite o no se asignó!"); // <--- NUEVO
        else
            Debug.Log("DEBUG: Sprite asignado correctamente."); // <--- NUEVO

        icon.enabled = true;

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
    // Limpia la casilla si no hay objeto
    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        amountText.enabled = false;
        removeButton.interactable = false;
    }

    // --- Botones ---

    // Se ejecutará al hacer clic en la X
    public void OnRemoveButton()
    {
        if (item != null)
        {
            Inventory.instance.Remove(item);
        }
    }

    // Se ejecutará al hacer clic en el icono grande (Para usar el objeto)
    public void OnUseButton()
    {
        if (item != null)
        {
            // Llamamos al intermediario para que aplique el efecto real
            PlayerStatsIntegration.instance.UseItemEffect(item);
        }
    }
}