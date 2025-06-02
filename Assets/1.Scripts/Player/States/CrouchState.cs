using UnityEngine;

public class CrouchState : CrouchGuardBaseState
{
    private CapsuleCollider2D collider;
    private Vector2 originalSize;
    private Vector2 crouchSize = new Vector2(0.5f, 0.689999998f);
    private Vector2 originalOffset;
    private Vector2 crouchOffset = new Vector2(0, 0.340000004f);

    public override void Enter(PlayerStateController controller)
    {
        base.Enter(controller);

        collider = controller.GetComponent<CapsuleCollider2D>();
        originalSize = collider.size;
        originalOffset = collider.offset;

        collider.size = crouchSize;
        collider.offset = crouchOffset;

        var anim = controller.GetComponent<PlayerAnimationController>();
        anim.SetBool("isCrouching", true);
    }

    public override void Exit(PlayerStateController controller)
    {
        base.Exit(controller);

        collider.size = originalSize;
        collider.offset = originalOffset;

        var anim = controller.GetComponent<PlayerAnimationController>();
        anim.SetBool("isCrouching", false);
    }

    protected override bool IsHolding(PlayerInputHandler input)
    {
        return input.CrouchHeld;
    }
}