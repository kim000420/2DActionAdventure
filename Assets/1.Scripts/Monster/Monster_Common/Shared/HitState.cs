using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using CommonMonster.Controller; // CommonMonsterController 참조
using CommonMonster.States;
using CommonMonster.Stats;
using CommonMonster.States.Groundfish;
using CommonMonster.States.Lizardman;
using CommonMonster.States.Forg;

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
            // 넉백 방향 계산 (몬스터 위치 - 공격자 위치)
            Vector2 knockbackDirection = ((Vector2)controller.transform.position - attackerPosition).normalized;
            // X축 넉백 방향만 사용 (수직 넉백은 점프 관련 로직에서 별도 처리하거나, 아예 없애는 경우가 많음)
            knockbackDirection.y = 0;
            knockbackDirection.Normalize(); // X축만 남기고 다시 정규화

            // ⭐ 넉백량 공식: 넉백량 X 0.2 ⭐
            float knockbackDistance = knockbackForce * 0.2f;
            float knockbackDuration = 0.1f; // 0.1초 동안 넉백 이동
            float elapsedKnockbackTime = 0f;

            Vector2 initialPosition = controller.transform.position;
            Vector2 targetKnockbackPosition = initialPosition + knockbackDirection * knockbackDistance;

            controller.rb.velocity = Vector2.zero; // 넉백 시작 시 기존 속도 초기화

            // 0.1초 동안 넉백 이동
            while (elapsedKnockbackTime < knockbackDuration)
            {
                // 시간에 따라 넉백 목표 위치로 보간
                controller.rb.MovePosition(Vector2.Lerp(initialPosition, targetKnockbackPosition, elapsedKnockbackTime / knockbackDuration));
                // Rigidbody2D.MovePosition은 물리 업데이트 사이에서 호출되어야 하므로 WaitForFixedUpdate 사용
                yield return new WaitForFixedUpdate();
                elapsedKnockbackTime += Time.fixedDeltaTime;
            }
            // 넉백 이동이 끝난 후 정확한 위치로 설정
            controller.rb.MovePosition(targetKnockbackPosition);
            controller.rb.velocity = Vector2.zero; // 넉백 후 속도 0으로 초기화


            // ⭐ 피격 경직 시간: 넉백량에 비례 (넉백량 * 0.2f) ⭐
            // 최소 경직 시간을 설정하여 너무 짧게 끝나지 않도록 방지
            float hitStunDuration = Mathf.Max(0.1f, knockbackForce * 0.4f);
            Debug.Log($"[HitState] Hit Stun Duration: {hitStunDuration} seconds (Knockback Force: {knockbackForce})");

            // 경직 시간 동안 대기 (넉백 이동 후부터 경직 시간이 시작됨)
            yield return new WaitForSeconds(hitStunDuration);

            // 피격 경직 플래그 해제
            controller.isHitRecovery = false;

            // 몬스터 이름에 따라 적절한 다음 상태로 전환
            // 각 몬스터의 Idle 애니메이션 재생 및 해당 IdleState로 전환
            switch (controller.monsterName)
            {
                case "Groundfish":
                    controller.animator.Play("Groundfish_Idle"); // Idle 애니메이션 재생
                    controller.ChangeState(new GroundfishIdleState(controller));
                    break;
                case "Lizardman":
                    controller.animator.Play("Lizardman_Idle"); // Idle 애니메이션 재생
                    controller.ChangeState(new LizardmanIdleState(controller));
                    break;
                case "Forg":
                    controller.animator.Play("Forg_Idle"); // Idle 애니메이션 재생
                    controller.ChangeState(new ForgIdleState(controller));
                    break;
                default:
                    controller.rb.velocity = Vector2.zero; // 혹시 모를 잔여 속도 제거
                    break;
            }
            controller.rb.velocity = Vector2.zero;
            hitRecoveryCoroutine = null; // 코루틴 참조 해제
        }
    }
}