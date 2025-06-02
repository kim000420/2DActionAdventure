using UnityEngine;

public class GuardState : CrouchGuardBaseState
{
    private float enterTime;

    public override void Enter(PlayerStateController controller)
    {
        base.Enter(controller);
        enterTime = Time.time;

        var anim = controller.GetComponent<PlayerAnimationController>();
        anim.SetBool("isGuarding", true);

        var stats = controller.GetComponent<PlayerStats>();
        stats.SetGuarding(true);
    }

    public override void Exit(PlayerStateController controller)
    {
        base.Exit(controller);

        var anim = controller.GetComponent<PlayerAnimationController>();
        anim.SetBool("isGuarding", false);

        var stats = controller.GetComponent<PlayerStats>();
        stats.SetGuarding(false);
    }

    protected override bool IsHolding(PlayerInputHandler input)
    {
        return input.GuardHeld;
    }

    public float TimeSinceEntered => Time.time - enterTime;

    public void OnGuardHit(int damage, DamageType type, float timeSinceStart, float forceX, float forceY, float attackerX, PlayerStateController controller)
    {
        var stats = controller.GetComponent<PlayerStats>();
        var motor = controller.GetComponent<PlayerMotor>();

        if (type == DamageType.GuardBreak || damage > stats.currentStamina)
        {
            stats.TakeDamage(damage, type, KnockbackType.Strong, forceX, forceY, attackerX);
            return;
        }

        if (timeSinceStart <= 0.5f)
        {
            stats.UseStamina((int)(damage * 0.5f));
            var anim = controller.GetComponent<PlayerAnimationController>();
            anim.SetTrigger("JustGuard");
        }
        else
        {
            stats.UseStamina(damage);
        }

        // Guard 성공 시 미세 넉백 방향 계산
        float direction = controller.transform.position.x < attackerX ? -1f : 1f;
        Vector2 guardKnockback = new Vector2(forceX * 0.4f * direction, 0f);
        motor.ForceMove(guardKnockback);
    }
}