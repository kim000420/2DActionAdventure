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
            // Hit 애니메이션이 끝나면 Idle로 복귀하는 방식이 일반적
            // 여기서는 Animator 이벤트로 직접 상태 전이를 트리거하거나,
            // 일정 시간 후 복귀하도록 수정할 수 있음
        }

        public void Exit()
        {
            // 필요 시 피격 효과 종료 처리 가능
        }
    }
}
