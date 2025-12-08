// Modifica tu Item.cs para incluir soporte químico
using UnityEngine;

public enum ItemType 
{ 
    Consumable, // Pociones, comida
    Weapon,     // Espadas, arcos
    Armor,      // Escudos, cascos
    Material,   // Para crafting futuro
    Chemical    // NUEVO: Elementos químicos
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Información Básica")]
    public string itemName = "Nuevo Objeto";
    public Sprite icon = null;              // El dibujo que se verá en el inventario
    [TextArea] public string description = "Descripción del objeto";
    
    [Header("Propiedades")]
    public ItemType itemType;
    public bool isStackable = false;        // ¿Se pueden juntar varios en una casilla?

    [Header("Estadísticas / Valores")]
    public int value; 

    // NUEVO: Referencia al Plantilla_Objeto si es un elemento químico
    public Plantilla_Objeto chemicalElement;

    // NUEVO: Método para saber si es un elemento químico
    public bool IsChemical()
    {
        return itemType == ItemType.Chemical || chemicalElement != null;
    }

    // Método virtual: Esto nos permitirá programar efectos únicos más adelante si queremos
    public virtual void Use()
    {
        Debug.Log("Usando objeto: " + itemName);
        
        // Si es químico, lo manejamos diferente
        if (IsChemical())
        {
            Debug.Log($"Usando elemento químico: {itemName}");
            // Podrías añadir lógica específica aquí
        }
    }
}