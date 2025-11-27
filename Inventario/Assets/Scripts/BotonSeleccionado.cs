using UnityEngine;

public class BotonSeleccionado : MonoBehaviour
{
    public Objeto objetoOriginal;
    private Seleccionados seleccionados;

    private void Awake()
    {
        seleccionados = FindFirstObjectByType<Seleccionados>();
    }

    public void CuandoLoPresiono()
    {
        seleccionados.QuitarSeleccionado(this);
    }
}
