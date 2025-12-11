#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class Remove
{
    static Remove()
    {
        // Buscar y deshabilitar el script problemático
        string[] guids = AssetDatabase.FindAssets("DOTweenModuleEPOOutline");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PluginImporter importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer != null)
            {
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SaveAndReimport();
                UnityEngine.Debug.Log($"Script deshabilitado: {path}");
            }
        }
    }
}
#endif