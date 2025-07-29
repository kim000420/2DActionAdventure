using UnityEngine;

// 사운드 재생 전담 싱글톤 매니저
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    [Header("Sound Library")]
    [SerializeField] private SoundLibrary soundLibrary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        soundLibrary.Initialize();
    }

    public void PlaySFX(SoundID id)
    {
        var clip = soundLibrary.GetClip(id);
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(SoundID id, bool loop = true)
    {
        var clip = soundLibrary.GetClip(id);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM() => bgmSource.Stop();
}
