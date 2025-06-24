using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsDialoguePlaying { get; private set; }
    public void SetDialogueState(bool isPlaying)
    {
        IsDialoguePlaying = isPlaying;
    }

    [Header("전역 참조")]
    public SceneTransitionManager sceneTransitionManager;

    private GameObject player;

    private void Awake()
    {
        // 싱글톤 구성
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        EnsureSceneTransitionManager();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPlayerReference();
    }

    private void EnsureSceneTransitionManager()
    {
        if (sceneTransitionManager == null)
        {
            sceneTransitionManager = GetComponentInChildren<SceneTransitionManager>();
            if (sceneTransitionManager == null)
                Debug.LogWarning("[GameManager] SceneTransitionManager가 연결되어 있지 않습니다.");
        }
    }

    // ✅ Player 등록 (플레이어가 직접 호출할 수 있음)
    public void RegisterPlayer(GameObject playerObj)
    {
        this.player = playerObj;
    }

    // ✅ Player 자동 탐색 (태그 기반 또는 이름 기반)
    public void RefreshPlayerReference()
    {
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("[GameManager] 'Player' 태그를 가진 오브젝트를 찾지 못했습니다.");
        }
    }

    // ✅ Player에 붙은 컴포넌트를 안전하게 가져오기
    public T GetPlayerComponent<T>() where T : Component
    {
        return player != null ? player.GetComponent<T>() : null;
    }

    // ✅ 강제 리스폰 예제
    public void ForceRespawn(string sceneName, string spawnID)
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.RespawnToScene(sceneName, spawnID);
        }
        else
        {
            Debug.LogError("[GameManager] SceneTransitionManager가 설정되지 않았습니다.");
        }
    }
}
