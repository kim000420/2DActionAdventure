using UnityEngine;

namespace Player.States
{
    public class KnockbackState : IPlayerState
    {
        private KnockbackType knockbackType;
        private Vector2 knockbackForce;
        private float stateTimer;
        private bool isGrounded;
        private bool isWakeupPlayed;

        private const float strongKnockbackDuration = 2.0f;
        private const float weakKnockbackDuration = 0.5f;

        public KnockbackState() { }

        public KnockbackState(KnockbackType type, Vector2 force)
        {
            knockbackType = type;
            knockbackForce = force;
        }

        public void Enter(PlayerStateController controller)
        {
            var anim = controller.GetComponent<PlayerAnimationController>();
            var input = controller.GetComponent<PlayerInputHandler>();
            var motor = controller.GetComponent<PlayerMotor>();

            stateTimer = 0f;
            isWakeupPlayed = false;
            isGrounded = motor.IsGrounded();

            if (knockbackType == KnockbackType.Weak)
            {
                if (isGrounded)
                {
                    anim.SetTrigger("WeakHit");
                }
                motor.ForceMove(new Vector2(knockbackForce.x, 0f));

            }
            else // Strong
            {
                anim.SetTrigger("StartKnockback");
                motor.ForceMove(new Vector2(knockbackForce.x, knockbackForce.y));
            }

            motor.EnableMovementOverride();
        }

        public void Update(PlayerStateController controller)
        {
            var anim = controller.GetComponent<PlayerAnimationController>();
            var motor = controller.GetComponent<PlayerMotor>();
            stateTimer += Time.deltaTime;

            anim.SetVerticalVelocity(motor.GetVerticalVelocity());

            if (knockbackType == KnockbackType.Weak)
            {
                if (stateTimer >= weakKnockbackDuration)
                    controller.RequestStateChange(PlayerState.Idle);
                return;
            }

            // 강넉백 애니메이션은 Transition에 의해 자동 처리됨
            if (motor.IsGrounded() && !isWakeupPlayed)
            {
                anim.SetTrigger("landedFromKnockback");
                isWakeupPlayed = true;
            }

            if (stateTimer >= strongKnockbackDuration)
                controller.RequestStateChange(PlayerState.Idle);
        }

        public void Exit(PlayerStateController controller)
        {
            var anim = controller.GetComponent<PlayerAnimationController>();
            var motor = controller.GetComponent<PlayerMotor>();

            anim.SetBool("isKnockback", false);
            motor.DisableMovementOverride();
        }
        public bool CanTransitionTo(PlayerState nextState)
        {
            // 넉백 상태에서는 사망 외에는 전이 제한
            return nextState == PlayerState.Dead;
        }
        public static KnockbackState Create(KnockbackType type, float forceX, float forceY)
        {
            return new KnockbackState(type, new Vector2(forceX, forceY));
        }
    }
}
