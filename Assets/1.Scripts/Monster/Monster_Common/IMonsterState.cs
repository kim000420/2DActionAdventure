namespace CommonMonster.States
{
    public interface IMonsterState
    {
        // 상태 진입 시 한 번 호출되는 메서드
        void Enter();

        // 상태가 활성화되어 있는 동안 매 프레임(또는 FixedUpdate) 호출되는 메서드
        void Execute();

        // 상태 종료 시 한 번 호출되는 메서드
        void Exit();
    }
}