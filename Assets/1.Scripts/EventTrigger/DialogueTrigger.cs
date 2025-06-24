using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 🎯 DialogueTrigger.cs
/// - 조건에 따라 대사를 실행하는 핵심 트리거 스크립트입니다.
/// - 스토리 상태 + 대화 컨텍스트 기준으로 대사 JSON을 로드합니다.
///
/// ✅ 사용 방법:
/// - NPC나 상호작용 대상 오브젝트에 부착
/// - Inspector에서 조건별 대사(`conditionTable`) 설정
/// - IInteractable 인터페이스 구현 (플레이어와 상호작용할 때 실행됨)
///
/// ✅ 필요 컴포넌트/매니저:
/// - GameManager (싱글톤)
/// - DialogueEventManager (컨텍스트 상태 참조)
/// - GameEventManager (스토리 상태 참조)
///
/// ✅ 외부 호출용 함수:
/// - TryStartDialogue() → 외부에서 강제 실행 가능 (트리거 존 등에서 사용)
/// </summary>

[System.Serializable]
public class DialogueConditionEntry
{
    [Header("조건 - 현재 스토리 진행상황")]
    public string storyStage;      // 예: "Story1", "Default"
    [Header("선택지 선택 상황 ")]
    public string talkContext;     // 예: "QuestDeclinedOnce", "Default"
    [Header("대사 json 경로")]
    public string dialoguePath = "Dialogue";    // Resources 내 JSON 경로
}

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("조건별 대사 설정")]
    [SerializeField] private List<DialogueConditionEntry> conditionTable;

    private bool isPlaying = false;

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void Interact(PlayerStateController controller)
    {
        TryStartDialogue();
    }

    public void TryStartDialogue()
    {
        if (isPlaying) return;

        string path = GetDialoguePathByCondition();
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[DialogueTrigger] 유효한 대사 경로를 찾지 못했습니다.");
            return;
        }

        TextAsset json = Resources.Load<TextAsset>(path);
        if (json == null)
        {
            Debug.LogError($"[DialogueTrigger] 경로 '{path}' 에 해당하는 JSON 파일을 찾을 수 없습니다.");
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);
        var ui = GameManager.Instance.GetComponent<DialogueUIManager>();
        var playerController = GameManager.Instance.GetPlayerComponent<PlayerStateController>();

        if (playerController == null)
        {
            Debug.LogError("[DialogueTrigger] PlayerStateController를 찾을 수 없습니다.");
            return;
        }

        ui.StartDialogue(data.lines, this.transform);
        ui.SetOnEndEvent(data.onEndEvent);

        playerController.RequestStateChange(PlayerState.Interacting);
        isPlaying = true;

        StoryExclamationController exclamationController = GetComponent<StoryExclamationController>();
        if (exclamationController != null)
        {
            exclamationController.gameObject.SetActive(false); // 느낌표 오브젝트 비활성화 (전체 ExclamationMark 오브젝트를 비활성화)
            Debug.Log($"[Exclamation] {gameObject.name} 대화 시작, 느낌표 비활성화.");
        }

        ui.onDialogueEnd = () =>
        {
            playerController.RequestStateChange(PlayerState.Idle);
            isPlaying = false;

            // 느낌표 갱신
            GameEventManager.Instance?.RefreshAllExclamations();
        };
    }

    private string GetDialoguePathByCondition()
    {
        string currentStoryStage = GameEventManager.Instance.GetCurrentStoryStage();
        string currentTalkContext = DialogueEventManager.Instance.GetCurrentTalkContext();

        foreach (var entry in conditionTable)
        {
            bool storyMatch = entry.storyStage == currentStoryStage || entry.storyStage == "Default";
            bool contextMatch = entry.talkContext == currentTalkContext || entry.talkContext == "Default";

            if (storyMatch && contextMatch)
                return entry.dialoguePath;
        }

        return null;
    }
}
