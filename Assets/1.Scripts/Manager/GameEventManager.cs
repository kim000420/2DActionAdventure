using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전역 상태(스토리 진행, 보스 클리어 등)를 관리하는 매니저
/// </summary>
public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    [SerializeField]
    public string currentStoryStage = "Default"; // ✅ 현재 스토리 진행 상태

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadState();

        if (string.IsNullOrEmpty(currentStoryStage) || currentStoryStage == "Default")
        {
            SetCurrentStoryStage("ST_intro");
            Debug.Log("[GameEvent] 초기 스토리 상태 설정됨: ST_intro");
        }
    }


    //  현재 스토리 상태 조회
    public string GetCurrentStoryStage()
    {
        return currentStoryStage;
    }

    //  스토리 상태 설정 (ex: "Story1", "Story2_Started")
    public void SetCurrentStoryStage(string stage)
    {
        currentStoryStage = stage;
        RefreshAllExclamations();
    }

    //  특정 이벤트 플래그 저장
    public void SetFlag(string key, bool value)
    {
        flags[key] = value;
    }

    //  특정 이벤트 플래그 조회
    public bool GetFlag(string key)
    {
        return flags.TryGetValue(key, out bool value) && value;
    }

    //  예시: 이벤트 토글
    public void ToggleFlag(string key)
    {
        SetFlag(key, !GetFlag(key));
    }

    // 정수 저장
    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    // 정수 불러오기
    public int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key, 0); // 기본값 0
    }


    //  저장
    public void SaveState()
    {
        PlayerPrefs.SetString("CurrentStoryStage", currentStoryStage);
        // 선택적으로 flags 저장 구현 가능
        PlayerPrefs.Save();
    }

    //  불러오기
    public void LoadState()
    {
        currentStoryStage = PlayerPrefs.GetString("CurrentStoryStage", "Default");
        // 선택적으로 flags 불러오기 구현 가능
    }

    public void RefreshAllExclamations()
    {
        // 씬의 모든 StoryExclamationController 컴포넌트를 찾음
        // 주의: 씬에 오브젝트가 많을 경우 성능에 영향을 줄 수 있으나,
        // 느낌표는 일반적으로 NPC에만 부착되므로 큰 문제 없을 것입니다.
        StoryExclamationController[] controllers = FindObjectsOfType<StoryExclamationController>();
        foreach (StoryExclamationController controller in controllers)
        {
            controller.RefreshExclamationState();
        }
        Debug.Log("[GameEvent] 모든 NPC 느낌표 상태 갱신 완료.");
    }
}
