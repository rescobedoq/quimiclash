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

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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

            // Congela el movimiento mientras dura la animación
            if (playerMovement != null)
                StartCoroutine(FreezeMovementDuringAttack());
        }
    }
    public void dealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            EnemyKnockback enemyKnockback = enemy.GetComponent<EnemyKnockback>();

            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-attackDamage);
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

        // Desactiva movimiento
        playerMovement.CanMove = false;

        // Espera hasta que termine la animación
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // 🔓 Reactiva movimiento
        playerMovement.CanMove = true;
        anim.SetBool("isAttacking", false);
    }


    public void StopAttack()
    {
        anim.SetBool("isAttacking", false); 
        if (transform.localScale.x < 0) { 
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); 
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }   
}
