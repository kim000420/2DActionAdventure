using UnityEngine;

/// <summary>
/// 🎯 AutoTriggerZone.cs
/// - 플레이어가 Trigger Collider에 들어오면 자동으로 DialogueTrigger를 실행합니다.
/// - 특정 스토리 키/값이 조건에 맞을 때만 실행됩니다.
///
/// ✅ 사용 방법:
/// - 투명한 콜라이더를 가진 오브젝트(예: TriggerZone)에 부착
/// - DialogueTrigger 컴포넌트를 연결
///
/// ✅ 필요 사항:
/// - GameObject에 Collider 컴포넌트 추가 + "IsTrigger" 체크
/// - 플레이어는 "Player" 태그를 가짐
/// - GameEventManager에 조건 키/값 사전 등록 필요
///
/// ✅ 주의:
/// - triggerOnlyOnce가 true일 경우 한 번만 발동합니다.
/// - GameEventManager.Instance.GetInt(key) 로 값 체크함
/// </summary>
public class AutoTriggerZone : MonoBehaviour
{
    [Header("조건 설정")]
    public string storyStage = "Story_Progress";  // 진행상황 키
    public bool triggerOnlyOnce = true;

    private bool alreadyTriggered = false;

    [Header("대화 트리거")]
    public DialogueTrigger dialogueTrigger;  // 연결된 DialogueTrigger

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (alreadyTriggered) return;

        string current = GameEventManager.Instance.GetCurrentStoryStage();

        if (current == storyStage)
        {
            dialogueTrigger.TryStartDialogue();
            if (triggerOnlyOnce)
                alreadyTriggered = true;
        }
    }
}
