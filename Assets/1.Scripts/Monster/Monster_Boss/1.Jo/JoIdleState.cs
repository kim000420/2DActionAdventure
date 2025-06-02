using UnityEngine;
using TutorialBoss.Controller;

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
            if (controller.isDead || controller.isGroggy)
                return;

            float distance = Vector2.Distance(controller.transform.position, controller.player.position);

            if (distance <= stats.detectRange && !controller.isAttackCooldown)
            {
                controller.ChangeState(new JoChaseState(controller));
            }
            else
            {
                // Idle �ִϸ��̼� �ݺ� ����
                controller.animator.Play($"{controller.bossName}_Idle");
            }
        }

        public override void Exit()
        {
            // �ƹ��͵� ���� ����
        }
    }
}
