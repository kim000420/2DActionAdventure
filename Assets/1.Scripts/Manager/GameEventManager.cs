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

    //  현재 스토리 상태 조회
    public string GetCurrentStoryStage()
    {
        return currentStoryStage;
    }

    //  스토리 상태 설정 (ex: "Story1", "Story2_Started")
    public void SetCurrentStoryStage(string stage)
    {
        currentStoryStage = stage;
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
}
