using UnityEngine;

namespace Player.States
{
    public abstract class GroundedState : IPlayerState
    {
        public virtual void Enter(PlayerStateController controller) { }

        public virtual void Exit(PlayerStateController controller) { }

        public virtual void Update(PlayerStateController controller)
        {
            HandleCommonInput(controller);
        }

        protected void HandleCommonInput(PlayerStateController controller)
        {
            var input = controller.GetComponent<PlayerInputHandler>();
            var state = controller.StateMachine.CurrentEnumState;

            if (state == PlayerState.Idle && input.CrouchHeld)
            {
                controller.RequestStateChange(PlayerState.Crouching);
                return;
            }

            if ((state == PlayerState.Idle || state == PlayerState.Moving) && input.GuardHeld)
            {
                controller.RequestStateChange(PlayerState.Guarding);
                return;
            }
        }

        public abstract bool CanTransitionTo(PlayerState nextState);
    }
}
