using UnityEngine;

public class HealthBarFix : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        // Guardamos la referencia del padre (el enemigo)
        parentTransform = transform.parent;
    }

    void LateUpdate()
    {
        // Si el padre se voltea (escala X negativa), invertimos la escala local de la barra
        // para compensar y que siempre se vea bien.
        if (parentTransform != null)
        {
            float parentScaleX = parentTransform.localScale.x;

            // Si el padre mira a la izquierda (-1), ponemos la barra en -1 local 
            // (-1 * -1 = 1 en el mundo). Si es 1, la ponemos en 1.
            float newScaleX = (parentScaleX < 0) ? -1f : 1f;

            // Mantenemos el tamaño original absoluto que le diste en el editor (ej. 0.01)
            // Asumimos que la escala Y y Z no cambian.
            transform.localScale = new Vector3(newScaleX * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}