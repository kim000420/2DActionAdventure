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
            // Idle �ִϸ��̼� ���
            controller.animator.Play($"{controller.monsterName}_Idle");
        }

        public override void Execute()
        {
            // ���� ��Ÿ���� �ƴ϶�� �ٷ� AttackState�� ��ȯ
            if (!controller.isAttackCooldown)
            {
                controller.ChangeState(new GroundfishAttackState(controller));
            }
            // ��Ÿ�� ���̶�� �ƹ��͵� ���� �ʰ� Idle ����
        }

        public override void Exit()
        {

        }
    }
}