using UnityEngine;

public class PlayerStatsIntegration : MonoBehaviour
{
    public static PlayerStatsIntegration instance;
    public PlayerHealth playerStats; // Referencia a tu script de vida

    private void Awake()
    {
        instance = this;
    }

    public void UseItemEffect(Item item)
    {
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.Consumable:
                playerStats.ChangeHealth(item.value);
                Inventory.instance.Remove(item);
                break;

            case ItemType.Weapon:
                // Reemplaza el poder del arma actual
                playerStats.weaponPower = item.value;
                playerStats.equippedWeapon = item.itemName;
                Debug.Log($"⚔️ Arma equipada: {item.itemName} (+{item.value} Ataque)");
                break;

            case ItemType.Armor:
                // Reemplaza la defensa de la armadura actual
                playerStats.armorPower = item.value;
                playerStats.equippedArmor = item.itemName;
                Debug.Log($"🛡️ Armadura equipada: {item.itemName} (+{item.value} Defensa)");
                break;
        }
    }
}