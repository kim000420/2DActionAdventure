namespace TutorialBoss.States
{
    public interface ITutorialBossState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}
