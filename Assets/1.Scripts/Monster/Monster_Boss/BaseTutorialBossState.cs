using TutorialBoss.Controller;

namespace TutorialBoss.States
{
    public abstract class BaseTutorialBossState : ITutorialBossState
    {
        protected TutorialBossStateController controller;

        public BaseTutorialBossState(TutorialBossStateController controller)
        {
            this.controller = controller;
        }

        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}
