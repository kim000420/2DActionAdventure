using UnityEngine;
using Monster.States;

namespace Monster.CommonStates
{
    public class AttackState : IMonsterState
    {
        private MonsterStateController controller;

        public AttackState(MonsterStateController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.isAttacking = true;

            // ���� �ִϸ��̼� ��� (��: Goblin_Attack)
            controller.animator.Play($"{controller.monsterName}_Attack");
        }

        public void Execute()
        {
            // ���� �߿��� ���� ����
            // �ʿ� �� controller.FaceToPlayer() ���� ����
        }

        public void Exit()
        {
            controller.isAttacking = false;
        }
    }
}
