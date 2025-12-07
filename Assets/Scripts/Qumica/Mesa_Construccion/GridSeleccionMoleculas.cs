using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridSeleccionMoleculas : MonoBehaviour
{
    [Header("Configuración Grid")]
    [SerializeField] private int filas = 4;
    [SerializeField] private int columnas = 6;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Vector2 buttonSize = new Vector2(80, 80);
    [SerializeField] private float spacing = 5f;

    [Header("Configuración Feedback")]
    [SerializeField] private float duracionFeedback = 1f;
    [SerializeField] private Color colorCorrecto = new Color(0.6f, 1f, 0.6f);
    [SerializeField] private Color colorIncorrecto = new Color(1f, 0.8f, 0.6f);

    [Header("Referencia Ventana Felicitaciones")]
    [SerializeField] private VentanaFelicitaciones ventanaFelicitaciones;

    [Header("Referencia MostrarSeleccionados")]
    [SerializeField] private MostrarSeleccionados mostrarSeleccionados;

    private Button[,] gridBotones;
    private Image[,] gridImagenes;
    private Sprite[,] spritesOriginales;
    private List<Vector2Int> patronActual = new List<Vector2Int>();
    private HashSet<Vector2Int> celdasSeleccionadas = new HashSet<Vector2Int>();
    private PlantillaObjetoMolecula moleculaActual;

    [Header("Modo Colocación")]
    private bool modoColocacionElemento = false;
    private Plantilla_Objeto elementoParaColocar;

    private Dictionary<Vector2Int, Plantilla_Objeto> elementosEnGrid = new Dictionary<Vector2Int, Plantilla_Objeto>();
    private Dictionary<Vector2Int, bool> elementosCorrectos = new Dictionary<Vector2Int, bool>();

    void Start()
    {
        CrearGrid();
        DesactivarTodoElGrid();

        // Buscar automáticamente la ventana si no está asignada
        if (ventanaFelicitaciones == null)
        {
            ventanaFelicitaciones = FindAnyObjectByType<VentanaFelicitaciones>();
            if (ventanaFelicitaciones != null)
            {
                Debug.Log("VentanaFelicitaciones encontrada automáticamente");
            }
        }

        if (mostrarSeleccionados == null)
        {
            mostrarSeleccionados = FindAnyObjectByType<MostrarSeleccionados>();
        }
    }

    void CrearGrid()
    {
        gridBotones = new Button[filas, columnas];
        gridImagenes = new Image[filas, columnas];
        spritesOriginales = new Sprite[filas, columnas];

        float totalWidth = columnas * (buttonSize.x + spacing) - spacing;
        float totalHeight = filas * (buttonSize.y + spacing) - spacing;

        float offsetX = -totalWidth / 2f + buttonSize.x / 2f;
        float offsetY = totalHeight / 2f - buttonSize.y / 2f;

        for (int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                CrearBoton(fila, columna, offsetX, offsetY);
            }
        }
    }

    void CrearBoton(int fila, int columna, float offsetX, float offsetY)
    {
        GameObject btnObj = Instantiate(buttonPrefab, this.transform);
        btnObj.name = $"Btn_Grid_{fila}_{columna}";

        // Posicionar
        RectTransform rectTransform = btnObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = buttonSize;
            rectTransform.anchoredPosition = new Vector2(
                offsetX + columna * (buttonSize.x + spacing),
                offsetY - fila * (buttonSize.y + spacing)
            );
        }

        // Configurar botón
        Button btn = btnObj.GetComponent<Button>();
        gridBotones[fila, columna] = btn;

        // Configurar imagen
        Image img = btnObj.GetComponent<Image>();
        gridImagenes[fila, columna] = img;
        if (img != null)
        {
            spritesOriginales[fila, columna] = img.sprite;
        }

        // Configurar listener del botón
        int x = fila;
        int y = columna;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnCeldaGridClickeada(x, y));

        // Crear texto de coordenada (para debug)
        CrearTextoCelda(btnObj, fila, columna);
    }

    void CrearTextoCelda(GameObject parent, int fila, int columna)
    {
        GameObject textoObj = new GameObject("TextoCoordenada");
        textoObj.transform.SetParent(parent.transform);
        textoObj.transform.localPosition = Vector3.zero;

        UnityEngine.UI.Text texto = textoObj.AddComponent<UnityEngine.UI.Text>();
        texto.text = $"{fila},{columna}";
        texto.fontSize = 8;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.gray;

        RectTransform rectTransform = textoObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = buttonSize;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    void OnCeldaGridClickeada(int fila, int columna)
    {
        Debug.Log($"Celda clickeada: ({fila}, {columna})");

        // Verificar si la celda está en el patrón actual
        Vector2Int coord = new Vector2Int(fila, columna);

        if (!patronActual.Contains(coord))
        {
            Debug.Log("Esta celda no está en el patrón de la molécula actual");
            return;
        }

        // Si estamos en modo colocación, colocar el elemento
        if (modoColocacionElemento && elementoParaColocar != null)
        {
            ColocarElementoEnCelda(coord, elementoParaColocar);
            return;
        }

        // Si ya hay un elemento en esta celda, ofrecer opción para removerlo
        if (elementosEnGrid.ContainsKey(coord))
        {
            MostrarOpcionesParaCelda(coord);
        }
    }

    void ColocarElementoEnCelda(Vector2Int coordenada, Plantilla_Objeto elemento)
    {
        // Verificar si ya hay un elemento en esta celda
        if (elementosEnGrid.ContainsKey(coordenada))
        {
            Debug.Log($"Ya hay un elemento en la celda ({coordenada.x}, {coordenada.y})");
            return;
        }

        // Colocar el elemento en la celda
        elementosEnGrid[coordenada] = elemento;

        // Actualizar la imagen de la celda
        if (gridImagenes[coordenada.x, coordenada.y] != null)
        {
            gridImagenes[coordenada.x, coordenada.y].sprite = elemento.imagenObjeto;
            gridImagenes[coordenada.x, coordenada.y].color = Color.white;
        }

        // Verificar si la colocación es correcta
        VerificarColocacion(coordenada, elemento);

        // Notificar a MostrarSeleccionados que el elemento fue colocado
        if (mostrarSeleccionados != null)
        {
            mostrarSeleccionados.ElementoColocadoEnGrid(elemento, coordenada);
        }

        // Salir del modo colocación
        modoColocacionElemento = false;
        elementoParaColocar = null;

        // Verificar si la molécula está completa
        VerificarMoléculaCompleta();

        Debug.Log($"Elemento '{elemento.nombreObjeto}' colocado en ({coordenada.x}, {coordenada.y})");
    }

    void VerificarColocacion(Vector2Int coordenada, Plantilla_Objeto elemento)
    {
        if (moleculaActual == null) return;

        // Verificar si el elemento está en la posición correcta según la plantilla
        string elementoEsperado = moleculaActual.ObtenerElementoEnCoordenada(coordenada);
        bool esCorrecto = (elementoEsperado == elemento.nombreObjeto);

        elementosCorrectos[coordenada] = esCorrecto;

        // Mostrar feedback visual
        StartCoroutine(MostrarFeedbackColocacion(coordenada, esCorrecto));

        Debug.Log($"Colocación en ({coordenada.x}, {coordenada.y}): {(esCorrecto ? "CORRECTA" : "INCORRECTA")} (Esperado: {elementoEsperado}, Colocado: {elemento.nombreObjeto})");
    }

    System.Collections.IEnumerator MostrarFeedbackColocacion(Vector2Int coordenada, bool esCorrecto)
    {
        if (gridImagenes[coordenada.x, coordenada.y] == null) yield break;

        Color colorOriginal = gridImagenes[coordenada.x, coordenada.y].color;
        Color colorFeedback = esCorrecto ? colorCorrecto : colorIncorrecto;

        gridImagenes[coordenada.x, coordenada.y].color = colorFeedback;
        yield return new WaitForSeconds(duracionFeedback);
        gridImagenes[coordenada.x, coordenada.y].color = colorOriginal;
    }

    void VerificarMoléculaCompleta()
    {
        if (moleculaActual == null) return;

        bool todasLlenas = true;
        bool todasCorrectas = true;

        foreach (Vector2Int coordGrid in patronActual)
        {
            if (!elementosEnGrid.ContainsKey(coordGrid))
            {
                todasLlenas = false;
                break;
            }

            if (!elementosCorrectos.ContainsKey(coordGrid) || !elementosCorrectos[coordGrid])
            {
                todasCorrectas = false;
            }
        }

        if (todasLlenas)
        {
            if (todasCorrectas)
            {
                Debug.Log("¡¡¡MOLÉCULA COMPLETADA CORRECTAMENTE!!!");

                // Marcar como desbloqueada
                moleculaActual.Desbloquear();

                // Si hay un AlbumMoleculas en la escena, notificar
                AlbumMoleculas album = FindAnyObjectByType<AlbumMoleculas>();
                if (album != null)
                {
                    album.DesbloquearMolecula(moleculaActual.nombreMolecula);
                }

                // Mostrar efecto de completado
                StartCoroutine(EfectoCompletadoExitoso());

                // Después del efecto, mostrar ventana de felicitaciones
                Invoke("MostrarVentanaFelicitaciones", 1.5f);
            }
            else
            {
                Debug.Log("Molécula completada, pero hay elementos en posiciones incorrectas");
            }
        }
    }


    void MostrarVentanaFelicitaciones()
    {
        if (ventanaFelicitaciones != null && moleculaActual != null)
        {
            ventanaFelicitaciones.TraerAdelante();
            ventanaFelicitaciones.MostrarFelicitaciones(moleculaActual);
            Debug.Log($"Ventana de felicitaciones solicitada para: {moleculaActual.nombreMolecula}");
        }
        else
        {
            if (ventanaFelicitaciones == null)
                Debug.LogError("VentanaFelicitaciones no asignada");
            if (moleculaActual == null)
                Debug.LogError("moleculaActual es null");
        }
    }

    System.Collections.IEnumerator EfectoCompletadoExitoso()
    {
        Debug.Log("¡¡¡EFECTO DE COMPLETADO EXITOSO!!!");

        for (int i = 0; i < 5; i++)
        {
            foreach (var coord in patronActual)
            {
                if (gridImagenes[coord.x, coord.y] != null)
                {
                    gridImagenes[coord.x, coord.y].color = Color.green;
                }
            }

            yield return new WaitForSeconds(0.2f);

            foreach (var coord in patronActual)
            {
                if (gridImagenes[coord.x, coord.y] != null)
                {
                    gridImagenes[coord.x, coord.y].color = Color.yellow;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        // Restaurar todos a color blanco normal
        foreach (var coord in patronActual)
        {
            if (gridImagenes[coord.x, coord.y] != null)
            {
                gridImagenes[coord.x, coord.y].color = Color.white;
            }
        }

        Debug.Log("Efecto de completado terminado");
    }

    void MostrarOpcionesParaCelda(Vector2Int coordenada)
    {
        // Aquí puedes implementar un menú contextual o simplemente remover el elemento
        Plantilla_Objeto elemento = elementosEnGrid[coordenada];

        // Remover el elemento del grid
        elementosEnGrid.Remove(coordenada);
        elementosCorrectos.Remove(coordenada);

        // Restaurar la imagen original
        if (gridImagenes[coordenada.x, coordenada.y] != null)
        {
            gridImagenes[coordenada.x, coordenada.y].sprite = spritesOriginales[coordenada.x, coordenada.y];
            gridImagenes[coordenada.x, coordenada.y].color = Color.white;
        }

        // Devolver el elemento a la lista
        if (mostrarSeleccionados != null)
        {
            mostrarSeleccionados.ElementoDevueltoALista(elemento);
        }

        Debug.Log($"Elemento '{elemento.nombreObjeto}' removido de ({coordenada.x}, {coordenada.y})");
    }

    void DesactivarTodoElGrid()
    {
        for (int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                if (gridBotones[fila, columna] != null)
                {
                    gridBotones[fila, columna].interactable = false;
                    if (gridImagenes[fila, columna] != null)
                    {
                        gridImagenes[fila, columna].color = Color.gray;
                    }
                }
            }
        }
    }

    void ActivarSoloPatron(List<Vector2Int> patron)
    {
        DesactivarTodoElGrid();

        foreach (Vector2Int coord in patron)
        {
            if (coord.x >= 0 && coord.x < filas && coord.y >= 0 && coord.y < columnas)
            {
                if (gridBotones[coord.x, coord.y] != null)
                {
                    gridBotones[coord.x, coord.y].interactable = true;
                    if (gridImagenes[coord.x, coord.y] != null)
                    {
                        gridImagenes[coord.x, coord.y].color = Color.white;
                    }
                }
            }
        }
    }

    public void OnMoleculaSeleccionada(PlantillaObjetoMolecula molecula)
    {
        Debug.Log($"=== MOLÉCULA SELECCIONADA: {molecula.nombreMolecula} ===");

        // Limpiar grid previo
        LimpiarGrid();

        moleculaActual = molecula;
        patronActual = molecula.patronCeldasDisponibles;

        // Activar solo las celdas del patrón
        ActivarSoloPatron(patronActual);

        Debug.Log($"Patrón activado con {patronActual.Count} celdas");
    }

    public void PrepararParaColocarElemento(Plantilla_Objeto elemento)
    {
        modoColocacionElemento = true;
        elementoParaColocar = elemento;

        Debug.Log($"Modo colocación activado. Elemento a colocar: {elemento.nombreObjeto}");
        Debug.Log("Haz clic en una celda del grid para colocar el elemento");
    }

    void LimpiarGrid()
    {
        elementosEnGrid.Clear();
        elementosCorrectos.Clear();

        for (int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                if (gridImagenes[fila, columna] != null)
                {
                    gridImagenes[fila, columna].sprite = spritesOriginales[fila, columna];
                    gridImagenes[fila, columna].color = Color.white;
                }
            }
        }
    }

    [ContextMenu("Test: Forzar Completado Molecula")]
    public void TestForzarCompletado()
    {
        if (moleculaActual != null)
        {
            // Simular que todas las celdas están llenas y correctas
            foreach (Vector2Int coord in patronActual)
            {
                if (!elementosEnGrid.ContainsKey(coord))
                {
                    elementosEnGrid[coord] = ScriptableObject.CreateInstance<Plantilla_Objeto>();
                    elementosCorrectos[coord] = true;
                }
            }

            VerificarMoléculaCompleta();
        }
        else
        {
            Debug.LogWarning("No hay molécula seleccionada para test");
        }
    }
}