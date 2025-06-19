using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common; // BaseMonsterState 참조
using CommonMonster.States.Lizardman; // IdleState 참조 (공격 종료 후 Idle로 전이)

namespace CommonMonster.States.Lizardman
{
    public class LizardmanMeleeAttackState : BaseMonsterState
    {
        // 생성자
        public LizardmanMeleeAttackState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[LizardmanMeleeAttackState] Entering Melee Attack State.");

            // 몬스터의 이동을 멈춤 (공격 중에는 움직이지 않음)
            controller.rb.velocity = Vector2.zero;

            // 플레이어를 바라보게 함 (공격 직전 위치를 고정)
            controller.FaceToPlayer();

            // 찌르기 공격
            controller.animator.Play("Lizardman_Attack2");
        }

        public override void Execute()
        {
            // 사망, 그로기, 피격 경직 중에는 상태 전이 로직을 실행하지 않음
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                // 공격 애니메이션이 중단될 수 있으므로, Rigidbody 속도 초기화
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // 공격 애니메이션이 진행되는 동안 Execute에서는 특별한 로직이 필요 없음.
            // 애니메이션 이벤트가 상태 전이를 담당합니다.
        }

        public override void Exit()
        {
            Debug.Log("[LizardmanMeleeAttackState] Exiting Melee Attack State.");
            // 나중에 필요할 경우 이곳에 정리 로직 추가
        }
    }
}