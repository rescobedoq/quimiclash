using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public string areaTransitionName;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isKnockedback = false;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    void Awake()
    {
        // Singleton (mantener jugador entre escenas)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Asignar cámara al jugador
        if (Camera.main != null)
        {
            var cam = Camera.main.GetComponent<CameraController>();
            if (cam != null)
                cam.target = transform;
        }
    }

    public void SetBounds(Vector3 bottomLeft, Vector3 topRight)
    {
        bottomLeftLimit = bottomLeft + new Vector3(1f, 1f, 0f);
        topRightLimit = topRight + new Vector3(-1f, -1f, 0f);
    }

    public Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, bottomLeftLimit.x + 0.5f, topRightLimit.x - 0.5f),
            Mathf.Clamp(position.y, bottomLeftLimit.y + 0.5f, topRightLimit.y - 0.5f),
            position.z
        );
    }
}
