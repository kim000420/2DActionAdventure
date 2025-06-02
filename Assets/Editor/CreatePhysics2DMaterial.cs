// Editor ������ �־�� �մϴ�
using UnityEditor;
using UnityEngine;

public class CreatePhysics2DMaterial
{
    [MenuItem("Assets/Create/Physics 2D Material (Force)")]
    public static void CreatePhysicsMaterial2D()
    {
        var mat = new PhysicsMaterial2D("NoFriction");
        AssetDatabase.CreateAsset(mat, "Assets/PhysicsMaterials/NoFriction.physicsMaterial2D");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mat;
    }
}
