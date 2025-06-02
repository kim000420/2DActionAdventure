using UnityEngine;
using UnityEditor;

public class CopyPastePivot : EditorWindow
{
    private Vector2 pivot = new Vector2(0.5f, 0f); // 기본값: Bottom Center

    [MenuItem("Tools/Pivot Setter")]
    public static void ShowWindow()
    {
        GetWindow<CopyPastePivot>("Pivot Setter");
    }

    void OnGUI()
    {
        GUILayout.Label("Set Custom Pivot", EditorStyles.boldLabel);
        pivot = EditorGUILayout.Vector2Field("Pivot", pivot);

        if (GUILayout.Button("Apply Pivot to Selected Sprites"))
        {
            ApplyPivotToSelected(pivot);
        }
    }

    void ApplyPivotToSelected(Vector2 newPivot)
    {
        Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Single)
            {
                importer.spritePivot = newPivot;
                importer.SaveAndReimport();
                Debug.Log($"Pivot 적용됨: {path}");
            }
        }
    }
}