using UnityEngine;

namespace Player.States
{
    public class JumpState : GroundedState
    {
        public override void Enter(PlayerStateController controller)
        {
            var motor = controller.GetComponent<PlayerMotor>();
            var anim = controller.GetComponent<PlayerAnimationController>();

            motor.Jump();
            anim.SetJump(true);
        }

        public override void Update(PlayerStateController controller)
        {
            base.Update(controller); 

            var motor = controller.GetComponent<PlayerMotor>();
            var anim = controller.GetComponent<PlayerAnimationController>();
            var input = controller.GetComponent<PlayerInputHandler>();

            anim.UpdateJumpParameters();
            anim.UpdateDirection(input.Horizontal);

            motor.Move(input.Horizontal); // ✅ 공중에서도 입력 적용

            if (motor.IsGrounded() && motor.GetVerticalVelocity() <= 0)
            {
                controller.RequestStateChange(PlayerState.Idle);
            }
        }
        public override void Exit(PlayerStateController controller)
        {
            controller.GetComponent<PlayerAnimationController>().SetJump(false); // 상태 종료 시 isJump = false 처리
        }
        public override bool CanTransitionTo(PlayerState nextState)
        {
            // 점프 상태에서는 피격, 넉백, 스킬, 사망 외에는 자유롭게 전이 가능
            return nextState != PlayerState.Crouching &&
                   nextState != PlayerState.Guarding;
        }
    }

}