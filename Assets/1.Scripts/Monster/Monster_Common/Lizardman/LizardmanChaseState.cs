using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common; // BaseMonsterState 참조
using CommonMonster.States.Lizardman; // Lizardman 전용 상태 참조 (MeleeAttackState, RangedAttackState)

namespace CommonMonster.States.Lizardman
{
    public class LizardmanChaseState : BaseMonsterState
    {
        // 생성자
        public LizardmanChaseState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[LizardmanChaseState] Entering Chase State.");
            // Lizardman의 Run 애니메이션 재생
            // CommonMonsterController의 monsterName을 "Lizardman"으로 설정해야 함
            controller.animator.Play("Lizardman_Run");
        }

        public override void Execute()
        {
            // 사망, 그로기, 피격 경직 중에는 상태 전이 로직을 실행하지 않음
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                // 상태 전이는 막지만, 현재 상태의 이동/방향 전환은 멈추도록 처리
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // 1. 인식 범위를 벗어나면 IdleState로 전이
            if (distanceToPlayer > controller.monsterStats.detectionRange)
            {
                Debug.Log("[LizardmanChaseState] Player out of detection range. Transitioning to IdleState.");
                controller.ChangeState(new LizardmanIdleState(controller));
                return; // 다음 로직 실행 방지
            }

            // 2. 플레이어를 향해 이동 및 방향 전환
            // CommonMonsterController의 FaceToPlayer()와 moveSpeed를 사용
            // 플레이어의 x 위치를 기준으로 몬스터의 방향을 설정하고 이동
            float directionToPlayerX = Mathf.Sign(controller.player.position.x - controller.transform.position.x);
            controller.rb.velocity = new Vector2(directionToPlayerX * controller.monsterStats.moveSpeed, controller.rb.velocity.y);
            controller.FaceToPlayer(); // 스프라이트 방향 전환

            // 3. 공격 범위 체크 및 공격 State로 전이 (근거리가 우선)
            // CommonMonsterStats에 meleeAttackRange와 rangedAttackRange가 설정되어 있어야 함
            if (distanceToPlayer <= controller.monsterStats.meleeAttackRange)
            {
                Debug.Log("[LizardmanChaseState] Player in melee attack range. Transitioning to MeleeAttackState.");
                // 다음 단계에서 생성할 LizardmanMeleeAttackState
                controller.ChangeState(new LizardmanMeleeAttackState(controller));
            }
            else if (distanceToPlayer <= controller.monsterStats.rangedAttackRange)
            {
                Debug.Log("[LizardmanChaseState] Player in ranged attack range. Transitioning to RangedAttackState.");
                // 다음 단계에서 생성할 LizardmanRangedAttackState
                controller.ChangeState(new LizardmanRangedAttackState(controller));
            }
            // else: 공격 범위에 들어오지 않았다면 계속 추적 (Execute의 나머지 부분이 처리)
        }

        public override void Exit()
        {
            Debug.Log("[LizardmanChaseState] Exiting Chase State.");
            // ChaseState 종료 시 이동 멈춤 (다음 상태에서 다시 이동을 제어할 것임)
            controller.rb.velocity = Vector2.zero;
        }
    }
}