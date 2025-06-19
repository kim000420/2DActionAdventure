using UnityEngine;
using System.Collections;
using CommonMonster.Controller;
using CommonMonster.States.Common;
using CommonMonster.States.Lizardman;
using CommonMonster.Projectiles;

namespace CommonMonster.States.Lizardman
{
    public class LizardmanRangedAttackState : BaseMonsterState
    {
        private bool isPerformingJumpAttack = false;
        private Coroutine jumpAttackCoroutine;

        [Header("Fireball Settings")] // Fireball 관련 설정 ⭐
        [SerializeField] private float launchAngleDegrees = 45f; // 발사 각도 (도), 조절 필요
        [SerializeField] private float projectileMaxSpeed = 15f; // 투사체 최대 속도 (속도 제한용, 너무 빠르지 않게)


        public LizardmanRangedAttackState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[LizardmanRangedAttackState] Entering Ranged Attack State.");

            controller.rb.velocity = Vector2.zero;
            controller.FaceToPlayer();

            if (Random.value < 0.5f)
            {
                Debug.Log("[LizardmanRangedAttackState] Performing Fireball Attack.");
                controller.animator.Play("Lizardman_Fireball"); // Fireball 애니메이션 재생
                isPerformingJumpAttack = false;
            }
            else
            {
                Debug.Log("[LizardmanRangedAttackState] Performing Jump Attack.");
                jumpAttackCoroutine = controller.StartCoroutine(HandleJumpAttackRoutine());
                isPerformingJumpAttack = true;
            }
        }

        public override void Execute()
        {
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                controller.rb.velocity = Vector2.zero;
                return;
            }
        }

        public override void Exit()
        {
            Debug.Log("[LizardmanRangedAttackState] Exiting Ranged Attack State.");

            isPerformingJumpAttack = false;

            if (jumpAttackCoroutine != null)
            {
                controller.StopCoroutine(jumpAttackCoroutine);
                jumpAttackCoroutine = null;
            }
        }

        public void LaunchFireball(GameObject projectilePrefab, Transform shootPoint) // 매개변수 변경
        {
            if (projectilePrefab == null || shootPoint == null || controller.player == null)
            {
                Debug.LogWarning("[LizardmanRangedAttackState] Fireball components (Prefab/ShootPoint/Player) not assigned!");
                return;
            }

            Debug.Log("[LizardmanRangedAttackState] Launching Fireball.");

            Vector2 startPosition = shootPoint.position;
            Vector2 targetPosition = controller.player.position;

            // ⭐ CalculateProjectileVelocity 호출 ⭐
            Vector2 initialVelocity = CalculateProjectileVelocity(startPosition, targetPosition, launchAngleDegrees, Physics2D.gravity.y);

            if (initialVelocity == Vector2.zero) // 도달 불가능한 경우 처리
            {
                Debug.LogWarning("[LizardmanRangedAttackState] 투사체 발사 속도를 계산할 수 없습니다. 목표 도달 불가능하거나 너무 멀리 있습니다.");
                return;
            }

            // ⭐ 계산된 속도에 최대 속도 제한 적용 (선택 사항이지만 안전성 증가) ⭐
            if (initialVelocity.magnitude > projectileMaxSpeed)
            {
                initialVelocity = initialVelocity.normalized * projectileMaxSpeed;
                Debug.LogWarning($"[LizardmanRangedAttackState] Calculated velocity was too high. Clamping to max speed: {projectileMaxSpeed}");
            }

            // 투사체 생성
            GameObject newProjectile = GameObject.Instantiate(projectilePrefab, startPosition, Quaternion.identity);
            Rigidbody2D projectileRb = newProjectile.GetComponent<Rigidbody2D>();

            if (projectileRb == null)
            {
                Debug.LogError("[LizardmanRangedAttackState] Projectile prefab does not have a Rigidbody2D! Destroying projectile.");
                GameObject.Destroy(newProjectile);
                return;
            }

            projectileRb.velocity = initialVelocity;
            Debug.Log($"[LizardmanRangedAttackState] Fireball Launched! Calculated Initial Velocity: {initialVelocity}");
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

        private IEnumerator HandleJumpAttackRoutine()
        {
            controller.animator.Play("Lizardman_Jump"); // 점프 애니메이션 재생
            Debug.Log("[LizardmanRangedAttackState] Playing Lizardman_Jump animation.");

            ApplyJumpForce();
            Debug.Log("[LizardmanRangedAttackState] Applied jump force.");

            // 점프 시작후 0.5초간 착지감지 방지
            yield return new WaitForSeconds(0.4f);

            // 착지 감지
            // 점프 애니메이션이 끝나고, 지면에 닿을 때까지 대기
            yield return new WaitUntil(() => controller.IsGrounded() && Mathf.Abs(controller.rb.velocity.y) < 0.1f); // 거의 멈췄을 때
            Debug.Log("[LizardmanRangedAttackState] Landed from jump attack.");

            // 착지 후 이동 종료
            controller.rb.velocity = Vector2.zero;
            // 착지 후 Attack1 애니메이션 재생
            controller.animator.Play("Lizardman_Attack1");
        }

        public void ApplyJumpForce()
        {
            if (controller.rb == null || controller.monsterStats == null) return;

            Debug.Log("[LizardmanRangedAttackState] Applying jump force for JumpAttack.");

            float horizontalDirection = Mathf.Sign(controller.transform.localScale.x); // 현재 몬스터가 바라보는 방향
            float jumpHorizontalSpeed = controller.monsterStats.moveSpeed * 1.35f; // 점프 중 이동 속도 (조절 가능)

            controller.rb.velocity = new Vector2(-jumpHorizontalSpeed * horizontalDirection, 0); // 수평 속도 설정, Y는 0으로 초기화
            controller.rb.AddForce(Vector2.up * controller.monsterStats.jumpForce, ForceMode2D.Impulse); // 점프 힘 적용
        }
    }
}