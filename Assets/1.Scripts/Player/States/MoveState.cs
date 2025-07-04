using UnityEngine;

namespace Player.States
{
    public class MoveState : GroundedState
    {
        public override void Enter(PlayerStateController controller)
        {
            if (!controller.StateMachine.CurrentStateInstance.CanTransitionTo(PlayerState.Moving))
                return;
        }

        public override void Update(PlayerStateController controller)
        {
            base.Update(controller);

            var input = controller.GetComponent<PlayerInputHandler>();
            var motor = controller.GetComponent<PlayerMotor>();
            var anim = controller.GetComponent<PlayerAnimationController>();

            // 구르기 조건: 이동 중 아래 방향키 입력
            if (input.RollPressed && Mathf.Abs(input.Horizontal) > 0.1f)
            {
                controller.RequestStateChange(PlayerState.Rolling);
                return;
            }

            motor.Move(input.Horizontal);
            anim.UpdateDirection(input.Horizontal);
            anim.UpdateLocomotionBlend();

            if (Mathf.Abs(input.Horizontal) == 0)
            {
                controller.RequestStateChange(PlayerState.Idle);
            }
            else if (input.JumpPressed && motor.IsGrounded())
            {
                controller.RequestStateChange(PlayerState.Jumping);
            }
        }

        public override void Exit(PlayerStateController controller)
        {
            controller.GetComponent<PlayerAnimationController>().SetSpeed(0);
        }
        public override bool CanTransitionTo(PlayerState nextState)
        {
            return true;
        }
    }
}
