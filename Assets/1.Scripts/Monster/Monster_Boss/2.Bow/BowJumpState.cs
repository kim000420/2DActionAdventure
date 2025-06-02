using UnityEngine;
using TutorialBoss.Controller;
using System.Collections;

namespace TutorialBoss.States.Bow
{
    public class BowJumpState : BaseTutorialBossState
    {
        private bool isWallEvadeJump;
        private bool hasAppliedJumpForce = false;
        private bool hasStartedAnimation = false;

        public BowJumpState(TutorialBossStateController controller, bool isWallEvadeJump = false) : base(controller)
        {
            this.isWallEvadeJump = isWallEvadeJump;
        }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_JumpStart");
            controller.isBowJumping = true;
            hasAppliedJumpForce = false;
            hasStartedAnimation = false;

            controller.rb.velocity = new Vector2(0, controller.rb.velocity.y);
            controller.StartJumpGracePeriod();
            controller.FaceToPlayer();
        }

        public override void Execute()
        {
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;

            if (!hasStartedAnimation) return; // hasStartedAnimation으로 변경

            // 1. 점프 힘이 적용된 후, 실제로 공중에서 착지했는지 감지
            if (hasAppliedJumpForce && controller.IsGrounded())
            {
                Debug.Log("[BowJumpState] 지면 착지 감지! 즉시 점프 상태 종료 및 다음 행동으로 전이.");
                // 점프 중 상태를 종료
                controller.isBowJumping = false;
                // Rigidbody 속도 초기화 (미끄러짐 방지)
                controller.rb.velocity = Vector2.zero;

                // 다음 상태로 즉시 전이
                float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);
                if (distanceToPlayer <= controller.bossStats.escapeTriggerRange)
                {
                    controller.ChangeState(new BowEscapeState(controller));
                }
                else
                {
                    controller.ChangeState(new BowAttackState(controller));
                }
                return; // 상태 변경 후 즉시 리턴
            }

            // 2. 아직 점프 힘을 적용하지 않았고 (즉, 점프 시작 직후), 땅에 있다면 점프 힘 적용
            if (!hasAppliedJumpForce)
            {
                Debug.Log("[BowJumpState] 점프 힘 적용.");
                controller.rb.velocity = new Vector2(controller.rb.velocity.x, 0);
                controller.rb.AddForce(Vector2.up * controller.bowJumpForce, ForceMode2D.Impulse);
                hasAppliedJumpForce = true; // 점프 힘 적용 완료 플래그

                // 벽 회피 점프 시 횡방향 힘 적용
                if (isWallEvadeJump)
                {
                    // ⭐ 변경: 벽 회피 점프 시 횡방향 힘 로직 수정
                    // 보스가 현재 바라보고 있는 방향(currentFacingDirection)이 곧 벽이 감지된 방향
                    // 따라서, 벽에서 멀어지려면 currentFacingDirection의 반대 방향으로 점프해야 함.
                    float currentFacingDirection = Mathf.Sign(controller.transform.localScale.x);
                    float horizontalJumpDirection = -currentFacingDirection; // ⭐ 벽이 있는 방향의 반대 방향으로 점프

                    float horizontalJumpForce = controller.bowJumpForce * 0.5f;
                    controller.rb.velocity = new Vector2(horizontalJumpDirection * horizontalJumpForce, controller.rb.velocity.y);
                    Debug.Log($"[BowJumpState] 벽 회피 점프 - 횡방향 힘 적용: {horizontalJumpDirection * horizontalJumpForce}. 현재 바라보는 방향: {controller.transform.localScale.x}");
                }
            }
        }

        public override void Exit()
        {
            controller.isBowJumping = false;
            // ⭐ JumpState 종료 시 혹시 모를 횡방향 잔여 속도 제거 및 착지 후 다음 행동을 위한 초기화
            controller.rb.velocity = Vector2.zero;
        }

        public void OnJumpAnimationStarted()
        {
            hasStartedAnimation = true;
        }
    }
}