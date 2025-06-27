using UnityEngine;
using TutorialBoss.Controller;

namespace TutorialBoss.States.Dok2
{
    public class Dok2ChaseState : BaseTutorialBossState
    {
        public Dok2ChaseState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_Walk"); // Dok2_Run 애니메이션 재생
            controller.rb.velocity = Vector2.zero; // 이전 상태의 잔여 속도 초기화
        }

        public override void Execute()
        {
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;

            if (controller.player == null)
            {
                // 플레이어가 없다면 Idle 상태로 돌아가거나 대기 (현재는 Chase 유지)
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // 플레이어를 바라봄
            controller.FaceToPlayer();

            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // 공격 범위 체크
            if (distanceToPlayer <= controller.bossStats.attackRange && !controller.isAttackCooldown)
            {
                // 플레이어가 근접 공격 범위 내에 있고, 쿨타임이 아니라면 AttackState로 전환
                controller.FaceToPlayer(); // 공격 전에 플레이어를 바라보도록
                controller.ChangeState(new Dok2AttackState(controller));
                return;
            }
            else
            {
                // 플레이어가 인식 범위 안에 있다면 플레이어 방향으로 이동
                if (distanceToPlayer <= controller.bossStats.detectRange) // Dok2Stats에 detectionRange가 있다고 가정
                {
                    Vector2 direction = (controller.player.position - controller.transform.position).normalized;
                    controller.transform.position += (Vector3)(direction * controller.bossStats.moveSpeed * Time.deltaTime);
                }
            }
        }

        public override void Exit()
        {
            Debug.Log("[Dok2ChaseState] Exit");
        }
    }
}