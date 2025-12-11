using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BotonAlbumMolecula : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Componentes")]
    [SerializeField] private Image imagenFondo;
    [SerializeField] private TextMeshProUGUI textoNombre;

    [Header("Efectos")]
    [SerializeField] private Color colorHover = new Color(0.9f, 0.95f, 1f, 1f);
    [SerializeField] private float escalaHover = 1.1f;

    [Header("Estado")]
    [SerializeField] private bool desbloqueada = false;

    private PlantillaObjetoMolecula datosMolecula;
    private Color colorOriginal;
    private Vector3 escalaOriginal;
    private Coroutine animacionCoroutine;

    void Start()
    {
        if (imagenFondo != null)
            colorOriginal = imagenFondo.color;

        escalaOriginal = transform.localScale;
    }

    public void Configurar(PlantillaObjetoMolecula molecula, bool estaDesbloqueada)
    {
        datosMolecula = molecula;
        desbloqueada = estaDesbloqueada;

        if (molecula != null)
        {
            if (imagenFondo != null)
            {
                // Si está desbloqueada, usar la imagen de la molécula
                if (estaDesbloqueada && molecula.imagenObjetoMolecula != null)
                {
                    imagenFondo.sprite = molecula.imagenObjetoMolecula;
                    imagenFondo.color = Color.white;
                    Debug.Log($"✓ Botón con imagen especial: {molecula.nombreMolecula}");
                }
                else
                {
                    // Mantener la imagen por defecto
                    imagenFondo.color = Color.white;
                }
            }

            if (textoNombre != null)
            {
                textoNombre.text = molecula.nombreMolecula;
                textoNombre.color = estaDesbloqueada ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f);
            }
        }

        if (estaDesbloqueada)
        {
            AñadirEfectosDesbloqueo();
        }
        else
        {
            QuitarEfectosDesbloqueo();
        }
    }

    void AñadirEfectosDesbloqueo()
    {
        Outline outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.8f, 0f, 1f);
            outline.effectDistance = new Vector2(2, 2);
        }
    }

    void QuitarEfectosDesbloqueo()
    {
        Outline outline = GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animacionCoroutine != null)
        {
            StopCoroutine(animacionCoroutine);
        }

        animacionCoroutine = StartCoroutine(AnimacionHoverEntrada());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animacionCoroutine != null)
        {
            StopCoroutine(animacionCoroutine);
        }

        animacionCoroutine = StartCoroutine(AnimacionHoverSalida());
    }

    IEnumerator AnimacionHoverEntrada()
    {
        float tiempo = 0f;
        Vector3 escalaInicial = transform.localScale;
        Vector3 escalaObjetivo = escalaOriginal * escalaHover;

        if (imagenFondo != null)
        {
            Color colorInicial = imagenFondo.color;

            while (tiempo < 0.2f)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / 0.2f;

                if (imagenFondo != null)
                    imagenFondo.color = Color.Lerp(colorInicial, colorHover, t);

                transform.localScale = Vector3.Lerp(escalaInicial, escalaObjetivo, t);
                yield return null;
            }
        }

        if (imagenFondo != null)
            imagenFondo.color = colorHover;
        transform.localScale = escalaObjetivo;
    }

    IEnumerator AnimacionHoverSalida()
    {
        float tiempo = 0f;
        Vector3 escalaInicial = transform.localScale;

        if (imagenFondo != null)
        {
            Color colorInicial = imagenFondo.color;

            while (tiempo < 0.2f)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / 0.2f;

                if (imagenFondo != null)
                    imagenFondo.color = Color.Lerp(colorInicial, Color.white, t);

                transform.localScale = Vector3.Lerp(escalaInicial, escalaOriginal, t);
                yield return null;
            }
        }

        if (imagenFondo != null)
            imagenFondo.color = Color.white;
        transform.localScale = escalaOriginal;
    }

    public void OnClick()
    {
        if (datosMolecula == null) return;

        Debug.Log($"Botón de álbum clickeado: {datosMolecula.nombreMolecula}");
        Debug.Log($"Estado: {(desbloqueada ? "DESBLOQUEADA" : "BLOQUEADA")}");

        // USAR UIControladorMensajes en lugar de mostrar directamente
        if (UIControladorMensajes.Instancia != null)
        {
            // Este método ya verifica si está desbloqueada y muestra la ventana correcta
            UIControladorMensajes.Instancia.MostrarMensajeParaMolecula(datosMolecula);
        }
        else
        {
            Debug.LogError("No se encontró UIControladorMensajes.Instancia!");

            // Fallback: mostrar según estado
            if (!desbloqueada)
            {
                MostrarElementosNecesarios();
            }
            else
            {
                Debug.Log($"Molécula {datosMolecula.nombreMolecula} ya desbloqueada");
            }
        }

        StartCoroutine(EfectoClic());
    }

    void MostrarElementosNecesarios()
    {
        List<string> elementosNecesarios = ObtenerElementosUnicosDeMolecula();

        // CORREGIDO: Usar FindFirstObjectByType en lugar de FindObjectOfType
        UIControladorMensajes controlador = FindFirstObjectByType<UIControladorMensajes>();

        if (controlador != null)
        {
            controlador.MostrarMensajeElementos(datosMolecula.nombreMolecula, elementosNecesarios);
        }
        else
        {
            Debug.LogError("No se encontró UIControladorMensajes en la escena!");

            // Mostrar en consola como fallback
            string mensaje = $"Molécula no encontrada, necesitas: {datosMolecula.nombreMolecula}\n";
            foreach (string elemento in elementosNecesarios)
            {
                mensaje += $"• {elemento}\n";
            }
            Debug.Log(mensaje);
        }
    }

    List<string> ObtenerElementosUnicosDeMolecula()
    {
        List<string> elementosUnicos = new List<string>();

        if (datosMolecula != null && datosMolecula.elementosEnCoordenadas != null)
        {
            foreach (var elementoCoord in datosMolecula.elementosEnCoordenadas)
            {
                string nombreElemento = elementoCoord.nombreElemento;

                if (!string.IsNullOrEmpty(nombreElemento) && !elementosUnicos.Contains(nombreElemento))
                {
                    elementosUnicos.Add(nombreElemento);
                }
            }
        }
            
        return elementosUnicos;
    }

    IEnumerator EfectoClic()
    {
        Vector3 escalaClic = escalaOriginal * 0.9f;
        transform.localScale = escalaClic;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = escalaOriginal;
    }

    public bool EstaDesbloqueada() => desbloqueada;
    public string GetNombreMolecula() => datosMolecula != null ? datosMolecula.nombreMolecula : "";
    public PlantillaObjetoMolecula GetDatosMolecula() => datosMolecula;

    [ContextMenu("Debug: Ver estado")]
    public void DebugVerEstado()
    {
        if (datosMolecula != null)
        {
            Debug.Log($"Molécula: {datosMolecula.nombreMolecula}");
            Debug.Log($"Desbloqueada en script: {desbloqueada}");
            Debug.Log($"Desbloqueada en plantilla: {datosMolecula.desbloqueada}");
            Debug.Log($"Imagen asignada: {datosMolecula.imagenObjetoMolecula?.name ?? "null"}");
        }
    }
}