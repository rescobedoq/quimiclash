using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    // --- Singleton: Para acceder desde cualquier lado con "Inventory.instance" ---
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("¡Cuidado! Hay más de un inventario en la escena.");
            return;
        }
        instance = this;
    }
    // -----------------------------------------------------------------------------

    // Evento: Avisará a la UI cuando algo cambie para que se actualice sola
    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    [Header("Configuración")]
    public int space = 20;  // Cantidad máxima de casillas
    
    // Nuestra lista real de objetos. Usamos una clase auxiliar 'InventorySlot' para manejar cantidades
    public List<InventorySlot> items = new List<InventorySlot>();

    // Método para añadir un objeto
    public bool Add(Item item, int amount = 1)
    {
        // 1. Si el objeto es apilable, buscamos si ya tenemos uno igual
        if (item.isStackable)
        {
            foreach (InventorySlot slot in items)
            {
                if (slot.item == item)
                {
                    slot.amount += amount;
                    // Avisamos que hubo cambios
                    if (onInventoryChangedCallback != null) onInventoryChangedCallback.Invoke();
                    return true;
                }
            }
        }

        // 2. Si no es apilable o no lo tenemos, verificamos espacio
        if (items.Count >= space)
        {
            Debug.Log("Inventario lleno. No se puede recoger.");
            return false;
        }

        // 3. Añadimos el nuevo objeto a la lista
        items.Add(new InventorySlot(item, amount));
        
        // Avisamos que hubo cambios
        if (onInventoryChangedCallback != null) onInventoryChangedCallback.Invoke();
        
        return true;
    }

    // Método para eliminar/remover un objeto
    public void Remove(Item item, int amountToRemove = 1)
    {
        InventorySlot slotToRemove = null;

        // Buscamos el objeto
        foreach (InventorySlot slot in items)
        {
            if (slot.item == item)
            {
                slot.amount -= amountToRemove;
                
                // Si la cantidad llega a 0, marcamos para borrar la casilla completa
                if (slot.amount <= 0)
                {
                    slotToRemove = slot;
                }
                break;
            }
        }

        // Borramos la casilla si quedó vacía
        if (slotToRemove != null)
        {
            items.Remove(slotToRemove);
        }

        // Avisamos a la UI
        if (onInventoryChangedCallback != null) onInventoryChangedCallback.Invoke();
    }
}

// Clase auxiliar simple para guardar el Item y cuántos tenemos
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int amount;

    public InventorySlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
}