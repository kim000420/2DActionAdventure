using UnityEngine;
using TutorialBoss.Controller;
using TutorialBoss.States; // BaseTutorialBossState 참조

namespace TutorialBoss.States.Webuin
{
    public class WebuinChaseState : BaseTutorialBossState
    {
        public WebuinChaseState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[WebuinChaseState] Entering Chase State.");
            controller.animator.Play("Webuin_ChaseWalk"); // Webuin의 추격(달리기) 애니메이션 재생
            controller.rb.velocity = Vector2.zero; // 이전 상태의 잔여 속도 초기화
        }

        public override void Execute()
        {
            // 보스의 죽음, 그로기, 피격 경직 상태에서는 상태 전이 로직을 실행하지 않습니다.
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;

            if (controller.player == null)
            {
                // 플레이어가 없다면 Idle 상태로 돌아가거나 대기 (현재는 Chase 유지)
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // 보스 스프라이트 방향 전환
            controller.FaceToPlayer();

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // 공격 범위 체크
            if (distanceToPlayer <= controller.bossStats.attackRange && !controller.isAttackCooldown)
            {
                // 플레이어가 근접 공격 범위 내에 있고, 쿨타임이 아니라면 AttackState로 전환
                controller.FaceToPlayer(); // 공격 전에 플레이어를 바라보도록
                controller.ChangeState(new WebuinAttackState(controller));
                return;
            }
            else
            {
                // 플레이어가 인식 범위 안에 있다면 플레이어 방향으로 이동
                if (distanceToPlayer <= controller.bossStats.detectRange) // Dok2Stats에 detectionRange가 있다고 가정
                {
                    Vector2 moveDirection = controller.player.position - controller.transform.position;
                    controller.rb.velocity = new Vector2(moveDirection.x * controller.bossStats.moveSpeed, controller.rb.velocity.y);
                }
            }
        }

        public override void Exit()
        {
            Debug.Log("[WebuinChaseState] Exit");
            controller.rb.velocity = Vector2.zero; // 상태 종료 시 이동 멈춤
        }
    }
}