using UnityEngine;
using Player.States;

public class JumpState : IPlayerState
{
    public void Enter(PlayerStateController controller)
    {
        var motor = controller.GetComponent<PlayerMotor>();
        var anim = controller.GetComponent<PlayerAnimationController>();

        motor.Jump();
        anim.SetJump(true);
    }

    public void Update(PlayerStateController controller)
    {
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



    public void Exit(PlayerStateController controller) 
    {
        controller.GetComponent<PlayerAnimationController>().SetJump(false); // 상태 종료 시 isJump = false 처리
    }
}
