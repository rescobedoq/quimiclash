using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public string characterName = "Player";

    public int characterLevel = 1;
    public int currentExperience;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseExp = 1000;

    public int currentHealth;
    public int maxHealth = 100;
    public int currentMana;
    public int maxMana = 50;
    public int[] manaLevelBonus;

    public int strength;
    public int defense;
    public int weaponPower;
    public int armorPower;

    public string equippedWeapon;
    public string equippedArmor;
    public Sprite characterImage;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        expToNextLevel = new int[maxLevel];
        expToNextLevel[1] = baseExp;

        for (int i = 2; i < expToNextLevel.Length; i++)
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i - 1] * 1.05f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            AddExperience(1000);
    }

    public void AddExperience(int expToAdd)
    {
        currentExperience += expToAdd;

        if (characterLevel < maxLevel)
        {
            if (currentExperience > expToNextLevel[characterLevel])
            {
                currentExperience -= expToNextLevel[characterLevel];
                characterLevel++;

                if (characterLevel % 2 == 0) strength++;
                else defense++;

                maxHealth = Mathf.FloorToInt(maxHealth * 1.05f);
                currentHealth = maxHealth;

                if (manaLevelBonus != null && characterLevel < manaLevelBonus.Length)
                    maxMana += manaLevelBonus[characterLevel];

                currentMana = maxMana;
            }
        }

        if (characterLevel >= maxLevel)
            currentExperience = 0;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameObject.SetActive(false);
            Debug.Log("El jugador ha sido derrotado.");
        }

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
