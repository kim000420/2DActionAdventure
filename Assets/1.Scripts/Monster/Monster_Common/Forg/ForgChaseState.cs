using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common;
using CommonMonster.Stats;

namespace CommonMonster.States.Forg
{
    public class ForgChaseState : BaseMonsterState
    {
        private float nextJumpReadyTime; // 다음 점프를 시도할 수 있는 시간

        // 점프 관련 임계값은 스탯으로 옮기거나 Forg 전용 스탯으로 관리하는 것이 더 유연합니다.
        // 여기서는 예시로 그대로 두지만, CommonMonsterStats에 추가하는 것을 권장합니다.
        private float verticalObstacleThreshold = 1.5f; // 플레이어와의 Y축 차이가 이 값 이상이면 점프 고려

        public ForgChaseState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[ForgChaseState] Entering Chase State.");
            controller.animator.Play("Forg_Idle"); // Forg는 Chase 상태에서는 이동 애니메이션 없이 대기 (점프만 이동하므로)
                                                   // 만약 아주 짧은 이동 애니메이션이 있다면 Forg_Run을 사용해도 됨
            controller.rb.velocity = Vector2.zero; // Chase State 진입 시 움직임 멈춤
            controller.rb.gravityScale = 1f;

            // 다음 점프가 가능한 시간을 현재 시간 + 쿨타임으로 초기화
            nextJumpReadyTime = Time.time + controller.monsterStats.jumpCooldown;
            controller.isJumping = false; // 혹시 모를 경우를 대비하여 점프 중 플래그 해제
        }

        public override void Execute()
        {
            // 사망, 그로기, 피격 경직 중에는 로직 실행 안 함
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                controller.rb.velocity = Vector2.zero;
                return;
            }

            controller.FaceToPlayer(); // 플레이어 방향으로 바라봄 (공격 및 점프 방향 결정 위함)

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);
            float directionToPlayerX = Mathf.Sign(controller.player.position.x - controller.transform.position.x);
            float yDifference = controller.player.position.y - controller.transform.position.y;

            // 1. 공격 범위 안에 들어왔는지 확인 (공격 범위 안에 들어왔다면 ChaseState에서 직접 AttackState로 전이)
            if (distanceToPlayer <= controller.monsterStats.rangedAttackRange && !controller.isAttackCooldown)
            {
                // ForgAttackState로 전이 (이 부분은 다음 단계에서 구현)
                controller.ChangeState(new ForgAttackState(controller, 2f));
                return;
            }
            // 2. 인식 범위 밖으로 나갔는지 확인 (Idle 상태로 돌아갈 조건)
            else if (distanceToPlayer > controller.monsterStats.detectionRange)
            {
                controller.ChangeState(new ForgIdleState(controller));
                return;
            }

            // 3. 점프를 통한 이동 로직
            // 현재 시간이 다음 점프 가능 시간보다 크거나 같고, 땅에 닿아 있고, 점프 중이 아닐 때
            if (Time.time >= nextJumpReadyTime && controller.IsGrounded() && !controller.isJumping)
            {
                bool shouldJump = false;
                Debug.Log("[ForgChaseState] 점프준비애니");
                controller.animator.Play("Forg_JumpReady");

                // 3-1. 앞에 벽이 있는지 확인하여 점프
                if (controller.IsWallAhead(directionToPlayerX))
                {
                    shouldJump = true;
                    Debug.Log("[ForgChaseState] Wall ahead, initiating jump.");
                }
                // 3-2. 플레이어가 Forg보다 높은 위치에 있는지 확인하여 점프
                else if (yDifference > verticalObstacleThreshold)
                {
                    shouldJump = true;
                    Debug.Log("[ForgChaseState] Player is above, initiating jump.");
                }
                // 3-3. 단순히 플레이어에게 다가가기 위해 점프 (거리가 좁혀지지 않았다면 계속 점프)
                // 이 조건은 마지막에 두어 다른 점프 조건에 우선순위를 줍니다.
                else if (distanceToPlayer > controller.monsterStats.rangedAttackRange)
                {
                    shouldJump = true;
                    Debug.Log("[ForgChaseState] Player too far, initiating jump for chase.");
                }

                if (shouldJump)
                {
                    controller.ChangeState(new ForgChaseJumpState(controller));
                    // 점프 상태로 전이했으므로, 다음 점프 가능 시간을 업데이트할 필요가 없습니다.
                    // 다음 점프 가능 시간은 착지 후 다시 ChaseState로 돌아왔을 때 결정됩니다.
                    return; // 다음 상태로 전환되었으므로 현재 Execute 종료
                }
            }
            controller.rb.velocity = new Vector2(0, controller.rb.velocity.y); // 수평 이동은 멈춤
        }

        public override void Exit()
        {
            Debug.Log("[ForgChaseState] Exiting Chase State.");
        }
    }
}