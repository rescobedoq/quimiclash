using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    [TextArea(2, 6)]
    public string[] dialogLines;
    private bool canActivate;

    void Update()
    {
        // Asegurarse de que el diálogo no se active si ya está mostrando algo
        if (canActivate && Input.GetButtonUp("Fire2") && !DialogManager.instance.dialogBox.activeInHierarchy)
        {
            DialogManager.instance.showDialog(dialogLines);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canActivate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canActivate = false;
        }
    }
}
