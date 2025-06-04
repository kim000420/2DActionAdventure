using UnityEngine;
using TutorialBoss.Controller;
using Monster.CommonStates;
using TutorialBoss.States.Dok2;

namespace TutorialBoss.States
{
    public class GroggyState : BaseTutorialBossState
    {
        public GroggyState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.isGroggy = true;
            controller.animator.Play($"{controller.bossName}_Groggy");

            controller.rb.velocity = Vector2.zero;
            controller.rb.bodyType = RigidbodyType2D.Kinematic;
        }

        public override void Execute()
        {
            // OnGroggyEnd() 같은 외부 호출로 상태 전이
        }

        public override void Exit()
        {
            controller.isGroggy = false;
            controller.rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
