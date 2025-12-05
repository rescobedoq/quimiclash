using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject lootPrefab; // El prefab genérico (la bolsita o el item en el suelo)
    [Range(0, 100)] public float dropChance = 50f; // Probabilidad de soltar algo (50%)

    public void DropRandomLoot()
    {
        // 1. Calculamos si suelta algo o no
        float randomCalc = Random.Range(0f, 100f);
        if (randomCalc > dropChance) 
        {
            // Mala suerte, no suelta nada
            return; 
        }

        // 2. Cargamos TODOS los items de la carpeta Resources/ItemsLoot
        Item[] allItems = Resources.LoadAll<Item>("ItemsLoot");

        // Seguridad: Si la carpeta está vacía, no hacemos nada
        if (allItems.Length == 0)
        {
            Debug.LogWarning("No se encontraron Items en la carpeta Resources/ItemsLoot");
            return;
        }

        // 3. Elegimos uno al azar
        int randomIndex = Random.Range(0, allItems.Length);
        Item randomItem = allItems[randomIndex];

        // 4. Instanciamos el objeto físico en el suelo
        SpawnItem(randomItem);
    }

    void SpawnItem(Item itemToDrop)
    {
        // Creamos el objeto en la posición del enemigo
        GameObject drop = Instantiate(lootPrefab, transform.position, Quaternion.identity);

        // --- CONFIGURACIÓN DINÁMICA ---
        
        // 1. Asignamos los datos al script ItemPickup
        ItemPickup pickupScript = drop.GetComponent<ItemPickup>();
        if (pickupScript != null)
        {
            pickupScript.item = itemToDrop;
            pickupScript.amount = 1; // Por defecto 1, podrías randomizarlo también
        }

        // 2. Cambiamos el Sprite visualmente para que coincida con el item
        SpriteRenderer sr = drop.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = itemToDrop.icon;
        }
        
        // (Opcional) Cambiar el nombre en la jerarquía para depurar mejor
        drop.name = "Loot_" + itemToDrop.itemName;
    }
}