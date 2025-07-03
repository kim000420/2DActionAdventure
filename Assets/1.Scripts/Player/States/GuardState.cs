using UnityEngine;

namespace Player.States
{
    public class GuardState : IPlayerState
    {
        private float holdTime = 0.5f;
        private float timer = 0f;
        private bool hasHeldEnough = false;
        private bool isReleased = false;
        private float enterTime;

        public void Enter(PlayerStateController controller)
        {
            timer = 0f;
            hasHeldEnough = false;
            isReleased = false;
            enterTime = Time.time;

            var motor = controller.GetComponent<PlayerMotor>();
            motor.EnableMovementOverride(); // 이동 금지

            var anim = controller.GetComponent<PlayerAnimationController>();
            anim.SetBool("isGuarding", true);

            var stats = controller.GetComponent<PlayerStats>();
            stats.SetGuarding(true);
        }
        public void Update(PlayerStateController controller)
        {
            var input = controller.GetComponent<PlayerInputHandler>();
            timer += Time.deltaTime;

            if (timer >= holdTime) hasHeldEnough = true;
            if (!input.GuardHeld) isReleased = true;

            if (hasHeldEnough && isReleased)
            {
                controller.RequestStateChange(PlayerState.Idle);
            }
        }
        public void Exit(PlayerStateController controller)
        {
            var motor = controller.GetComponent<PlayerMotor>();
            motor.DisableMovementOverride();

            var anim = controller.GetComponent<PlayerAnimationController>();
            anim.SetBool("isGuarding", false);

            var stats = controller.GetComponent<PlayerStats>();
            stats.SetGuarding(false);
        }
        public bool CanTransitionTo(PlayerState nextState)
        {
            // 가드 중에는 피격, 넉백, 사망만 허용
            return nextState is PlayerState.Hit or PlayerState.Knockback or PlayerState.Dead;
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
}
