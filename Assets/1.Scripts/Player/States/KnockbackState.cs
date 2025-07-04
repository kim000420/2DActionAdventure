using UnityEngine;

namespace Player.States
{
    public class KnockbackState : IPlayerState
    {
        private KnockbackType knockbackType;
        private Vector2 knockbackForce;
        private float enterTime;
        private bool isGroundedOnEnter;
        private bool hasLanded = false;

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

            enterTime = Time.time;
            isGroundedOnEnter = motor.IsGrounded();
            hasLanded = false;

            anim.SetBool("isKnockback", true);

            if (knockbackType == KnockbackType.Weak)
            {
                if (isGroundedOnEnter)
                {
                    anim.SetTrigger("WeakHit");
                }
                motor.ForceMove(new Vector2(knockbackForce.x, 0f));

            }
            else if (knockbackType == KnockbackType.Strong)// Strong
            {
                anim.SetTrigger("StartKnockback");
                motor.ForceMove(knockbackForce);
            }

            motor.EnableMovementOverride();
        }

        public void Update(PlayerStateController controller)
        {
            var anim = controller.GetComponent<PlayerAnimationController>();
            var motor = controller.GetComponent<PlayerMotor>();

            if (!hasLanded && knockbackType == KnockbackType.Strong)
            {
                if (motor.IsGrounded())
                {
                    anim.SetTrigger("landedFromKnockback"); // Wakeup 애니 진입 트리거
                    hasLanded = true;
                }
            }
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
            return nextState is PlayerState.Idle or PlayerState.Dead;
        }
    }
}
