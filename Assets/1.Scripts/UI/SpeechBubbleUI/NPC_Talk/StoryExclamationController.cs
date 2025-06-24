// 파일명: StoryExclamationController.cs

using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

/// <summary>
/// 🎯 StoryExclamationController.cs
/// - 각 NPC에 부착되어 스토리 진행 상태에 따라 느낌표 아이콘을 활성화/비활성화합니다.
/// - GameEventManager.RefreshAllExclamations()에 의해 갱신됩니다.
/// </summary>
public class StoryExclamationController : MonoBehaviour
{
    [Header("느낌표 아이콘 설정")]
    [SerializeField] public GameObject exclamationIcon; // 활성화/비활성화할 느낌표 아이콘 오브젝트

    [Header("느낌표 표시 조건")]
    [Tooltip("느낌표가 활성화될 스토리 단계 목록입니다. 비어있거나 'Default'를 포함하면 항상 활성화됩니다.")]
    [SerializeField] private List<string> requiredStoryStages; // ✅ List<string>으로 변경

    [Tooltip("이 NPC가 대화를 진행 중일 때는 느낌표를 비활성화합니다. DialogueTrigger와 연동됩니다.")]
    [SerializeField] private DialogueTrigger dialogueTrigger; // 해당 NPC의 DialogueTrigger 참조 (선택 사항)

    private void Awake()
    {
        // 느낌표 아이콘이 할당되지 않았다면 경고
        if (exclamationIcon == null)
        {
            Debug.LogWarning($"[StoryExclamationController] {gameObject.name}에 느낌표 아이콘이 할당되지 않았습니다.", this);
        }

        // DialogueTrigger가 할당되지 않았다면 같은 오브젝트에서 찾기 시도
        if (dialogueTrigger == null)
        {
            dialogueTrigger = GetComponent<DialogueTrigger>();
        }

        // List가 null일 경우 초기화 (에디터에서 초기화되지 않은 경우 대비)
        if (requiredStoryStages == null)
        {
            requiredStoryStages = new List<string>();
        }
    }

    /// <summary>
    /// 현재 스토리 단계에 따라 느낌표 아이콘을 갱신합니다.
    /// 이 함수는 GameEventManager에 의해 호출됩니다.
    /// </summary>
    public void RefreshExclamationState()
    {
        if (exclamationIcon == null) return;

        string currentStage = GameEventManager.Instance.GetCurrentStoryStage();

        // 1. 스토리 조건 확인
        bool isStoryConditionMet = false;
        if (requiredStoryStages.Count == 0 || requiredStoryStages.Contains("Default"))
        {
            // requiredStoryStages가 비어있거나 "Default"를 포함하면,
            // 모든 스토리 단계에서 느낌표가 표시될 수 있다는 의미 (항상 활성화)
            isStoryConditionMet = true;
        }
        else
        {
            // requiredStoryStages 목록에 현재 스토리 단계가 포함되어 있는지 확인
            isStoryConditionMet = requiredStoryStages.Contains(currentStage);
        }

        // 2. 대화 진행 중인지 확인 (DialogueTrigger가 연결되어 있고 현재 대화 중일 경우)
        bool isDialoguePlaying = false;
        if (dialogueTrigger != null)
        {
            isDialoguePlaying = dialogueTrigger.IsPlaying();
        }

        // 최종 느낌표 활성화 여부 결정
        bool shouldBeActive = isStoryConditionMet && !isDialoguePlaying;

        exclamationIcon.SetActive(shouldBeActive);
        Debug.Log($"[Exclamation] {gameObject.name} Exclamation State: {shouldBeActive} (Current Story: {currentStage}, Required Stages: {string.Join(", ", requiredStoryStages)}, DialoguePlaying: {isDialoguePlaying})");
    }
}