using UnityEngine;

// Definimos los tipos de objetos posibles
public enum ItemType 
{ 
    Consumable, // Pociones, comida
    Weapon,     // Espadas, arcos
    Armor,      // Escudos, cascos
    Material    // Para crafting futuro
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
    // Este valor será genérico: Daño si es arma, Curación si es poción, Defensa si es armadura
    public int value; 

    // Método virtual: Esto nos permitirá programar efectos únicos más adelante si queremos
    public virtual void Use()
    {
        Debug.Log("Usando objeto: " + itemName);
    }
}