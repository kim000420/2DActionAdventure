using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueConditionEntry
{
    public string storyStage;      // 예: "Story1", "Default"
    public string talkContext;     // 예: "QuestDeclinedOnce", "Default"
    public string dialoguePath;    // Resources 내 JSON 경로
}
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("조건별 대사 설정")]
    [SerializeField] private List<DialogueConditionEntry> conditionTable;

    public void Interact(PlayerStateController controller)
    {
        string path = null;

        string currentStoryStage = GameEventManager.Instance.GetCurrentStoryStage();  // 구현 필요
        string currentTalkContext = DialogueEventManager.Instance.GetCurrentTalkContext(); // 구현 필요

        // ✅ 조건에 맞는 대사 탐색
        foreach (var entry in conditionTable)
        {
            bool storyMatch = (entry.storyStage == currentStoryStage || entry.storyStage == "Default");
            bool contextMatch = (entry.talkContext == currentTalkContext || entry.talkContext == "Default");

            if (storyMatch && contextMatch)
            {
                path = entry.dialoguePath;
                break;
            }
        }

        // 오류 디버그 
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[DialogueTrigger] 유효한 대사 경로를 찾을 수 없습니다.");
            return;
        }

        var json = Resources.Load<TextAsset>(path);
        if (json == null)
        {
            Debug.LogError($"[DialogueTrigger] 파일 '{path}' 을(를) 찾을 수 없습니다.");
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);
        var ui = GameManager.Instance.GetComponent<DialogueUIManager>();

        ui.StartDialogue(data.lines, this.transform);
        ui.SetOnEndEvent(data.onEndEvent);
        ui.onDialogueEnd = () => controller.RequestStateChange(PlayerState.Idle);
        controller.RequestStateChange(PlayerState.Interacting);
    }
}
