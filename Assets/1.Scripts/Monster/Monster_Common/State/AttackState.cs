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

            // 공격 애니메이션 재생 (예: Goblin_Attack)
            controller.animator.Play($"{controller.monsterName}_Attack");
        }

        public void Execute()
        {
            // 공격 중에는 방향 고정
            // 필요 시 controller.FaceToPlayer() 생략 가능
        }

        public void Exit()
        {
            controller.isAttacking = false;
        }
    }
}
