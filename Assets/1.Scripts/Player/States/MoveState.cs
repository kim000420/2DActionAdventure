using Player.States;
using UnityEngine;

public class MoveState : IPlayerState
{
    public void Enter(PlayerStateController controller)
    {
        if (!controller.CanTransitionTo(PlayerState.Moving))
            return;
    }

    public void Update(PlayerStateController controller)
    {
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

    public void Exit(PlayerStateController controller)
    {
        controller.GetComponent<PlayerAnimationController>().SetSpeed(0);
    }
}
