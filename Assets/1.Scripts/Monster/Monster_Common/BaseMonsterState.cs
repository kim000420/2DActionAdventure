using CommonMonster.Controller; // CommonMonsterController 참조

namespace CommonMonster.States
{
    public abstract class BaseMonsterState : IMonsterState
    {
        // 모든 몬스터 상태가 접근할 수 있는 몬스터 컨트롤러 인스턴스
        protected CommonMonsterController controller;

        // 생성자: 상태가 생성될 때 컨트롤러 인스턴스를 받아서 저장
        public BaseMonsterState(CommonMonsterController controller)
        {
            this.controller = controller;
        }

        // IMonsterState 인터페이스의 메서드들을 추상으로 선언
        // 이 클래스를 상속받는 모든 구체적인 상태 클래스들은 이 메서드들을 반드시 구현해야 함
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}