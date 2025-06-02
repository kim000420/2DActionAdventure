using UnityEngine;
using Monster.States;

namespace Monster.CommonStates
{
    public class DieState : IMonsterState
    {
        private MonsterStateController controller;

        public DieState(MonsterStateController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.animator.Play($"{controller.monsterName}_Die");
            Object.Destroy(controller.gameObject, 2f); // 2�� �� ����
        }

        public void Execute()
        {
            // �ƹ� �͵� ���� ����
        }

        public void Exit()
        {
            // ��� ���´� Exit ȣ����� ���� �� ���� (�����)
        }
    }
}
