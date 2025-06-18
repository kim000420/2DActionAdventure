using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using CommonMonster.Controller;
using CommonMonster.States.Common; // BaseMonsterState 참조
using CommonMonster.States.Lizardman; // Lizardman 전용 상태 참조 (ChaseState for Lizardman)

namespace CommonMonster.States.Lizardman
{
    public class LizardmanIdleState : BaseMonsterState
    {
        private Coroutine idleDelayRoutine; // Idle 상태 진입 시 딜레이 코루틴
        private bool isDelayFinished = false; // 딜레이 완료 여부

        // 생성자
        public LizardmanIdleState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[LizardmanIdleState] Entering Idle State.");
            controller.animator.Play("Lizardman_Idle");

            // 딜레이 시작 전에 플래그 초기화
            isDelayFinished = false;
            idleDelayRoutine = controller.StartCoroutine(IdleDelayRoutine());

            // Idle 상태 진입 시 이동 멈춤
            controller.rb.velocity = Vector2.zero;
        }

        public override void Execute()
        {
            // 사망, 그로기, 피격 경직 중에는 상태 전이 로직을 실행하지 않음
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                return;
            }

            // 딜레이가 끝나지 않았으면 다음 상태로 전이하지 않음
            if (!isDelayFinished)
            {
                return;
            }

            // 플레이어 인식 범위 확인 (ChaseState로 전이 조건)
            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // 플레이어가 인식 범위 안에 들어오면 ChaseState로 전이
            if (distanceToPlayer <= controller.monsterStats.detectionRange)
            {
                Debug.Log("[LizardmanIdleState] Player detected. Transitioning to ChaseState.");
                controller.ChangeState(new LizardmanChaseState(controller)); // 다음 단계에서 생성할 LizardmanChaseState
            }
        }

        public override void Exit()
        {
            Debug.Log("[LizardmanIdleState] Exiting Idle State.");
            // 딜레이 코루틴이 아직 실행 중이라면 강제로 중단
            if (idleDelayRoutine != null)
            {
                controller.StopCoroutine(idleDelayRoutine);
                idleDelayRoutine = null;
            }
            isDelayFinished = false; // 플래그 초기화
        }

        // Idle 애니메이션 시간 + 추가 딜레이를 위한 코루틴
        private IEnumerator IdleDelayRoutine()
        {
            // 1초간 상태 전이 불가능 딜레이
            yield return new WaitForSeconds(1.0f);

            isDelayFinished = true; // 딜레이 완료 플래그 설정
            Debug.Log("[LizardmanIdleState] Idle delay finished.");
        }
    }
}