using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common;
using CommonMonster.Stats;

namespace CommonMonster.States.Forg
{
    public class ForgChaseState : BaseMonsterState
    {
        private float nextJumpReadyTime; // ���� ������ �õ��� �� �ִ� �ð�

        // ���� ���� �Ӱ谪�� �������� �ű�ų� Forg ���� �������� �����ϴ� ���� �� �����մϴ�.
        // ���⼭�� ���÷� �״�� ������, CommonMonsterStats�� �߰��ϴ� ���� �����մϴ�.
        private float verticalObstacleThreshold = 1.5f; // �÷��̾���� Y�� ���̰� �� �� �̻��̸� ���� ���

        public ForgChaseState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[ForgChaseState] Entering Chase State.");
            controller.animator.Play("Forg_Idle"); // Forg�� Chase ���¿����� �̵� �ִϸ��̼� ���� ��� (������ �̵��ϹǷ�)
                                                   // ���� ���� ª�� �̵� �ִϸ��̼��� �ִٸ� Forg_Run�� ����ص� ��
            controller.rb.velocity = Vector2.zero; // Chase State ���� �� ������ ����
            controller.rb.gravityScale = 1f;

            // ���� ������ ������ �ð��� ���� �ð� + ��Ÿ������ �ʱ�ȭ
            nextJumpReadyTime = Time.time + controller.monsterStats.jumpCooldown;
            controller.isJumping = false; // Ȥ�� �� ��츦 ����Ͽ� ���� �� �÷��� ����
        }

        public override void Execute()
        {
            // ���, �׷α�, �ǰ� ���� �߿��� ���� ���� �� ��
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                controller.rb.velocity = Vector2.zero;
                return;
            }

            controller.FaceToPlayer(); // �÷��̾� �������� �ٶ� (���� �� ���� ���� ���� ����)

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);
            float directionToPlayerX = Mathf.Sign(controller.player.position.x - controller.transform.position.x);
            float yDifference = controller.player.position.y - controller.transform.position.y;

            // 1. ���� ���� �ȿ� ���Դ��� Ȯ�� (���� ���� �ȿ� ���Դٸ� ChaseState���� ���� AttackState�� ����)
            if (distanceToPlayer <= controller.monsterStats.rangedAttackRange && !controller.isAttackCooldown)
            {
                // ForgAttackState�� ���� (�� �κ��� ���� �ܰ迡�� ����)
                controller.ChangeState(new ForgAttackState(controller, 2f));
                return;
            }
            // 2. �ν� ���� ������ �������� Ȯ�� (Idle ���·� ���ư� ����)
            else if (distanceToPlayer > controller.monsterStats.detectionRange)
            {
                controller.ChangeState(new ForgIdleState(controller));
                return;
            }

            // 3. ������ ���� �̵� ����
            // ���� �ð��� ���� ���� ���� �ð����� ũ�ų� ����, ���� ��� �ְ�, ���� ���� �ƴ� ��
            if (Time.time >= nextJumpReadyTime && controller.IsGrounded() && !controller.isJumping)
            {
                bool shouldJump = false;
                Debug.Log("[ForgChaseState] �����غ�ִ�");
                controller.animator.Play("Forg_JumpReady");

                // 3-1. �տ� ���� �ִ��� Ȯ���Ͽ� ����
                if (controller.IsWallAhead(directionToPlayerX))
                {
                    shouldJump = true;
                    Debug.Log("[ForgChaseState] Wall ahead, initiating jump.");
                }
                // 3-2. �÷��̾ Forg���� ���� ��ġ�� �ִ��� Ȯ���Ͽ� ����
                else if (yDifference > verticalObstacleThreshold)
                {
                    shouldJump = true;
                    Debug.Log("[ForgChaseState] Player is above, initiating jump.");
                }
                // 3-3. �ܼ��� �÷��̾�� �ٰ����� ���� ���� (�Ÿ��� �������� �ʾҴٸ� ��� ����)
                // �� ������ �������� �ξ� �ٸ� ���� ���ǿ� �켱������ �ݴϴ�.
                else if (distanceToPlayer > controller.monsterStats.rangedAttackRange)
                {
                    shouldJump = true;
                    Debug.Log("[ForgChaseState] Player too far, initiating jump for chase.");
                }

                if (shouldJump)
                {
                    controller.ChangeState(new ForgChaseJumpState(controller));
                    // ���� ���·� ���������Ƿ�, ���� ���� ���� �ð��� ������Ʈ�� �ʿ䰡 �����ϴ�.
                    // ���� ���� ���� �ð��� ���� �� �ٽ� ChaseState�� ���ƿ��� �� �����˴ϴ�.
                    return; // ���� ���·� ��ȯ�Ǿ����Ƿ� ���� Execute ����
                }
            }
            controller.rb.velocity = new Vector2(0, controller.rb.velocity.y); // ���� �̵��� ����
        }

        public override void Exit()
        {
            Debug.Log("[ForgChaseState] Exiting Chase State.");
        }
    }
}