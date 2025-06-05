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
            Debug.Log($"�׶����ǽ� ���̵� enter");
            // Idle �ִϸ��̼� ���
            controller.animator.Play($"{controller.monsterName}_Idle");
        }

        public override void Execute()
        {
            Debug.Log($"�׶����ǽ� ���̵� execute");
            // ���� ��Ÿ���� �ƴ϶�� �ٷ� AttackState�� ��ȯ
            if (!controller.isAttackCooldown)
            {
                Debug.Log($"�׶����ǽ� AttackState ����");
                controller.ChangeState(new GroundfishAttackState(controller));
            }
            // ��Ÿ�� ���̶�� �ƹ��͵� ���� �ʰ� Idle ����
        }

        public override void Exit()
        {

        }
    }
}