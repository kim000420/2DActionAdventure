using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    private string targetSpawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToScene(string sceneName, string spawnPoint)
    {
        targetSpawnPoint = spawnPoint;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    public void RespawnToScene(string sceneName, string returnSpawnID)
    {
        targetSpawnPoint = returnSpawnID;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[SceneTransitionManager] 플레이어를 찾지 못했습니다.");
            return;
        }

        // 스폰 포인트 처리
        SpawnPoint[] points = GameObject.FindObjectsOfType<SpawnPoint>();
        foreach (var point in points)
        {
            if (point.spawnID == targetSpawnPoint)
            {
                player.transform.position = point.transform.position;
                // 느낌표 갱신
                GameEventManager.Instance?.RefreshAllExclamations();
                Debug.Log($"[Spawn] '{targetSpawnPoint}' 위치로 이동 완료");
                break;
            }
        }

        // 카메라 타겟 재설정
        CameraController cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.SetTarget(player.transform);
            cam.FollowOn();
        }
        else
        {
            Debug.LogWarning("[SceneTransitionManager] Main Camera 또는 CameraController를 찾지 못했습니다.");
        }

        // UI HUD 연결
        var stats = player.GetComponent<PlayerStats>();
        var skill = player.GetComponent<PlayerSkillController>();
        var hud = FindObjectOfType<PlayerHUDManager>();
        if (hud != null && stats != null && skill != null)
        {
            hud.SetPlayer(stats, skill);
        }
    }
}
