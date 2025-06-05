using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common; // For common states like Hit, Groggy, Idle
using CommonMonster.Stats; // For monsterStats
using System.Collections; // For coroutines

namespace CommonMonster.States.Forg
{
    public class ForgChaseJumpState : BaseMonsterState
    {
        public ForgChaseJumpState(CommonMonsterController controller) : base(controller)
        {

        }

        public override void Enter()
        {
            Debug.Log("[ForgChaseJumpState] Entering Chase Jump State.");
            controller.animator.Play("Forg_Jump"); // Forg의 점프 애니메이션 재생
            controller.isJumping = true; // 점프 중 플래그 설정

            controller.FaceToPlayer(); // 점프 시작 시 플레이어 방향 바라보기

            // 기존 y속도 초기화 후 점프 힘 적용
            controller.rb.velocity = new Vector2(controller.rb.velocity.x, 0);
            // CommonMonsterStats에서 가져온 jumpForce 사용
            controller.rb.AddForce(Vector2.up * controller.monsterStats.jumpForce, ForceMode2D.Impulse);
        }

        public override void Execute()
        {
            // 사망, 그로기, 피격 경직 중에는 상태 전이 로직을 실행하지 않음
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                controller.rb.velocity = Vector2.zero; // 상태 비활성화 시 움직임 중단
                return;
            }

            // 점프 중에도 플레이어 방향으로 수평 이동 유지
            float directionToPlayerX = Mathf.Sign(controller.player.position.x - controller.transform.position.x);
            // CommonMonsterStats에서 가져온 horizontalJumpSpeed 사용
            controller.rb.velocity = new Vector2(directionToPlayerX * controller.monsterStats.moveSpeed, controller.rb.velocity.y);

            // ⭐ Execute()에서 착지 감지 (Animator Event를 사용하지 않을 경우) ⭐
            // 이 로직은 `ForgAnimatorEvents`의 `Forg_OnLand()`를 사용한다면 제거할 수 있습니다.
            if (controller.IsGrounded() && controller.rb.velocity.y <= 0.1f) // Y 속도가 거의 0에 가깝다면 착지로 간주
            {
                controller.isJumping = false; // 점프 중 플래그 해제
                Debug.Log("[ForgChaseJumpState] Landed (Execute). Changing state to ForgChaseState.");
                controller.ChangeState(new ForgIdleState(controller)); // 착지 후 추적 상태로 돌아감
                return; // 상태 전환 후 바로 종료
            }
        }

        public override void Exit()
        {
            Debug.Log("[ForgChaseJumpState] Exiting Chase Jump State.");
            controller.isJumping = false; // 점프 중 플래그 해제
        }

        private IEnumerator CheckLandingRoutine()
        {
            // 점프 애니메이션이 끝나거나, 일정 시간 이상 Y축 속도가 줄어들 때까지 기다림
            // 또는 단순히 IsGrounded()가 true가 될 때까지 기다림
            yield return new WaitForSeconds(1f); // 점프 직후 잠시 대기하여 높이 오르도록

            while (controller.isJumping)
            {
                // 땅에 착지했는지 확인 (IsGrounded()는 CommonMonsterController에 있음)
                if (controller.IsGrounded())
                {
                    Debug.Log("[ForgChaseJumpState] Landed. Changing state to ForgChaseState.");
                    controller.ChangeState(new ForgIdleState(controller)); // 착지 후 다시 Chase State로 돌아감
                    yield break; // 코루틴 종료
                }
                yield return null; // 다음 프레임까지 대기
            }
        }
    }
}