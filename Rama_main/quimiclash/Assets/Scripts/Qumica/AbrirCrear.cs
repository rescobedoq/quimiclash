using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonCrear : MonoBehaviour
{
    public void IrASegundaPantalla()
    {
        // Buscar el manager de seleccionados y guardar antes de cambiar
        Seleccionados seleccionados = FindFirstObjectByType<Seleccionados>();
        if (seleccionados != null)
        {
            seleccionados.GuardarAntesDeCambioEscena();
        }

        SceneManager.LoadScene("Mesa_de_Construccion");
    }
}