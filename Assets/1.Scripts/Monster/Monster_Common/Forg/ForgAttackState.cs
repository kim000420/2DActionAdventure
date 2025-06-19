using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common;
using System.Collections; // Coroutine 사용을 위해 추가

namespace CommonMonster.States.Forg
{
    public class ForgAttackState : BaseMonsterState
    {
        private float attackCooldown;         // 공격 후 다음 공격까지의 쿨타임
        public float launchAngleDegrees = 60f; // 발사각도 / 기본 45도

        // 공격 애니메이션 지속 시간과 쿨타임을 외부에서 받도록 생성자 수정
        public ForgAttackState(CommonMonsterController controller, float cooldown) : base(controller)
        {
            this.attackCooldown = cooldown;
        }

        public override void Enter()
        {
            Debug.Log("[ForgAttackState] Entering Attack State.");
            controller.rb.velocity = Vector2.zero; // 공격 중에는 이동 정지
            controller.FaceToPlayer(); // 공격 직전에 다시 한번 플레이어 방향 바라보기

            controller.animator.Play("Forg_Attack"); // Forg 공격 애니메이션 재생
        }

        public override void Execute()
        {
            // 사망, 그로기, 피격 경직 중에는 로직 실행 안 함
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                controller.rb.velocity = Vector2.zero;
                return;
            }
            // 공격 애니메이션이 재생되는 동안에는 특별한 로직이 없을 수 있습니다.
            // 이 상태의 주된 목적은 공격 애니메이션을 재생하고 투사체를 발사하는 것입니다.
        }

        public override void Exit()
        {
            Debug.Log("[ForgAttackState] Exiting Attack State.");
        }

        public void LaunchProjectile()
        {
            if (controller.forgProjectilePrefab == null || controller.forgShootPoint == null || controller.player == null)
            {
                Debug.LogWarning("[ForgAttackState] 투사체 발사에 필요한 컴포넌트가 부족합니다. (Prefab, ShootPoint, Player)");
                return;
            }

            // 발사 지점과 목표 지점
            Vector2 startPos = controller.forgShootPoint.position;
            Vector2 targetPos = controller.player.position;

            // Unity의 중력 값 (Physics2D.gravity.y)
            float gravity = Physics2D.gravity.y; // 일반적으로 음수 값

            // 몬스터가 바라보는 방향 (투사체 발사 방향 결정에 사용)
            float directionX = Mathf.Sign(targetPos.x - startPos.x); // 플레이어가 오른쪽에 있으면 1, 왼쪽에 있으면 -1

            // 플레이어와의 거리 계산
            float distanceX = Mathf.Abs(targetPos.x - startPos.x);
            float distanceY = targetPos.y - startPos.y;

            // 라디안으로 각도 변환
            float launchAngleRad = launchAngleDegrees * Mathf.Deg2Rad;

            Vector2 initialVelocity = CalculateProjectileVelocity(startPos, targetPos, launchAngleDegrees, gravity);
            
            if (initialVelocity == Vector2.zero)
            {
                Debug.LogWarning("[ForgAttackState] 투사체 발사 속도를 계산할 수 없습니다. 목표 도달 불가능.");
                return;
            }

            // 투사체 생성
            GameObject projectileGO = GameObject.Instantiate(
                controller.forgProjectilePrefab,
                startPos,
                Quaternion.identity // 초기 회전은 중요하지 않음, Rigidbody2D가 방향을 담당
            );

            // 투사체에 Rigidbody2D가 있다면 속도 적용
            Rigidbody2D projectileRb = projectileGO.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.velocity = initialVelocity;
                Debug.Log($"[ForgAttackState] Forg 투사체 발사 완료! Calculated Velocity: {initialVelocity}");
            }
            else
            {
                Debug.LogWarning("[ForgAttackState] 발사된 투사체에 Rigidbody2D가 없습니다. 속도가 적용되지 않습니다.");
            }
            Debug.Log("[ForgAttackState] Forg 투사체 발사 완료!");
        }
        private Vector2 CalculateProjectileVelocity(Vector2 startPoint, Vector2 targetPoint, float launchAngle, float gravity)
        {
            float angleRad = launchAngle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = targetPoint.x - startPoint.x;
            float y = targetPoint.y - startPoint.y;

            // 목표를 향하는 방향 (x 축)
            int directionX = x > 0 ? 1 : -1;
            x = Mathf.Abs(x); // x는 항상 양수로 계산

            // 공식에 맞게 중력 가속도 G는 양수로 정의 (Unity의 Physics2D.gravity.y는 보통 음수)
            float g = Mathf.Abs(gravity);

            // V0 = sqrt( (g * x^2) / (2 * cos^2(theta) * (x * tan(theta) - y)) )
            float denominator = 2 * Mathf.Pow(Mathf.Cos(angleRad), 2) * (x * Mathf.Tan(angleRad) - y);

            if (denominator <= 0) // 분모가 0이거나 음수면 목표에 도달할 수 없는 각도
            {
                Debug.LogWarning("[CalculateProjectileVelocity] 목표 도달 불가능: 분모가 0 이하입니다.");
                return Vector2.zero; // 도달 불가능
            }

            float v0Squared = (g * Mathf.Pow(x, 2)) / denominator;

            if (v0Squared < 0) // 제곱근 내부가 음수면 허수 속도, 즉 도달 불가능
            {
                Debug.LogWarning("[CalculateProjectileVelocity] 목표 도달 불가능: 초기 속도 제곱값이 음수입니다.");
                return Vector2.zero; // 도달 불가능
            }

            float v0 = Mathf.Sqrt(v0Squared); // 초기 속도 크기

            // X, Y 속도 성분 계산
            float velocityX = v0 * Mathf.Cos(angleRad) * directionX; // 방향 적용
            float velocityY = v0 * Mathf.Sin(angleRad);

            return new Vector2(velocityX, velocityY);
        }

        // ⭐ 새로운 PUBLIC 메서드: Animator Event에서 호출될 공격 종료 처리 메서드 ⭐
        public void OnAttackAnimationEnd()
        {
            Debug.Log("[ForgAttackState] Forg_OnAttackAnimationEnd 호출됨!");

            // 공격 쿨타임 시작
            controller.StartAttackCooldown(attackCooldown);

            // 공격 애니메이션이 끝나고 쿨타임이 시작되면 idle 상태로 돌아감
            controller.ChangeState(new ForgIdleState(controller));
        }
    }
}