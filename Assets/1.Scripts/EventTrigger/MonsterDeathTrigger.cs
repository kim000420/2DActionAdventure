using UnityEngine;

/// <summary>
/// 🎯 MonsterDeathTrigger.cs
/// - 몬스터 사망 시 특정 스토리 상태일 경우 지정된 이벤트를 발생시킵니다.
///
/// ✅ 사용 방법:
/// - 몬스터 프리팹에 이 스크립트를 부착
/// - MonsterController/EnemyAI 등의 사망 코드에서 OnMonsterDie() 호출
///
/// ✅ 필요 매니저:
/// - GameEventManager (현재 스토리 상태 체크)
/// - DialogueEventManager (이벤트 실행)
///
/// ✅ 코드 삽입 위치 예시:
/// void Die() {
///     GetComponent<MonsterDeathTrigger>()?.OnMonsterDie();
///     Destroy(gameObject);
/// }
///
/// ✅ 주의:
/// - 중복 실행 방지를 위해 내부에서 자동으로 한 번만 실행됩니다.
/// </summary>
public class MonsterDeathTrigger : MonoBehaviour
{
    [Header("발동 조건")]
    public string requiredStoryStage = "Story3";

    [Header("사망 시 실행할 이벤트 ID")]
    public string onDeathEventId = "AfterMonsterDefeated";

    private bool hasTriggered = false;

    public void OnMonsterDie()
    {
        if (hasTriggered) return;

        string currentStage = GameEventManager.Instance.GetCurrentStoryStage();

        if (currentStage == requiredStoryStage)
        {
            Debug.Log($"[MonsterDeathTrigger] 스토리 조건 충족 → {onDeathEventId} 실행");
            DialogueEventManager.Instance?.Trigger(onDeathEventId);
            hasTriggered = true;
        }
    }
}
