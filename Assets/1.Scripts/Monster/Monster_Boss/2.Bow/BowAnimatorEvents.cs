using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;
using TutorialBoss.States.Bow;

namespace TutorialBoss.AnimEvents
{
    public class BowAnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;

        [Header("화살 발사 설정")]
        public GameObject arrowPrefab; // 유니티 에디터에서 화살 프리팹을 여기에 연결
        public float arrowAimHeightOffset = 0.5f; // 플레이어 위치에서 얼마나 위를 조준할지

        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<TutorialBossStateController>();
            }
        }
        public void ShootBowArrow()
        {
            if (arrowPrefab == null || controller.bowShootPoint == null || controller.player == null)
            {
                Debug.LogWarning("[BowAnimatorEvents] Arrow prefab, ShootPoint, or Player not set for arrow shot!");
                return;
            }

            Vector2 startPosition = controller.bowShootPoint.position;

            // ⭐ 변경: 플레이어의 위치를 기반으로 발사 방향 계산
            Vector2 targetPosition = (Vector2)controller.player.position + Vector2.up * arrowAimHeightOffset;
            Vector2 shotDirection = (targetPosition - startPosition).normalized;

            // ⭐ 변경: 화살을 인스턴스화하고 Arrow 스크립트의 Fire 메서드 호출
            GameObject arrowGO = Instantiate(arrowPrefab, startPosition, Quaternion.identity);
            Arrow arrowScript = arrowGO.GetComponent<Arrow>(); // Arrow.cs 스크립트 이름이 Arrow라고 가정

            if (arrowScript != null)
            {
                arrowScript.Fire(shotDirection, controller.transform.localScale.x);
            }
            else
            {
                Debug.LogWarning("[BowAnimatorEvents] Arrow prefab is missing the Arrow script!");
            }
            Debug.Log($"[BowAnimatorEvents] Arrow shot towards Player. Direction: {shotDirection}");
        }

        public void OnBowAttackEnd()
        {
            controller.StartAttackCooldown(controller.bowJumpCooldown);

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);
            Debug.Log($"[BowAnimatorEvents] 공격 종료. 거리: {distanceToPlayer}, 도망 트리거: {controller.bossStats.escapeTriggerRange}");

            if (distanceToPlayer <= controller.bossStats.escapeTriggerRange)
            {
                controller.ChangeState(new BowEscapeState(controller));
            }
            else
            {
                controller.ChangeState(new BowAttackState(controller));
            }
        }

        public void OnBowJumpEnd()
        {
            Debug.Log("[BowAnimatorEvents] Jump 애니메이션 종료.");
        }

        public void OnBowJumpStartEnd()
        {
            controller.animator.Play($"{controller.bossName}_Jump");

            if (controller.currentState is BowJumpState currentJumpState)
            {
                currentJumpState.OnJumpAnimationStarted();
            }
        }
    }
}