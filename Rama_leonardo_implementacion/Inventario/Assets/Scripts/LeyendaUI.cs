using UnityEngine;
using TMPro;

public class LeyendaUI : MonoBehaviour
{
    public GameObject prefabItemLeyenda;
    public Transform contenedor;

    private (string nombre, Color color)[] categorias =
    {
        ("Metales alcalinos", new Color(0.87f, 0.55f, 0.96f)),
        ("Alcalinotérreos", new Color(0.95f, 0.76f, 0.41f)),
        ("Metales transición", new Color(0.93f, 0.62f, 0.29f)),
        ("Metaloides", new Color(0.28f, 0.59f, 0.36f)),
        ("No metales", new Color(0.19f, 0.77f, 0.44f)),
        ("Halógenos", new Color(0.22f, 0.55f, 0.77f)),
        ("Gases nobles", new Color(0.35f, 0.70f, 0.90f)),
        ("Lantánidos", new Color(0.78f, 0.47f, 0.94f)),
        ("Actínidos", new Color(0.55f, 0.25f, 0.76f))
    };

    void Start()
    {
        foreach (var c in categorias)
        {
            GameObject nuevo = Instantiate(prefabItemLeyenda, contenedor);
            nuevo.transform.Find("Color").GetComponent<UnityEngine.UI.Image>().color = c.color;
            nuevo.transform.Find("Texto").GetComponent<TextMeshProUGUI>().text = c.nombre;
        }
    }
}
