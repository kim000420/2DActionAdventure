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
            Object.Destroy(controller.gameObject, 2f); // 2초 후 제거
        }

        public void Execute()
        {
            // 아무 것도 하지 않음
        }

        public void Exit()
        {
            // 사망 상태는 Exit 호출되지 않을 수 있음 (참고용)
        }
    }
}
