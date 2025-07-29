using UnityEngine;

// 개별 AudioClip과 그에 대응하는 SoundID를 저장하는 ScriptableObject
[CreateAssetMenu(menuName = "Audio/SoundClipData")]
public class SoundClipData : ScriptableObject
{
    public SoundID soundID;
    public AudioClip clip;
}
