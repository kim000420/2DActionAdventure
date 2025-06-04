using UnityEngine;
using TutorialBoss.Controller;
using System.Collections;

namespace TutorialBoss.States.Bow
{
    public class BowEscapeState : BaseTutorialBossState
    {
        private float escapeTimer;
        private bool hasJumpedToEvadeWall = false;

        public BowEscapeState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_Run");
            controller.isBowJumping = false;
            escapeTimer = controller.bowEscapeDuration;
            hasJumpedToEvadeWall = false;
        }

        public override void Execute()
        {
            if (controller.player == null)
            {
                return; // 플레이어가 없으면 더 이상 진행하지 않음
            }
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // 플레이어 반대 방향으로 이동
            Vector2 escapeDirection = (controller.transform.position - controller.player.position).normalized;
            controller.rb.velocity = new Vector2(escapeDirection.x * controller.bossStats.moveSpeed, controller.rb.velocity.y);
            controller.FaceAwayFromPlayer(); // 도망치는 방향을 바라보도록

            // 벽 감지 및 회피 점프 (보스가 바라보는 방향으로 감지)
            if (controller.IsWallAhead() && controller.IsGrounded() && !hasJumpedToEvadeWall) // ⭐ 인자 제거
            {
                controller.ChangeState(new BowJumpState(controller, true));
                hasJumpedToEvadeWall = true;
                return;
            }

            escapeTimer -= Time.deltaTime;

            if (distanceToPlayer > controller.bossStats.escapeTriggerRange || escapeTimer <= 0)
            {
                controller.ChangeState(new BowAttackState(controller));
            }
        }

        public override void Exit()
        {
            // 도망 중단 로직
        }
    }
}