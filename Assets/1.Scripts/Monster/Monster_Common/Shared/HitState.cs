using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using CommonMonster.Controller; // CommonMonsterController 참조
using CommonMonster.States;
using CommonMonster.Stats;
using CommonMonster.States.Groundfish;

namespace CommonMonster.States.Common
{
    public class HitState : BaseMonsterState
    {
        private Vector2 attackerPosition; // 공격자의 위치
        private float knockbackForce;     // 적용할 넉백 힘
        private Coroutine hitRecoveryCoroutine; // 피격 경직 코루틴 참조

        // 생성자: 컨트롤러, 공격자 위치, 넉백 힘을 인자로 받음
        public HitState(CommonMonsterController controller, Vector2 attackerPosition, float knockbackForce)
            : base(controller)
        {
            this.attackerPosition = attackerPosition;
            this.knockbackForce = knockbackForce;
        }

        public override void Enter()
        {
            // 1. 피격 애니메이션 재생
            // CommonMonsterController에 monsterName 변수가 설정되어 있어야 함
            switch(controller.monsterName)
            {
                case "Groundfish":
                    break;
                case "Lizardman":
                    controller.animator.Play($"{controller.monsterName}_Hit");
                    break;
                case "Forg":
                    controller.animator.Play($"{controller.monsterName}_Hit");
                    break;
            }    

            // 2. 넉백 적용
            if (controller.rb != null)
            {
                // 넉백 방향 계산 (몬스터 위치 - 공격자 위치)
                Vector2 knockbackDirection = ((Vector2)controller.transform.position - attackerPosition).normalized;
                // 현재 속도 초기화 (기존 이동에 넉백이 더해지는 것을 방지)
                controller.rb.velocity = Vector2.zero;
                // 넉백 힘 적용
                controller.rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // 3. 피격 경직 코루틴 시작
            // 이미 실행 중인 코루틴이 있다면 중단하고 새로 시작하여 중복 실행 방지
            if (controller.isHitRecovery) // 이미 HitState에 진입했지만 (예: 중복 피격), 코루틴이 새로 시작되지 않았다면
            {
                if (hitRecoveryCoroutine != null) controller.StopCoroutine(hitRecoveryCoroutine);
            }
            controller.isHitRecovery = true; // 컨트롤러의 피격 경직 플래그 설정
            hitRecoveryCoroutine = controller.StartCoroutine(HitRecoveryRoutine());
        }

        public override void Execute()
        {
            // HitState에서는 주로 애니메이션 재생 및 넉백 처리가 Enter에서 끝나고,
            // 코루틴에 의해 상태 전환이 이루어지므로 Execute에서는 특별한 지속 로직이 없을 수 있습니다.
            // 필요하다면 추가적인 애니메이션 상태 확인, 플레이어 추적 중단 등을 넣을 수 있습니다.
        }

        public override void Exit()
        {
            // 실행 중인 피격 코루틴이 있다면 강제로 중단
            if (hitRecoveryCoroutine != null)
            {
                controller.StopCoroutine(hitRecoveryCoroutine);
                hitRecoveryCoroutine = null;
            }
        }

        // 피격 경직 시간 후 상태를 원래대로 돌리는 코루틴
        private IEnumerator HitRecoveryRoutine()
        {
            // 넉백 애니메이션 길이 또는 넉백 힘에 비례하여 경직 시간 조절
            float recoveryDuration = Mathf.Max(0.2f, knockbackForce * 0.1f); // 최소 0.2초, 넉백 힘에 비례

            yield return new WaitForSeconds(recoveryDuration);

            // 피격 경직 플래그 해제
            controller.isHitRecovery = false;

            // 몬스터 이름에 따라 적절한 다음 상태로 전환
            switch (controller.monsterName)
            {
                case "Groundfish":
                    controller.ChangeState(new GroundfishIdleState(controller));
                    break;
                case "Lizardman":
                    break;
                case "Forg":
                    break;
            }
            hitRecoveryCoroutine = null; // 코루틴 참조 해제
        }
    }
}