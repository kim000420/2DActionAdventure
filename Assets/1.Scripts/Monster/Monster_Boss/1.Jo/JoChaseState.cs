using UnityEngine;
using TutorialBoss.Controller;

namespace TutorialBoss.States.Jo
{
    public class JoChaseState : BaseTutorialBossState
    {
        private TutorialBossStats stats;

        public JoChaseState(TutorialBossStateController controller) : base(controller)
        {
            stats = controller.GetComponent<TutorialBossStats>();
        }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_Run");
        }

        public override void Execute()
        {
            if (controller.isDead || controller.isGroggy)
                return;

            float distance = Vector2.Distance(controller.transform.position, controller.player.position);

            if (distance <= stats.attackRange && !controller.isAttackCooldown && !controller.isHitRecovery)
            {
                controller.ChangeState(new JoAttackState(controller));
                return;
            }

            if (distance <= stats.detectRange)
            {
                Vector3 dir = (controller.player.position - controller.transform.position).normalized;

                controller.transform.position += dir * stats.moveSpeed * Time.deltaTime;

                // ✅ 좌측 애니메이션 기준 방향 보정
                if (dir.x != 0)
                {
                    Vector3 scale = controller.transform.localScale;
                    scale.x = dir.x > 0 ? -1f : 1f;
                    controller.transform.localScale = scale;
                }
            }
            else
            {
                controller.ChangeState(new JoIdleState(controller));
            }
        }

        public override void Exit()
        {
            // nothing
        }
    }
}
