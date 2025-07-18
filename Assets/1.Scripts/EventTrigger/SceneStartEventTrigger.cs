﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 🎯 SceneStartEventTrigger.cs
/// - 씬이 시작되면 GameEventManager의 현재 스토리 상태를 확인하고
/// - 조건에 맞는 이벤트 ID를 DialogueEventManager에 전달해 실행합니다.
///
/// ✅ 사용 방법:
/// - 씬 안에 빈 GameObject를 만들고 이 스크립트를 부착
/// - Inspector에서 조건-이벤트 쌍(SceneEventEntry[])을 설정
///
/// ✅ 필요 매니저:
/// - GameEventManager (스토리 상태 확인)
/// - DialogueEventManager (이벤트 실행)
///
/// ✅ 예시:
/// - StoryStage: "Story2_Started"
/// - eventId: "NPC_AlarmTrigger" → DialogueEventManager.Trigger("NPC_AlarmTrigger") 호출
/// </summary>
public class SceneStartEventTrigger : MonoBehaviour
{
    [System.Serializable]
    public class SceneEventEntry
    {
        public string storyStage;   // 조건 예: "Story2_Started"
        public string eventId;              // 실행할 트리거 ID
        public float delaySeconds = 0.5f; // 딜레이 시간
    }

    [Header("스토리 조건별 시작 이벤트")]
    public SceneEventEntry[] sceneEvents;

    private void Start()
    {
        RegisterBossesInScene();
        StartCoroutine(TriggerSceneEventWithDelay());
    }
    private void RegisterBossesInScene()
    {
        BossManager.Instance.ClearBosses();

        var bosses = Resources.FindObjectsOfTypeAll<BossComponent>();

        foreach (var boss in bosses)
        {
            if (!boss.gameObject.scene.IsValid()) continue; // 프리팹 제외
            string id = boss.GetBossId();
            BossManager.Instance.RegisterBoss(id, boss.gameObject);
        }

        Debug.Log($"[SceneStart] {bosses.Length}명의 보스 등록 완료");
    }
    private IEnumerator TriggerSceneEventWithDelay()
    {
        string currentStage = GameEventManager.Instance.GetCurrentStoryStage();

        foreach (var entry in sceneEvents)
        {
            if (entry.storyStage == currentStage)
            {
                if (entry.delaySeconds > 0)
                    yield return new WaitForSeconds(entry.delaySeconds);

                Debug.Log($"[SceneEvent] {entry.eventId} 실행됨 (조건: {entry.storyStage}, 딜레이: {entry.delaySeconds}s)");
                DialogueEventManager.Instance?.Trigger(entry.eventId);
                break;
            }
        }

        // 느낌표 갱신
        GameEventManager.Instance?.RefreshAllExclamations();
    }
}
