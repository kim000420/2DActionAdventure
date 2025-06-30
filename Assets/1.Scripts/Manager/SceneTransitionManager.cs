using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{

    [Header("현재 리스폰 위치 상태")]
    public string currentSceneName = "Home";
    public string currentSpawnId = "default_spawn";

    [Header("화면 암전시 UI 이미지")]
    [SerializeField] private CanvasGroup fadeCanvas;
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
    }
    
    // 리스폰 갱신
    public void SetRespawnLocation(string sceneName, string spawnId)
    {
        currentSceneName = sceneName;
        currentSpawnId = spawnId;
        Debug.Log($"[Respawn] 위치 갱신 → {sceneName} / {spawnId}");
    }
    public void RespawnToLastSavedPoint()
    {
        RespawnToScene(currentSceneName, currentSpawnId);
    }

    public void TransitionToScene(string sceneName, string spawnPoint)
    {
        targetSpawnPoint = spawnPoint;
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    public void RespawnToScene(string sceneName, string returnSpawnID)
    {
        targetSpawnPoint = returnSpawnID;
        StartCoroutine(TransitionCoroutine(sceneName));
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
    private IEnumerator TransitionCoroutine(string sceneName)
    {
        yield return StartCoroutine(FadeOut(0.3f)); // 암전
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
        yield return null; // 한 프레임 대기 후 → OnSceneLoaded 호출됨
        yield return StartCoroutine(FadeIn(0.3f)); // 암전 해제
    }
    public IEnumerator FadeOut(float duration = 0.3f)
    {
        fadeCanvas.gameObject.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }
        fadeCanvas.alpha = 1f;
    }

    public IEnumerator FadeIn(float duration = 0.3f)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, time / duration);
            yield return null;
        }
        fadeCanvas.alpha = 0f;
        fadeCanvas.gameObject.SetActive(false);
    }
}
