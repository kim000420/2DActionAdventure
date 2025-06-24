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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[AutoTriggerZone] 충돌 감지됨: {other.name}");
        if (!other.CompareTag("Player"))
        {
            Debug.Log("[AutoTriggerZone] 태그가 Player가 아님 → 무시됨");
            return;
        }

        if (alreadyTriggered)
        {
            Debug.Log("[AutoTriggerZone] 이미 트리거됨 → 무시됨");
            return;
        }

        if (GameManager.Instance.IsDialoguePlaying)
        {
            Debug.Log("[AutoTriggerZone] 대화 중 상태이므로 실행하지 않음");
            return;
        }


        string current = GameEventManager.Instance.GetCurrentStoryStage();
        Debug.Log($"[AutoTriggerZone] 현재 스토리: {current}, 요구 스토리: {storyStage}");

        if (current == storyStage)
        {
            if (dialogueTrigger == null)
            {
                Debug.LogError("[AutoTriggerZone] DialogueTrigger가 연결되지 않았습니다!");
                return;
            }

            Debug.Log("[AutoTriggerZone] 조건 일치 → 대화 실행 시도");
            dialogueTrigger.TryStartDialogue();

            if (triggerOnlyOnce)
            {
                alreadyTriggered = true;
                Debug.Log("[AutoTriggerZone] 트리거 1회 설정이므로, 재실행 방지됨");
            }
        }
        else
        {
            Debug.Log("[AutoTriggerZone] 스토리 조건 불일치 → 대화 실행 안 함");
        }
    }
}
