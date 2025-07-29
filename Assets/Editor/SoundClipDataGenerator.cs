using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class SoundClipDataGenerator : EditorWindow
{
    private DefaultAsset targetFolder;

    [MenuItem("Tools/Audio/Generate SoundClipData")]
    public static void OpenWindow()
    {
        GetWindow<SoundClipDataGenerator>("SoundClipData Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("SoundClipData 자동 생성기", EditorStyles.boldLabel);
        GUILayout.Space(5);
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("사운드 폴더", targetFolder, typeof(DefaultAsset), false);

        if (GUILayout.Button("생성하기"))
        {
            GenerateSoundClipData();
        }
    }

    private void GenerateSoundClipData()
    {
        if (targetFolder == null)
        {
            Debug.LogWarning("대상 폴더를 선택하세요.");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            if (!Enum.TryParse<SoundID>(filename, true, out var parsedID))
            {
                Debug.LogWarning($"SoundID에 '{filename}' 항목이 없음 → 건너뜀");
                continue;
            }

            string savePath = $"{folderPath}/{filename}.asset";
            if (File.Exists(savePath))
            {
                Debug.Log($"이미 존재함: {filename}.asset → 건너뜀");
                continue;
            }

            SoundClipData newSO = ScriptableObject.CreateInstance<SoundClipData>();
            newSO.soundID = parsedID;
            newSO.clip = clip;

            AssetDatabase.CreateAsset(newSO, savePath);
            Debug.Log($"생성 완료: {savePath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("모든 SoundClipData 생성 완료");
    }
}
