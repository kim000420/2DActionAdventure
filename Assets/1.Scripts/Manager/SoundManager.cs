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
    public SoundID CurrentBGM { get; private set; } // 현재 재생중인 BGM 저장
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SoundManager 생성됨");
        }
        else
        {
            Debug.LogWarning("중복된 SoundManager가 생성되어 파괴됨");
            Destroy(gameObject);
        }

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

            if (bgmSource.isPlaying && bgmSource.clip == clip) // 중첩방지
            {
                Debug.Log($"[SoundManager] 같은 BGM({id}) 재생 중 → 무시");
                return;
            }

            bgmSource.Stop(); // 기존 재생 중단
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();

            CurrentBGM = id; // 현재 재생중인 BGM 등록
            Debug.Log($"[SoundManager] BGM 재생 시작: {id}");
        }
    }

    public void StopBGM() => bgmSource.Stop();
}
