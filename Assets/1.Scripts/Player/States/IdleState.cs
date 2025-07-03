using UnityEngine;

namespace Player.States
{
    public class IdleState : GroundedState
    {
        private float entryDelay = 0.1f;
        private float delayTimer = 0f;
        public override void Enter(PlayerStateController controller)
        {
            controller.GetComponent<PlayerAnimationController>().SetSpeed(0);
            delayTimer = entryDelay;
        }
        public override void Update(PlayerStateController controller)
        {
            base.Update(controller);

            var input = controller.GetComponent<PlayerInputHandler>();

            if (input.CrouchHeld) controller.RequestStateChange(PlayerState.Crouching);
            if (input.GuardHeld) controller.RequestStateChange(PlayerState.Guarding);

            delayTimer -= Time.deltaTime;
            if (delayTimer > 0f)
                return; // 아직 입력 무시 중

            if (Mathf.Abs(input.Horizontal) > 0)
            {
                controller.RequestStateChange(PlayerState.Moving);
            }

            if (input.JumpPressed)
            {
                controller.RequestStateChange(PlayerState.Jumping);
            }
        }
        public override void Exit(PlayerStateController controller) { }
        public override bool CanTransitionTo(PlayerState nextState)
        {
            // Idle 상태에서는 어떤 상태로도 전이 가능
            return true;
        }
    }
}