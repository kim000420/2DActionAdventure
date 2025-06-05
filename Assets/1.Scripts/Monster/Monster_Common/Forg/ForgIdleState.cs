using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using CommonMonster.Controller;
using CommonMonster.States.Common; // IdleState, ChaseState, GroggyState 등 공통 상태 참조
using CommonMonster.States.Forg; // Forg 전용 상태 참조 (ChaseState for Forg)

namespace CommonMonster.States.Forg
{
    public class ForgIdleState : BaseMonsterState
    {
        private Coroutine idleDelayRoutine; // Idle 상태 진입 시 딜레이 코루틴
        private bool isDelayFinished = false; // 딜레이 완료 여부

        public ForgIdleState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[ForgIdleState] Entering Idle State.");
            controller.animator.Play("Forg_Idle"); // Forg의 Idle 애니메이션 재생

            // 딜레이 시작 전에 플래그 초기화
            isDelayFinished = false;
            // 딜레이 코루틴 시작
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
            if (distanceToPlayer <= controller.monsterStats.detectionRange)
            {
                // ChaseState로 전이 (ChaseState는 다음 단계에서 구현)
                // 현재는 임시로 Debug.Log만. 실제 구현 시 ForgChaseState로 변경
                controller.ChangeState(new ForgChaseState(controller)); // 다음 단계에서 생성할 ForgChaseState
            }
        }

        public override void Exit()
        {
            Debug.Log("[ForgIdleState] Exiting Idle State.");
            // 딜레이 코루틴이 아직 실행 중이라면 중단
            if (idleDelayRoutine != null)
            {
                controller.StopCoroutine(idleDelayRoutine);
                idleDelayRoutine = null;
            }
            isDelayFinished = false; // 플래그 초기화
        }

        // 1초간 가만히 있는 딜레이 코루틴
        private IEnumerator IdleDelayRoutine()
        {
            yield return new WaitForSeconds(1.0f); // 1초 대기
            isDelayFinished = true; // 딜레이 완료 플래그 설정
            Debug.Log("[ForgIdleState] Idle delay finished. State transition possible.");
        }
    }
}