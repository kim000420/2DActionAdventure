using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("전역 참조")]
    public SceneTransitionManager sceneTransitionManager;

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

        // 필수 매니저 구성 확인
        EnsureSceneTransitionManager();
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

    // 테스트용: 게임 강제 리스폰 트리거
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
