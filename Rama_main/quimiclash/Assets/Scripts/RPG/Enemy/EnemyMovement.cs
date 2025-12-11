using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f; // tiempo entre ataques

    private EnemyState enemyState;
    private int facingDirection = 1;
    private Animator anim;
    private Transform player;
    private float lastAttackTime; // tiempo del último ataque

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        changeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState != EnemyState.Knockback)
        {
            if (player == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // --- Cambiar de estado según la distancia ---
            if (distanceToPlayer <= attackRange)
            {
                changeState(EnemyState.Attacking);
            }
            else if (distanceToPlayer <= detectionRange)
            {
                changeState(EnemyState.Chasing);
            }
            else
            {
                changeState(EnemyState.Idle);
            }

            // --- Comportamientos según estado ---
            if (enemyState == EnemyState.Idle)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                rb.linearVelocity = Vector2.zero;
                Attack();
            }
        }
    }

    void Chase()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // Girar hacia el jugador
        if (player.position.x > transform.position.x && facingDirection == -1)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        rb.linearVelocity = direction * speed;
    }

    void Attack()
    {
        // Solo atacar si ha pasado suficiente tiempo desde el último ataque
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("attack"); // ejecuta animación de ataque
            Debug.Log("Enemy atacando!");

            // Aquí podrías agregar daño real al jugador:
            // player.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        //transform.localScale = new Vector3(facingDirection * , 1, 1);
        transform.localScale = new Vector3(transform.localScale.x * -1 , transform.localScale.y, transform.localScale.z);
    }

    public void changeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        // Desactivar animaciones anteriores
        anim.SetBool("isIdle", false);
        anim.SetBool("isChasing", false);
        anim.SetBool("isAttacking", false);

        // Cambiar estado actual
        enemyState = newState;

        // Activar animación del nuevo estado
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);

        Debug.Log("Estado cambiado a: " + enemyState);
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}
