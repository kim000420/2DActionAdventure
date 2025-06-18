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

            // attack1과 attack2 중 50% 확률로 하나를 선택
            // Random.value는 0.0f에서 1.0f 사이의 값을 반환합니다.
            if (Random.value < 0.5f)
            {
                Debug.Log("[LizardmanMeleeAttackState] Performing Attack1.");
                // Lizardman의 Attack1 애니메이션 재생
                controller.animator.Play("Lizardman_Attack1");
            }
            else
            {
                Debug.Log("[LizardmanMeleeAttackState] Performing Attack2.");
                // Lizardman의 Attack2 애니메이션 재생
                controller.animator.Play("Lizardman_Attack2");
            }

            // ⭐ 중요: 공격 애니메이션 종료 시점에 Animator Event를 추가해야 합니다.
            // 이 이벤트에서 controller.ChangeState(new LizardmanIdleState(controller)); 를 호출하도록 설정해야 합니다.
            // 예를 들어, CommonMonsterController에 public void OnAttackAnimationEnd() 함수를 만들고,
            // 이 함수 안에서 controller.ChangeState(new LizardmanIdleState(controller)); 를 호출하도록 하면 됩니다.
            // 애니메이터 이벤트를 통해 이 함수를 연결하는 것은 이 코드 작성 이후 유니티 에디터에서 진행됩니다.
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