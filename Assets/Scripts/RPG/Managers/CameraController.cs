using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform target; // Jugador
    public Tilemap Tilemap;  // Tilemap actual

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        TryFindPlayer();

        if (Camera.main != null)
        {
            halfHeight = Camera.main.orthographicSize;
            halfWidth = halfHeight * Camera.main.aspect;
        }

        if (Tilemap != null)
        {
            SetLimitsFromTilemap();
        }
    }

    void LateUpdate()
    {
        // Si el jugador se destruyó (al cambiar de escena), intenta volver a asignarlo
        if (target == null)
        {
            TryFindPlayer();
            if (target == null) return; // si aún no existe, salimos
        }

        // Seguir al jugador
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Limitar el movimiento de la cámara al área del Tilemap
        if (Tilemap != null)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, bottomLeftLimit.x + 0.5f, topRightLimit.x - 0.5f),
                Mathf.Clamp(transform.position.y, bottomLeftLimit.y + 0.5f, topRightLimit.y - 0.5f),
                transform.position.z
            );
        }
    }

    private void TryFindPlayer()
    {
        if (PlayerController2D.instance != null)
        {
            target = PlayerController2D.instance.transform;
        }
        else
        {
            var playerObj = Object.FindFirstObjectByType<PlayerController2D>();
            if (playerObj != null)
                target = playerObj.transform;
        }
    }

    private void SetLimitsFromTilemap()
    {
        // Calcula límites solo si hay Tilemap
        bottomLeftLimit = Tilemap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        topRightLimit = Tilemap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);

        if (PlayerController2D.instance != null)
            PlayerController2D.instance.SetBounds(Tilemap.localBounds.min, Tilemap.localBounds.max);
    }

    // Si se cambia de escena y se asigna otro Tilemap, se puede llamar desde AreaEntrance
    public void UpdateTilemap(Tilemap newTilemap)
    {
        Tilemap = newTilemap;
        SetLimitsFromTilemap();
    }
}
