using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Common; // BaseMonsterState ����
using CommonMonster.States.Lizardman; // Lizardman ���� ���� ���� (MeleeAttackState, RangedAttackState)

namespace CommonMonster.States.Lizardman
{
    public class LizardmanChaseState : BaseMonsterState
    {
        // ������
        public LizardmanChaseState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[LizardmanChaseState] Entering Chase State.");
            // Lizardman�� Run �ִϸ��̼� ���
            // CommonMonsterController�� monsterName�� "Lizardman"���� �����ؾ� ��
            controller.animator.Play("Lizardman_Run");
        }

        public override void Execute()
        {
            // ���, �׷α�, �ǰ� ���� �߿��� ���� ���� ������ �������� ����
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                // ���� ���̴� ������, ���� ������ �̵�/���� ��ȯ�� ���ߵ��� ó��
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // �÷��̾���� �Ÿ� ���
            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // 1. �ν� ������ ����� IdleState�� ����
            if (distanceToPlayer > controller.monsterStats.detectionRange)
            {
                Debug.Log("[LizardmanChaseState] Player out of detection range. Transitioning to IdleState.");
                controller.ChangeState(new LizardmanIdleState(controller));
                return; // ���� ���� ���� ����
            }

            // 2. �÷��̾ ���� �̵� �� ���� ��ȯ
            // CommonMonsterController�� FaceToPlayer()�� moveSpeed�� ���
            // �÷��̾��� x ��ġ�� �������� ������ ������ �����ϰ� �̵�
            float directionToPlayerX = Mathf.Sign(controller.player.position.x - controller.transform.position.x);
            controller.rb.velocity = new Vector2(directionToPlayerX * controller.monsterStats.moveSpeed, controller.rb.velocity.y);
            controller.FaceToPlayer(); // ��������Ʈ ���� ��ȯ

            // 3. ���� ���� üũ �� ���� State�� ���� (�ٰŸ��� �켱)
            // CommonMonsterStats�� meleeAttackRange�� rangedAttackRange�� �����Ǿ� �־�� ��
            if (distanceToPlayer <= controller.monsterStats.meleeAttackRange)
            {
                Debug.Log("[LizardmanChaseState] Player in melee attack range. Transitioning to MeleeAttackState.");
                // ���� �ܰ迡�� ������ LizardmanMeleeAttackState
                controller.ChangeState(new LizardmanMeleeAttackState(controller));
            }
            else if (distanceToPlayer <= controller.monsterStats.rangedAttackRange)
            {
                Debug.Log("[LizardmanChaseState] Player in ranged attack range. Transitioning to RangedAttackState.");
                // ���� �ܰ迡�� ������ LizardmanRangedAttackState
                controller.ChangeState(new LizardmanRangedAttackState(controller));
            }
            // else: ���� ������ ������ �ʾҴٸ� ��� ���� (Execute�� ������ �κ��� ó��)
        }

        public override void Exit()
        {
            Debug.Log("[LizardmanChaseState] Exiting Chase State.");
            // ChaseState ���� �� �̵� ���� (���� ���¿��� �ٽ� �̵��� ������ ����)
            controller.rb.velocity = Vector2.zero;
        }
    }
}