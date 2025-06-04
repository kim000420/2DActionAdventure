using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States;

namespace CommonMonster.States.Groundfish
{
    public class GroundfishIdleState : BaseMonsterState
    {
        public GroundfishIdleState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            // Idle 애니메이션 재생
            controller.animator.Play($"{controller.monsterName}_Idle");
        }

        public override void Execute()
        {
            // 공격 쿨타임이 아니라면 바로 AttackState로 전환
            if (!controller.isAttackCooldown)
            {
                controller.ChangeState(new GroundfishAttackState(controller));
            }
            // 쿨타임 중이라면 아무것도 하지 않고 Idle 유지
        }

        public override void Exit()
        {

        }
    }
}