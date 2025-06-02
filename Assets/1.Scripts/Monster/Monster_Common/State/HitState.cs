using UnityEngine;
using Monster.States;

namespace Monster.CommonStates
{
    public class HitState : IMonsterState
    {
        private MonsterStateController controller;

        public HitState(MonsterStateController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.isAttacking = false;
            controller.animator.Play($"{controller.monsterName}_Hit");
        }

        public void Execute()
        {
            // Hit �ִϸ��̼��� ������ Idle�� �����ϴ� ����� �Ϲ���
            // ���⼭�� Animator �̺�Ʈ�� ���� ���� ���̸� Ʈ�����ϰų�,
            // ���� �ð� �� �����ϵ��� ������ �� ����
        }

        public void Exit()
        {
            // �ʿ� �� �ǰ� ȿ�� ���� ó�� ����
        }
    }
}
