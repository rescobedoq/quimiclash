using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjetoM", menuName = "Objeto Molecula")]
public class PlantillaObjetoMolecula : ScriptableObject
{
    [Header("Información Visual")]
    public Sprite imagenObjetoMolecula;
    public Sprite imagenDescripcionMolecula;
    public string nombreMolecula;
    public int dificultad;

    [Header("Estado")]
    public bool desbloqueada = false; // <-- Este estado es solo en memoria

    [Header("Patrón en el grid")]
    public List<Vector2Int> patronCeldasDisponibles = new List<Vector2Int>();

    [Header("Asignación de elementos a coordenadas")]
    public List<ElementoEnCoordenada> elementosEnCoordenadas = new List<ElementoEnCoordenada>();

    [System.Serializable]
    public class ElementoEnCoordenada
    {
        public string nombreElemento;    // Ej: "HIDROGENO", "OXIGENO"
        public Vector2Int coordenada;    // Ej: (1,1), (2,1), (1,2)

        // Constructor para fácil creación
        public ElementoEnCoordenada(string nombre, int x, int y)
        {
            nombreElemento = nombre;
            coordenada = new Vector2Int(x, y);
        }
    }

    public bool EsCeldaDisponible(int x, int y)
    {
        return patronCeldasDisponibles.Contains(new Vector2Int(x, y));
    }

    // Método para obtener qué elemento debería ir en una coordenada
    public string ObtenerElementoEnCoordenada(Vector2Int coord)
    {
        foreach (var elemCoord in elementosEnCoordenadas)
        {
            if (elemCoord.coordenada == coord)
            {
                return elemCoord.nombreElemento;
            }
        }
        return null; // No hay elemento asignado a esta coordenada
    }

    // Método para obtener todas las coordenadas donde debe ir un elemento específico
    public List<Vector2Int> ObtenerCoordenadasDeElemento(string nombreElemento)
    {
        List<Vector2Int> coordenadas = new List<Vector2Int>();

        foreach (var elemCoord in elementosEnCoordenadas)
        {
            if (elemCoord.nombreElemento == nombreElemento)
            {
                coordenadas.Add(elemCoord.coordenada);
            }
        }

        return coordenadas;
    }

    // Método para verificar si una colocación es correcta
    public bool VerificarColocacionCorrecta(Vector2Int coord, string nombreElemento)
    {
        string elementoEsperado = ObtenerElementoEnCoordenada(coord);
        return elementoEsperado == nombreElemento;
    }

    // Método para desbloquear (solo en memoria)
    public void Desbloquear()
    {
        desbloqueada = true;
        Debug.Log($"Molécula '{nombreMolecula}' desbloqueada en memoria");
    }

    // Método para verificar estado
    public bool EstaDesbloqueada()
    {
        return desbloqueada;
    }

    // Método para debug
    [ContextMenu("Debug: Mostrar asignaciones")]
    public void DebugMostrarAsignaciones()
    {
        Debug.Log($"=== ASIGNACIONES PARA {nombreMolecula} ===");
        Debug.Log($"Estado en memoria: {(desbloqueada ? "DESBLOQUEADA" : "BLOQUEADA")}");

        foreach (var elemCoord in elementosEnCoordenadas)
        {
            Debug.Log($"  {elemCoord.nombreElemento} → ({elemCoord.coordenada.x}, {elemCoord.coordenada.y})");
        }

        Debug.Log($"Total: {elementosEnCoordenadas.Count} asignaciones");
    }

    [ContextMenu("Debug: Marcar como desbloqueada")]
    public void DebugMarcarComoDesbloqueada()
    {
        Desbloquear();
        Debug.Log($"Marcada como desbloqueada en memoria: {nombreMolecula}");
    }

    [ContextMenu("Debug: Marcar como bloqueada")]
    public void DebugMarcarComoBloqueada()
    {
        desbloqueada = false;
        Debug.Log($"Marcada como bloqueada en memoria: {nombreMolecula}");
    }
}