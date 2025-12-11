using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TablaSeleccionMoleculas : MonoBehaviour
{
    [Header("Configuración Tabla")]
    [SerializeField] GameObject prefabObjectTabla; // Prefab del botón de selección
    [SerializeField] int numeroMaximoElements; // Máximo de moléculas a mostrar
    [SerializeField] PlantillaObjetoMolecula[] listaMoleculas; // Lista de ScriptableObjects

    [Header("Referencia al Grid")]
    [SerializeField] private GridSeleccionMoleculas gridController; // Grid donde se mostrarán los patrones

    private List<GameObject> moleculasInstanciadas = new List<GameObject>();

    private void Start()
    {
        Debug.Log("=== INICIANDO TABLA DE SELECCIÓN DE MOLÉCULAS ===");

        // Verificar que tenemos el grid controller
        if (gridController == null)
        {
            Debug.LogError("GridController no asignado en el Inspector!");
            Debug.Log("Por favor, arrastra el GameObject con GridSeleccionMoleculas al campo Grid Controller");

            // Buscar automáticamente
            gridController = FindAnyObjectByType<GridSeleccionMoleculas>();
            if (gridController != null)
            {
                Debug.Log("GridController encontrado automáticamente: " + gridController.name);
            }
            else
            {
                Debug.LogError("No se encontró ningún GridSeleccionMoleculas en la escena");
                return;
            }
        }

        // Verificar prefab
        if (prefabObjectTabla == null)
        {
            Debug.LogError("Prefab Object Tabla no asignado!");
            return;
        }

        // Verificar lista de moléculas
        if (listaMoleculas == null || listaMoleculas.Length == 0)
        {
            Debug.LogWarning("ListaMoleculas está vacía o no asignada en el Inspector!");
            Debug.Log("Crea ScriptableObjects con Create -> Objeto Molecula y arrástralos al array");
            return;
        }

        // Mostrar información de debug
        Debug.Log($"Lista de moléculas: {listaMoleculas.Length} elementos");
        int contadorValidos = 0;
        for (int i = 0; i < listaMoleculas.Length; i++)
        {
            if (listaMoleculas[i] != null)
            {
                Debug.Log($"  [{i}] '{listaMoleculas[i].nombreMolecula}' - Celdas: {listaMoleculas[i].patronCeldasDisponibles.Count}");
                contadorValidos++;
            }
            else
            {
                Debug.LogWarning($"  [{i}] NULL - Por favor, asigna un ScriptableObject");
            }
        }

        if (contadorValidos == 0)
        {
            Debug.LogError("No hay moléculas válidas en la lista. No se puede crear la tabla.");
            return;
        }

        // Crear la tabla de selección
        CrearTablaSeleccion();
    }

    void CrearTablaSeleccion()
    {
        Debug.Log("=== CREANDO TABLA DE SELECCIÓN ===");

        // Limpiar tabla previa si existe
        LimpiarTabla();

        // Determinar cuántas moléculas crear
        int cantidadACrear = Mathf.Min(numeroMaximoElements, listaMoleculas.Length);

        if (cantidadACrear == 0)
        {
            Debug.LogWarning("No hay moléculas para mostrar en la tabla");
            return;
        }

        Debug.Log($"Creando {cantidadACrear} botones de selección...");

        int creadasConExito = 0;

        for (int i = 0; i < cantidadACrear; i++)
        {
            if (listaMoleculas[i] == null)
            {
                Debug.LogWarning($"Molécula [{i}] es null - saltando");
                continue;
            }

            // Instanciar el botón de selección
            GameObject botonSeleccion = Instantiate(prefabObjectTabla, Vector3.zero,
                Quaternion.identity, this.transform);

            botonSeleccion.name = $"Btn_{listaMoleculas[i].nombreMolecula}";
            moleculasInstanciadas.Add(botonSeleccion);

            // Configurar el componente ObjetoMolecula
            ObjetoMolecula objMolecula = botonSeleccion.GetComponent<ObjetoMolecula>();

            if (objMolecula == null)
            {
                Debug.LogError($"El prefab {prefabObjectTabla.name} no tiene componente ObjetoMolecula!");
                Destroy(botonSeleccion);
                continue;
            }

            // IMPORTANTE: Solo llamar a CrearObjeto, que ya configurará el texto
            objMolecula.CrearObjeto(listaMoleculas[i], gridController);

            // Configurar el botón para que al hacer clic envíe la molécula al grid
            Button btn = botonSeleccion.GetComponent<Button>();
            if (btn != null)
            {
                // Configurar listener del botón
                PlantillaObjetoMolecula molActual = listaMoleculas[i];
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnMoleculaSeleccionadaEnTabla(molActual);
                });

                Debug.Log($"✓ Botón creado: {listaMoleculas[i].nombreMolecula}");
                creadasConExito++;
            }
            else
            {
                Debug.LogError($"El prefab {prefabObjectTabla.name} no tiene componente Button!");
                Destroy(botonSeleccion);
            }
        }

        Debug.Log($"=== TABLA CREADA ===");
        Debug.Log($"Botones creados: {creadasConExito}/{cantidadACrear}");

        if (creadasConExito > 0)
        {
            Debug.Log($"Tabla lista. Haz clic en cualquier molécula para ver su patrón en el grid ({gridController.name})");

            // Si solo hay una molécula, seleccionarla automáticamente para debug
            if (creadasConExito == 1 && listaMoleculas[0] != null)
            {
                Debug.Log("Solo hay una molécula. Seleccionando automáticamente...");
                StartCoroutine(SeleccionarPrimeraAutomaticamente());
            }
        }
        else
        {
            Debug.LogError("No se pudo crear ningún botón. Verifica:");
            Debug.LogError("1. El prefab tiene componente Button");
            Debug.LogError("2. El prefab tiene componente ObjetoMolecula");
            Debug.LogError("3. Los ScriptableObjects están asignados correctamente");
        }
    }

    IEnumerator SeleccionarPrimeraAutomaticamente()
    {
        // Pequeña espera para que todo se inicialice
        yield return new WaitForSeconds(0.5f);

        if (listaMoleculas.Length > 0 && listaMoleculas[0] != null)
        {
            OnMoleculaSeleccionadaEnTabla(listaMoleculas[0]);
        }
    }

    void OnMoleculaSeleccionadaEnTabla(PlantillaObjetoMolecula molecula)
    {
        if (molecula == null)
        {
            Debug.LogWarning("Intento de seleccionar molécula nula");
            return;
        }

        Debug.Log($"--- MOLÉCULA SELECCIONADA DESDE TABLA ---");
        Debug.Log($"Nombre: {molecula.nombreMolecula}");
        Debug.Log($"Dificultad: {molecula.dificultad}");
        Debug.Log($"Patrón: {molecula.patronCeldasDisponibles.Count} celdas");

        // Mostrar coordenadas del patrón
        if (molecula.patronCeldasDisponibles.Count > 0)
        {
            string coordenadas = "Coordenadas: ";
            foreach (Vector2Int coord in molecula.patronCeldasDisponibles)
            {
                coordenadas += $"({coord.x},{coord.y}) ";
            }
            Debug.Log(coordenadas);
        }
        else
        {
            Debug.LogWarning("La molécula no tiene patrón definido");
        }

        if (gridController != null)
        {
            // Enviar la molécula seleccionada al grid
            gridController.OnMoleculaSeleccionada(molecula);

            // Cambiar color del botón seleccionado (opcional)
            StartCoroutine(ResaltarSeleccion(molecula.nombreMolecula));
        }
        else
        {
            Debug.LogError("GridController no disponible!");

            // Intentar encontrar el grid nuevamente
            gridController = FindAnyObjectByType<GridSeleccionMoleculas>();
            if (gridController != null)
            {
                Debug.Log("GridController encontrado. Reintentando...");
                gridController.OnMoleculaSeleccionada(molecula);
            }
            else
            {
                Debug.LogError("No se pudo encontrar GridController");
            }
        }
    }

    IEnumerator ResaltarSeleccion(string nombreMol)
    {
        // Buscar el botón correspondiente
        foreach (GameObject btn in moleculasInstanciadas)
        {
            if (btn != null && btn.name.Contains(nombreMol))
            {
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    Color original = img.color;
                    img.color = new Color(0.8f, 0.9f, 1f, 1f); // Azul claro
                    yield return new WaitForSeconds(0.3f);
                    img.color = original;
                }
                break;
            }
        }
    }

    void LimpiarTabla()
    {
        foreach (GameObject obj in moleculasInstanciadas)
        {
            if (obj != null)
                Destroy(obj);
        }
        moleculasInstanciadas.Clear();

        // También destruir cualquier hijo directo
        foreach (Transform child in transform)
        {
            if (child != null && child.gameObject != null)
                Destroy(child.gameObject);
        }

        Debug.Log("Tabla limpiada");
    }

    // ========== MÉTODOS PARA DEBUG DESDE EL INSPECTOR ==========

    [ContextMenu("TEST: Forzar Creación Tabla")]
    void TestForzarCreacion()
    {
        Debug.Log("=== TEST FORZADO DE CREACIÓN ===");

        if (listaMoleculas == null || listaMoleculas.Length == 0)
        {
            // Crear molécula de prueba
            PlantillaObjetoMolecula testMol = ScriptableObject.CreateInstance<PlantillaObjetoMolecula>();
            testMol.nombreMolecula = "AGUA_TEST";
            testMol.dificultad = 1;
            testMol.patronCeldasDisponibles = new List<Vector2Int>
            {
                new Vector2Int(1, 1),
                new Vector2Int(2, 1),
                new Vector2Int(1, 2)
            };

            listaMoleculas = new PlantillaObjetoMolecula[] { testMol };
            numeroMaximoElements = 1;

            Debug.Log("Creada molécula de prueba: AGUA_TEST");
        }

        CrearTablaSeleccion();
    }

    [ContextMenu("TEST: Seleccionar Primera Molécula")]
    void TestSeleccionarPrimera()
    {
        if (listaMoleculas != null && listaMoleculas.Length > 0 && listaMoleculas[0] != null)
        {
            OnMoleculaSeleccionadaEnTabla(listaMoleculas[0]);
        }
        else
        {
            Debug.LogWarning("No hay moléculas para seleccionar");
        }
    }

    [ContextMenu("TEST: Ver Configuración Prefab")]
    void TestVerConfiguracionPrefab()
    {
        if (prefabObjectTabla == null)
        {
            Debug.LogError("Prefab no asignado");
            return;
        }

        Debug.Log("=== CONFIGURACIÓN DEL PREFAB ===");
        Debug.Log($"Nombre: {prefabObjectTabla.name}");

        // Verificar componentes
        Button btn = prefabObjectTabla.GetComponent<Button>();
        Debug.Log($"Tiene Button: {btn != null}");

        ObjetoMolecula objMol = prefabObjectTabla.GetComponent<ObjetoMolecula>();
        Debug.Log($"Tiene ObjetoMolecula: {objMol != null}");
        if (objMol != null)
        {
            // Verificar si tiene el campo textoNombre asignado
            var campoTexto = objMol.GetType().GetField("textoNombre");
            if (campoTexto != null)
            {
                TextMeshProUGUI texto = (TextMeshProUGUI)campoTexto.GetValue(objMol);
                Debug.Log($"  Campo textoNombre asignado: {texto != null}");
                if (texto != null)
                    Debug.Log($"  Objeto de texto: {texto.gameObject.name}");
            }
        }

        // Verificar texto
        TextMeshProUGUI[] textos = prefabObjectTabla.GetComponentsInChildren<TextMeshProUGUI>(true);
        Debug.Log($"TextMeshProUGUI encontrados: {textos.Length}");
        foreach (TextMeshProUGUI texto in textos)
        {
            Debug.Log($"  Texto: '{texto.text}' en {texto.gameObject.name}");
        }

        // Verificar imágenes
        Image[] imagenes = prefabObjectTabla.GetComponentsInChildren<Image>(true);
        Debug.Log($"Imágenes encontradas: {imagenes.Length}");
        foreach (Image imagen in imagenes)
        {
            Debug.Log($"  Imagen: {imagen.gameObject.name}, Sprite: {imagen.sprite?.name ?? "null"}");
        }
    }

    [ContextMenu("Limpiar Tabla Completamente")]
    void LimpiarTablaCompletamente()
    {
        LimpiarTabla();
        Debug.Log("Tabla limpiada completamente");
    }

    [ContextMenu("Buscar Grid Automáticamente")]
    void BuscarGridAutomaticamente()
    {
        gridController = FindAnyObjectByType<GridSeleccionMoleculas>();
        if (gridController != null)
        {
            Debug.Log($"Grid encontrado: {gridController.name}");
        }
        else
        {
            Debug.LogError("No se encontró GridSeleccionMoleculas en la escena");
        }
    }

    [ContextMenu("Debug: Ver Moleculas Instanciadas")]
    void DebugVerMoleculasInstanciadas()
    {
        Debug.Log($"=== MOLÉCULAS INSTANCIADAS: {moleculasInstanciadas.Count} ===");
        for (int i = 0; i < moleculasInstanciadas.Count; i++)
        {
            if (moleculasInstanciadas[i] != null)
            {
                Debug.Log($"  [{i}] {moleculasInstanciadas[i].name}");

                // Verificar texto
                TextMeshProUGUI texto = moleculasInstanciadas[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                {
                    Debug.Log($"    Texto: '{texto.text}'");
                }
                else
                {
                    Debug.LogWarning($"    No tiene texto visible");
                }
            }
            else
            {
                Debug.LogWarning($"  [{i}] NULL (destruido)");
            }
        }
    }

    void OnValidate()
    {
        // Validación en tiempo de edición
        if (numeroMaximoElements < 0) numeroMaximoElements = 0;
        if (numeroMaximoElements > 50) numeroMaximoElements = 50; // Límite razonable

        // Si hay lista, asegurar que no hay elementos nulos en medio
        if (listaMoleculas != null)
        {
            for (int i = 0; i < listaMoleculas.Length; i++)
            {
                if (listaMoleculas[i] == null)
                {
                    Debug.LogWarning($"Elemento [{i}] de ListaMoleculas es nulo");
                }
            }
        }
    }

    void OnDestroy()
    {
        // Limpiar al destruir
        LimpiarTabla();
    }
}