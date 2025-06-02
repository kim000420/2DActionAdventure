using UnityEngine;
using TutorialBoss.Controller;
using System.Collections;

namespace TutorialBoss.States.Bow
{
    public class BowAttackState : BaseTutorialBossState
    {
        public BowAttackState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.rb.velocity = Vector2.zero;
            controller.FaceToPlayer();
            controller.animator.Play($"{controller.bossName}_Attack");
        }

        public override void Execute()
        {
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;
        }

        public override void Exit()
        {
            // 공격 중단 로직 (필요하다면)
        }
    }
}