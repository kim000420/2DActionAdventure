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
                controller.animator.Play("Lizardman_Jump"); // ⭐ 변경: 점프 애니메이션 이름 ⭐
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
        }

        // ⭐ 변경: 투사체 관련 데이터를 인자로 받음 ⭐
        public void LaunchFireball(GameObject projectilePrefab, Transform shootPoint, float projectileSpeed, float projectileArcFactor)
        {
            if (projectilePrefab == null || shootPoint == null || controller.player == null)
            {
                Debug.LogWarning("[LizardmanRangedAttackState] Fireball components (Prefab/ShootPoint/Player) not assigned!");
                return;
            }

            Debug.Log("[LizardmanRangedAttackState] Launching Fireball.");

            Vector2 startPosition = shootPoint.position;
            Vector2 targetPosition = controller.player.position;

            GameObject newProjectile = GameObject.Instantiate(projectilePrefab, startPosition, Quaternion.identity);
            Rigidbody2D projectileRb = newProjectile.GetComponent<Rigidbody2D>();

            if (projectileRb == null)
            {
                Debug.LogError("[LizardmanRangedAttackState] Projectile prefab does not have a Rigidbody2D!");
                GameObject.Destroy(newProjectile);
                return;
            }

            float displacementX = targetPosition.x - startPosition.x;
            float displacementY = targetPosition.y - startPosition.y;
            float gravity = Physics2D.gravity.y * projectileRb.gravityScale;

            float velocityX = displacementX / (Mathf.Abs(displacementX) / projectileSpeed);
            float velocityY = (displacementY + Mathf.Abs(displacementX) * projectileArcFactor) / (Mathf.Abs(displacementX) / projectileSpeed);

            projectileRb.velocity = new Vector2(velocityX, velocityY);
        }

        public void ApplyJumpForce()
        {
            if (controller.rb == null || controller.monsterStats == null) return;

            Debug.Log("[LizardmanRangedAttackState] Applying jump force for JumpAttack.");

            controller.rb.velocity = new Vector2(controller.rb.velocity.x, 0);
            controller.rb.AddForce(Vector2.up * controller.monsterStats.jumpForce, ForceMode2D.Impulse);
        }
    }
}