using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    [Header("References")]
    public PlayerCombat playerCombat;

    private PlayerController2D controller;
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;

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
    }

    void Update()
    {
        if (controller.isKnockedback) return;

        HandleMovement();
        HandleAttack();
    }

    private void HandleMovement()
    {
        if (!controller.canMove)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Prioridad horizontal
        if (moveX != 0) moveY = 0;

        Vector2 movementInput = new Vector2(moveX, moveY).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && movementInput.sqrMagnitude > 0;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        rb.linearVelocity = movementInput * currentSpeed;

        // Gestión del Flip
        if ((moveX > 0 && !facingRight) || (moveX < 0 && facingRight))
        {
            Flip();
        }

        // --- ANIMACIONES ---
        bool isMoving = movementInput.sqrMagnitude > 0;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);

        if (isMoving)
        {
            animator.SetFloat("lastMoveX", moveX);
            animator.SetFloat("lastMoveY", moveY);

            // Usamos Mathf.Abs porque la dirección visual se maneja con Flip (Scale),
            // así el Blend Tree siempre recibe valores positivos para animaciones laterales.
            animator.SetFloat("moveX", Mathf.Abs(moveX));
            animator.SetFloat("moveY", moveY);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        transform.position = controller.ClampPosition(transform.position);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Slice") && playerCombat != null)
        {
            playerCombat.Attack();
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