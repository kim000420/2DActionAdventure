using Player.States;
using UnityEngine;

public abstract class CrouchGuardBaseState : IPlayerState
{
    protected float holdTime = 0.5f; // 최소 유지 시간
    protected float cooldown = 0.5f; // 재사용 대기 시간
    protected float timer = 0f;
    protected bool hasHeldEnough = false;
    protected bool isReleased = false;
    protected float cooldownTimer = 0f;

    public virtual void Enter(PlayerStateController controller)
    {
        timer = 0f;
        hasHeldEnough = false;
        isReleased = false;

        var motor = controller.GetComponent<PlayerMotor>();
        motor.EnableMovementOverride(); // ✅ 이동 금지
    }

    public virtual void Update(PlayerStateController controller)
    {
        var input = controller.GetComponent<PlayerInputHandler>();
        var anim = controller.GetComponent<PlayerAnimationController>();

        timer += Time.deltaTime;

        if (timer >= holdTime)
            hasHeldEnough = true;

        if (!IsHolding(input))
        {
            isReleased = true;
        }

        if (hasHeldEnough && isReleased)
        {
            cooldownTimer = cooldown;
            controller.RequestStateChange(PlayerState.Idle);
        }
    }

    public virtual void Exit(PlayerStateController controller)
    {
        var motor = controller.GetComponent<PlayerMotor>();
        motor.DisableMovementOverride(); // 이동 허용 복귀
    }

    protected abstract bool IsHolding(PlayerInputHandler input);
}
