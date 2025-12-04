using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private PlayerController2D controller;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("References")]
    public PlayerCombat playerCombat;
    public SpriteRenderer spriteRenderer; // asigna en inspector si está en un hijo
    public bool CanMove

    {
        get { return controller.canMove; }
        set { controller.canMove = value; }
    }
    void Start()
    {
        controller = PlayerController2D.instance;
        rb = controller.rb;
        animator = controller.animator;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!controller.isKnockedback)
            HandleMovement();

        if (Input.GetButtonDown("Slice"))
        {
            HandleAttackDirection();
            float lastX = animator.GetFloat("lastMoveX");
            playerCombat.Attack();
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // prioridad horizontal
        if (moveX != 0) moveY = 0;

        if (controller.canMove)
        {
            Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;
            rb.linearVelocity = movement;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Animaciones (estos parámetros los usa el Animator que tiene solo animaciones "derecha")
        animator.SetFloat("moveX", rb.linearVelocity.x);
        animator.SetFloat("moveY", rb.linearVelocity.y);

        if (moveX != 0 || moveY != 0)
        {
            animator.SetFloat("lastMoveX", moveX);
            animator.SetFloat("lastMoveY", moveY);
            // mientras te mueves, orienta visualmente según moveX
            UpdateSpriteFlipDuringMovement(moveX);
        }
        else
        {
            // estás idle -> orienta según lastMoveX guardado en el animator
            float lastMoveX = animator.GetFloat("lastMoveX");
            UpdateSpriteFlipIdle(lastMoveX);
        }

        transform.position = controller.ClampPosition(transform.position);
    }

    private void UpdateSpriteFlipDuringMovement(float moveX)
    {
        if (spriteRenderer == null) return;

        if (moveX > 0)
            spriteRenderer.flipX = false; // mirar derecha (animaciones son right)
        else if (moveX < 0)
            spriteRenderer.flipX = false;  // mirar izquierda (flip visual)
    }

    private void UpdateSpriteFlipIdle(float lastMoveX)
    {
        if (spriteRenderer == null) return;

        if (lastMoveX > 0)
            spriteRenderer.flipX = false;
        else if (lastMoveX < 0)
            spriteRenderer.flipX = false;
        // si lastMoveX == 0 no cambiamos
    }

    private void HandleAttackDirection()
    {
        float lastMoveX = animator.GetFloat("lastMoveX");

        // Usa las variables lastX / lastY en lugar del animator
        if (spriteRenderer != null)
        {
            if (lastMoveX > 0)
                spriteRenderer.flipX = false;
            else if (lastMoveX < 0)
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }


    public void Knockback(Transform enemy, float force, float stunTime)
    {
        controller.isKnockedback = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    private IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        controller.isKnockedback = false;
    }
}
