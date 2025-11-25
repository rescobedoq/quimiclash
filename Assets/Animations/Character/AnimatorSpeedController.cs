using UnityEngine;

public class AnimatorSpeedController : MonoBehaviour
{
    [Range(0f, 3f)] // te permite deslizar la velocidad desde el inspector
    public float animationSpeed = 1f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
    }
}
