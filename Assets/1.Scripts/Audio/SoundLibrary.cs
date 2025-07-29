using UnityEngine;
using System.Collections.Generic;

// 여러 개의 SoundClipData를 모아 관리하는 ScriptableObject
[CreateAssetMenu(menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundClipData> soundClipList;

    private Dictionary<SoundID, AudioClip> clipDict;

    public void Initialize()
    {
        clipDict = new Dictionary<SoundID, AudioClip>();
        foreach (var data in soundClipList)
        {
            if (data != null && !clipDict.ContainsKey(data.soundID))
                clipDict.Add(data.soundID, data.clip);
        }
    }

    public AudioClip GetClip(SoundID id)
    {
        if (clipDict == null) Initialize();
        return clipDict.TryGetValue(id, out var clip) ? clip : null;
    }
}
