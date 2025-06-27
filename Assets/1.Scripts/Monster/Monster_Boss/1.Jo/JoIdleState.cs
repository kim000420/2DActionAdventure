using TutorialBoss.Controller;
using UnityEngine;

namespace TutorialBoss.States.Jo
{
    public class JoIdleState : BaseTutorialBossState
    {
        private TutorialBossStats stats;

        public JoIdleState(TutorialBossStateController controller) : base(controller)
        {
            stats = controller.GetComponent<TutorialBossStats>();
        }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_Idle");
        }

        public override void Execute()
        {
            // 보스의 죽음, 그로기, 피격 경직 상태에서는 상태 전이 로직을 실행하지 않습니다.
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;
            
            float distance = Vector2.Distance(controller.transform.position, controller.player.position);

            if (distance <= stats.detectRange && !controller.isAttackCooldown)
            {
                controller.ChangeState(new JoChaseState(controller));
            }
            else
            {
                // Idle 애니메이션 반복 유지
                controller.animator.Play($"{controller.bossName}_Idle");
            }
        }

        public override void Exit()
        {
            // 아무것도 하지 않음
        }
    }
}
