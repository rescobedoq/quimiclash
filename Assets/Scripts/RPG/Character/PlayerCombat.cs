using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 50; 
    public LayerMask enemyLayers;
    public int attackDamage = 10;
    public float attackStunTime = 0.5f;

    public Animator anim;

    public float cooldownBetweenAttacks = 1f;
    private float timer;

    private PlayerMovement playerMovement;
    private PlayerHealth playerStats; // <--- 1. NUEVA REFERENCIA

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerHealth>(); // <--- 2. OBTENER COMPONENTE
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }   
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            timer = cooldownBetweenAttacks;

            if (playerMovement != null)
                StartCoroutine(FreezeMovementDuringAttack());
        }
    }

    public void dealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayers);

        // <--- 3. CALCULO DEL DAÑO TOTAL --->
        int totalDamage = attackDamage;
        
        if (playerStats != null)
        {
            // Sumamos daño base + fuerza del personaje + poder del arma equipada
            totalDamage += playerStats.strength + playerStats.weaponPower;
        }
        
        Debug.Log("Golpeando con daño TOTAL: " + totalDamage);
        // <--------------------------------->

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            EnemyKnockback enemyKnockback = enemy.GetComponent<EnemyKnockback>();

            if (enemyHealth != null)
            {
                // USAMOS totalDamage EN LUGAR DE attackDamage
                enemyHealth.ChangeHealth(-totalDamage);
            }
            else
            {
                Debug.LogWarning($"El enemigo {enemy.name} no tiene componente EnemyHealth");
            }

            if (enemyKnockback != null)
            {
                enemyKnockback.Knockback(transform, knockbackForce, attackStunTime);
            }
            else
            {
                Debug.LogWarning($"El enemigo {enemy.name} no tiene componente EnemyKnockback");
            }
        }
    }

    private IEnumerator FreezeMovementDuringAttack()
    {
        if (playerMovement == null) yield break;

        playerMovement.CanMove = false;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        playerMovement.CanMove = true;
        anim.SetBool("isAttacking", false);
    }

    public void StopAttack()
    {
        anim.SetBool("isAttacking", false); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }   
}