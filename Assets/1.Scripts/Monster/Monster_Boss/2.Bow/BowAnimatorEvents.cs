using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;
using TutorialBoss.States.Bow;

namespace TutorialBoss.AnimEvents
{
    public class BowAnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;

        // 활 발사 타이밍 순간
        public void ShootBowArrow()
        {
            Vector2 startPosition = controller.bowShootPoint.position;
            Vector2 shotDirection = Quaternion.Euler(0, 0, controller.bowShotAngle) * Vector2.right;
            shotDirection.x *= Mathf.Sign(controller.transform.localScale.x);
            // TODO: 실제 화살 Prefab 인스턴스화 로직
            GameObject arrowGO = Instantiate(controller.arrowPrefab, startPosition, Quaternion.identity);
            Arrow arrowScript = arrowGO.GetComponent<Arrow>();

            if (arrowScript != null)
            {
                // ⭐ 화살 발사
                arrowScript.Fire(shotDirection, controller.transform.localScale.x);
            }
            else
            {
                Debug.LogWarning("[ShootBow] Arrow prefab missing Arrow script!");
            }
        }

        // 공격 애니메이션이 끝났을 때
        public void OnBowAttackEnd()
        {
            // 공격 쿨타임을 시작하고 다음 상태로 전이
            controller.StartAttackCooldown(controller.bowJumpCooldown);

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            if (distanceToPlayer <= controller.bossStats.escapeTriggerRange)
            {
                controller.ChangeState(new BowEscapeState(controller));
            }
            else
            {
                controller.ChangeState(new BowAttackState(controller));
            }
        }
        // 점프 시작 애니메이션 종료 이벤트
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