using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    // Usamos esta constante para establecer la escala, 
    // asumiendo que 100 Píxeles por Unidad (PPU) en la configuración del Sprite.
    // 16 / 100 = 0.16 (La escala necesaria para que se vea de 16x16 píxeles)
    private const float TARGET_SCALE = 0.16f;

    [Header("Configuración General")]
    public GameObject lootPrefab; 
    [Range(0, 100)] public float dropChance = 50f;

    [Header("Rutas de Carpetas (Dentro de Resources)")]
    public string pathItems = "ItemsLoot";            
    public string pathElementos = "ElementosQuimicos"; 

    [Header("Probabilidad de Tipo")]
    [Tooltip("Si decide soltar algo, ¿qué probabilidad hay de que sea un Elemento Químico en lugar de un Item normal?")]
    [Range(0, 100)] public float chanceOfElement = 50f; 

    public void DropRandomLoot()
    {
        // ... (La lógica de probabilidad es la misma)

        // 1. Calculamos si suelta algo en general
        if (Random.Range(0f, 100f) > dropChance) return;

        // 2. Decidimos QUÉ tipo de objeto soltar (Item vs Elemento)
        bool dropIsElement = Random.Range(0f, 100f) <= chanceOfElement;

        if (dropIsElement)
        {
            // --- LOGICA PARA ELEMENTOS DE LA TABLA (Plantilla_Objeto) ---
            Plantilla_Objeto[] allElementos = Resources.LoadAll<Plantilla_Objeto>(pathElementos);

            if (allElementos.Length > 0)
            {
                int randomIndex = Random.Range(0, allElementos.Length);
                SpawnElemento(allElementos[randomIndex]);
            }
            else
            {
                Debug.LogWarning($"La carpeta Resources/{pathElementos} está vacía o no existe.");
            }
        }
        else
        {
            // --- LOGICA PARA ITEMS NORMALES (Item) ---
            Item[] allItems = Resources.LoadAll<Item>(pathItems);

            if (allItems.Length > 0)
            {
                int randomIndex = Random.Range(0, allItems.Length);
                SpawnItem(allItems[randomIndex]);
            }
            else
            {
                Debug.LogWarning($"La carpeta Resources/{pathItems} está vacía.");
            }
        }
    }

    // Función para soltar Items normales
    void SpawnItem(Item itemToDrop)
    {
        GameObject drop = Instantiate(lootPrefab, transform.position, Quaternion.identity);
        
        // --- MODIFICACIÓN CLAVE: Establecer la escala para forzar el tamaño ---
        drop.transform.localScale = new Vector3(TARGET_SCALE, TARGET_SCALE, 1f); 
        // ---------------------------------------------------------------------

        // Asignar datos al script de recolección
        ItemPickup pickupScript = drop.GetComponent<ItemPickup>();
        if (pickupScript != null)
        {
            pickupScript.item = itemToDrop;
            pickupScript.elementoQuimico = null; 
            pickupScript.amount = 1; 
        }

        // Cambiar Sprite (Usa 'icon' porque es un Item)
        SpriteRenderer sr = drop.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = itemToDrop.icon;
        
        drop.name = "Loot_Item_" + itemToDrop.itemName;
    }

    // Función NUEVA para soltar Elementos (Plantilla_Objeto)
    void SpawnElemento(Plantilla_Objeto elementoToDrop)
    {
        GameObject drop = Instantiate(lootPrefab, transform.position, Quaternion.identity);

        // --- MODIFICACIÓN CLAVE: Establecer la escala para forzar el tamaño ---
        drop.transform.localScale = new Vector3(TARGET_SCALE, TARGET_SCALE, 1f); 
        // ---------------------------------------------------------------------

        // Asignar datos al script de recolección
        ItemPickup pickupScript = drop.GetComponent<ItemPickup>();
        if (pickupScript != null)
        {
            pickupScript.elementoQuimico = elementoToDrop; 
            pickupScript.item = null; 
            pickupScript.amount = 1; 
        }

        // Cambiar Sprite (Usa 'imagenObjeto' porque es Plantilla_Objeto)
        SpriteRenderer sr = drop.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = elementoToDrop.imagenObjeto;

        drop.name = "Loot_Elemento_" + elementoToDrop.nombreObjeto;
    }
}