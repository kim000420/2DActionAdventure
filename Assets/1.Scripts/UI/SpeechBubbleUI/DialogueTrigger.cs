using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("JSON 대사 파일 경로 (Resources 폴더 기준)")]
    [SerializeField] private string dialogueFilePath;

    public void Interact(PlayerStateController controller)
    {
        var json = Resources.Load<TextAsset>(dialogueFilePath);
        if (json == null)
        {
            Debug.LogError($"[DialogueTrigger] 파일 '{dialogueFilePath}' 을(를) 찾을 수 없습니다.");
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);
        if (data == null || data.lines == null || data.lines.Length == 0)
        {
            Debug.LogWarning("[DialogueTrigger] 대화 내용이 비어 있음.");
            return;
        }

        var ui = GameManager.Instance.GetComponent<DialogueUIManager>();
        ui.StartDialogue(data.lines);
        ui.onDialogueEnd = () =>
        {
            controller.RequestStateChange(PlayerState.Idle);
        };

        controller.RequestStateChange(PlayerState.Interacting);
    }
}
