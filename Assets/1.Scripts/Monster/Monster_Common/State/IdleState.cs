using UnityEngine;
using Monster.States;

namespace Monster.CommonStates
{
    public class IdleState : IMonsterState
    {
        private MonsterStateController controller;

        public IdleState(MonsterStateController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.animator.Play($"{controller.monsterName}_Idle");
            controller.animator.SetFloat("Blend", 0f);
        }

        public void Execute()
        {
            controller.FaceToPlayer();

            if (controller.isDead || controller.isAttacking)
                return;

            switch (controller.GetPlayerDistanceType())
            {
                case PlayerDistanceType.OutOfAttack:
                    // controller.ChangeState(new ChaseState(controller)); // ���� �� �ּ� ����
                    break;

                case PlayerDistanceType.InAttackRange:
                    // controller.ChangeState(new AttackState(controller)); // ���� �� �ּ� ����
                    break;
            }
        }

        public void Exit() { }
    }
}
