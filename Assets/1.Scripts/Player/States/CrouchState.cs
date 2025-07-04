using UnityEngine;

namespace Player.States
{
    public class CrouchState : IPlayerState
    {
        private CapsuleCollider2D collider;
        private Vector2 originalSize;
        private Vector2 crouchSize = new Vector2(0.5f, 0.689999998f);
        private Vector2 originalOffset;
        private Vector2 crouchOffset = new Vector2(0, 0.340000004f);

        private float holdTime = 0.5f;
        private float timer = 0f;
        private bool hasHeldEnough = false;
        private bool isReleased = false;


        public void Enter(PlayerStateController controller)
        {
            timer = 0f;
            hasHeldEnough = false;
            isReleased = false;

            var motor = controller.GetComponent<PlayerMotor>();
            motor.EnableMovementOverride(); // 이동 금지

            collider = controller.GetComponent<CapsuleCollider2D>();
            originalSize = collider.size;
            originalOffset = collider.offset;

            collider.size = crouchSize;
            collider.offset = crouchOffset;

            var anim = controller.GetComponent<PlayerAnimationController>();
            anim.SetBool("isCrouching", true);
        }
        public void Update(PlayerStateController controller)
        {
            var input = controller.GetComponent<PlayerInputHandler>();

            timer += Time.deltaTime;
            if (timer >= holdTime) hasHeldEnough = true;
            if (!input.CrouchHeld) isReleased = true;

            if (hasHeldEnough && isReleased)
            {
                controller.RequestStateChange(PlayerState.Idle);
            }
        }
        public  void Exit(PlayerStateController controller)
        {
            var motor = controller.GetComponent<PlayerMotor>();
            motor.DisableMovementOverride();

            collider.size = originalSize;
            collider.offset = originalOffset;

            var anim = controller.GetComponent<PlayerAnimationController>();
            anim.SetBool("isCrouching", false);
        }
        public bool CanTransitionTo(PlayerState nextState)
        {
            // 앉은 상태에서 스킬 시전, 점프, 피격, 넉백, 사망만 허용
            return nextState is PlayerState.SkillCasting or
                              PlayerState.Jumping or
                              PlayerState.Idle or
                              PlayerState.Hit or
                              PlayerState.Knockback or
                              PlayerState.Dead;
        }
    }
}
