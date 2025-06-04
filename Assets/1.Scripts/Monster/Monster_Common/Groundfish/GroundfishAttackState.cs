using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States;
using System.Collections; // Coroutine 사용을 위해 추가

namespace CommonMonster.States.Groundfish
{
    public class GroundfishAttackState : BaseMonsterState
    {
        public GroundfishAttackState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            // 점프 공격 애니메이션 재생
            controller.animator.Play($"{controller.monsterName}_Attack"); // 또는 Groundfish_AttackJump 등

            // 랜덤한 각도와 힘으로 점프
            PerformJumpAttack();
        }

        public override void Execute()
        {
            // 지면에 착지했는지 지속적으로 확인
            if (controller.IsGrounded())
            {
                // 착지 시 공격 쿨타임 시작 및 IdleState로 전환
                controller.StartAttackCooldown(controller.monsterStats.jumpCooldown); // 점프 쿨타임을 공격 쿨타임으로 사용
                controller.ChangeState(new GroundfishIdleState(controller));
            }
        }

        public override void Exit()
        {
            Debug.Log($"[{controller.monsterName} AttackState] Exit");
            // 공격 상태 종료 시 필요에 따라 속도 초기화 등
            controller.rb.velocity = Vector2.zero; // 착지 후 바로 멈추도록
        }

        private void PerformJumpAttack()
        {
            if (controller.rb == null || controller.monsterStats == null) return;

            // 현재 몬스터가 바라보는 방향 (SpriteRenderer의 LocalScale.x 부호)
            float horizontalFacingDirection = Mathf.Sign(controller.transform.localScale.x);

            // Y축(수직)을 기준으로 -45도에서 +45도 사이의 랜덤 각도 선택
            // 즉, 수평 축(X축) 기준으로 45도 ~ 135도 사이의 각도를 의미합니다.
            // 45도: 앞대각선 위, 90도: 순수 수직, 135도: 뒤대각선 위
            float angleFromVerticalCenter = Random.Range(-45f, 45f); // Y축 기준 상대 각도
            float absoluteAngleFromHorizontal = 90f + angleFromVerticalCenter; // X축 기준 절대 각도

            // 각도를 라디안으로 변환
            float angleRad = absoluteAngleFromHorizontal * Mathf.Deg2Rad;

            // 점프의 수평 성분 계산
            // cos(angleRad)는 angleFromVerticalCenter가 -45일 때 양수(오른쪽), 45일 때 음수(왼쪽)
            // 이를 몬스터가 바라보는 방향에 맞춰 곱합니다.
            float jumpX = Mathf.Cos(angleRad) * controller.monsterStats.jumpForce * horizontalFacingDirection;

            // 점프의 수직 성분 계산
            // sin(angleRad)는 45도~135도 범위에서 항상 양수 (위쪽)
            float jumpY = Mathf.Sin(angleRad) * controller.monsterStats.jumpForce;

            // 최종 점프 방향 벡터
            Vector2 jumpVelocity = new Vector2(jumpX, jumpY);

            // Rigidbody에 점프 힘 적용
            // 기존 속도 초기화 (이전 점프나 이동의 관성을 제거)
            controller.rb.velocity = Vector2.zero;
            controller.rb.AddForce(jumpVelocity, ForceMode2D.Impulse);}
    }
}