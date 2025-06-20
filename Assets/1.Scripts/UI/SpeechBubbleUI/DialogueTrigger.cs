using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionalDialogue
{
    public string conditionKey;    // 조건 키 (ex: "QuestA_Completed")
    public string dialoguePath;    // Resources 내 JSON 경로
}
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("조건별 대사 설정")]
    [SerializeField] private List<ConditionalDialogue> conditionalDialogues;

    public void Interact(PlayerStateController controller)
    {
        string path = null;

        // ✅ 조건에 맞는 대사 탐색
        foreach (var entry in conditionalDialogues)
        {
            if (!string.IsNullOrEmpty(entry.conditionKey) &&
                DialogueEventManager.Instance.GetFlag(entry.conditionKey))
            {
                path = entry.dialoguePath;
                break;
            }
        }

        // ✅ 조건을 만족하지 못한 경우 fallback
        if (string.IsNullOrEmpty(path))
        {
            var fallback = conditionalDialogues.Find(e => e.conditionKey == "Default");
            if (fallback != null)
            {
                path = fallback.dialoguePath;
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
        ui.onDialogueEnd = () => controller.RequestStateChange(PlayerState.Idle);
        controller.RequestStateChange(PlayerState.Interacting);
    }
}
