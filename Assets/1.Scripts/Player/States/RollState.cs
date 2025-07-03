using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Player.States
{
    public class RollState : GroundedState
    {
        private float rollDuration = 0.9f;
        private float rollTime = 0f;
        private float rollSpeed = 10f;
        private float decelerationTime = 0.5f;
        private bool isDecelerating = false;
        private float direction;
        private int staminaCost = 20;

        public override void Enter(PlayerStateController controller)
        {
            var anim = controller.GetComponent<PlayerAnimationController>();
            var input = controller.GetComponent<PlayerInputHandler>();
            var motor = controller.GetComponent<PlayerMotor>();
            var col = controller.GetComponent<CapsuleCollider2D>();
            var stats = controller.GetComponent<PlayerStats>();

            //구르기 상태로 조정
            col.size = new Vector2(0.5f, 0.689999998f);
            col.offset = new Vector2(0, 0.340000004f);

            direction = Mathf.Sign(input.Horizontal);
            if (direction == 0) direction = controller.GetComponent<SpriteRenderer>().flipX ? -1 : 1;

            isDecelerating = false;

            anim.SetBool("isRoll", true);
            rollTime = 0f;
            stats.UseStamina(staminaCost);

            motor.EnableMovementOverride();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

            // 무적 타이머 (0.3초간 레이어 충돌 무시 등 가능, 추후 확장)
            // 예: controller.StartCoroutine(Invincibility(0.3f));
        }

        public override void Update(PlayerStateController controller)
        {
            base.Update(controller);

            var motor = controller.GetComponent<PlayerMotor>();
            var anim = controller.GetComponent<PlayerAnimationController>();

            rollTime += Time.deltaTime;

            // 전체 길이: rollDuration = 0.6
            // 감속 시작점: 0.6 - 0.15 = 0.45초 후
            if (rollTime < rollDuration - decelerationTime)
            {
                motor.ForceMove(direction * rollSpeed); // 고속 전진
            }
            else if (!isDecelerating)
            {
                isDecelerating = true;
                motor.ForceMove(direction * (rollSpeed * 0.3f)); // 순간 감속
            }

            if (rollTime >= rollDuration)
            {
                anim.SetBool("isRoll", false);
                controller.RequestStateChange(PlayerState.Idle);
            }
        }


        public override void Exit(PlayerStateController controller)
        {
            var motor = controller.GetComponent<PlayerMotor>();
            var col = controller.GetComponent<CapsuleCollider2D>();

            // 원상태로 조정
            col.size = new Vector2(0.5f, 1.29966235f);
            col.offset = new Vector2(0f, 0.649831176f);

            motor.StopImmediately(); // 속도 0으로 초기화
            motor.DisableMovementOverride();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        }
        public override bool CanTransitionTo(PlayerState nextState)
        {
            // 구르기 중에는 대부분 전이 금지 (단, 피격, 넉백, 사망은 예외)
            return nextState is PlayerState.Hit or PlayerState.Knockback or PlayerState.Dead;
        }
    }
}