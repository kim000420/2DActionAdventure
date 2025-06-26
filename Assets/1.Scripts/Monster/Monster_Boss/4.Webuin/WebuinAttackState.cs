using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using TutorialBoss.Controller;
using TutorialBoss.States; // BaseTutorialBossState 참조

namespace TutorialBoss.States.Webuin
{
    public class WebuinAttackState : BaseTutorialBossState
    {
        public WebuinAttackState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            if (!controller.isAttackCooldown)
            {
                controller.animator.Play("Webuin_Attack");
            }
            else
            {
                // 쿨타임 중이라면 즉시 ChaseState로 전환 (원치 않는 공격 방지)
                controller.ChangeState(new WebuinChaseState(controller));
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