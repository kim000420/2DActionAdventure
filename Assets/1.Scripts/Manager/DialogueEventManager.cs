using UnityEngine;

/// <summary>
/// 대화 내 선택지 트리거 및 연출 이벤트 전용 매니저
/// 게임 전역 상태는 GameEventManager에서 관리
/// </summary>
public class DialogueEventManager : MonoBehaviour
{
    public static DialogueEventManager Instance;

    [SerializeField]
    private string currentTalkContext = "Default"; // ✅ 대화 컨텍스트 상태 (조건 분기에 사용)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ 현재 대화 컨텍스트 조회
    public string GetCurrentTalkContext()
    {
        return currentTalkContext;
    }

    // ✅ 대화 컨텍스트 설정
    public void SetTalkContext(string contextKey)
    {
        currentTalkContext = contextKey;
    }

    // ✅ 트리거 실행
    public void Trigger(string eventId)
    {
        Debug.Log($"[DialogueEvent] Triggered: {eventId}");
        
        switch (eventId)
        {
            case "Start_intro":
                var target = GameObject.Find("StartTriggerObj");
                var trigger = target.GetComponent<DialogueTrigger>();
                trigger?.TryStartDialogue();
                break;
            case "Start_intro_end":
                GameEventManager.Instance.SetCurrentStoryStage("Start_intro_end");
                break;

            case "QuestStart":
                GameEventManager.Instance.SetFlag("Quest_Started", true);
                SetTalkContext("QuestAccepted");
                break;

            case "QuestDeclinedOnce":
                SetTalkContext("QuestDeclinedOnce");
                break;

            case "ExploreSkip":
                SetTalkContext("ExploreRefused");
                break;

        }
    }
}
