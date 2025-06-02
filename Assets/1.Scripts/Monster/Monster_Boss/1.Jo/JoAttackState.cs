using UnityEngine;
using TutorialBoss.Controller;

namespace TutorialBoss.States.Jo
{
    public class JoAttackState : BaseTutorialBossState
    {
        public JoAttackState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            float roll = Random.value; // 0.0f ~ 1.0f

            if (roll < 0.5f)
            {
                controller.animator.Play($"{controller.bossName}_Attack1");
            }
            else
            {
                controller.animator.Play($"{controller.bossName}_Attack2");
            }

        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
        }
    }
}
