using UnityEngine;
using TutorialBoss.Controller;
using TutorialBoss.States; // BaseTutorialBossState ����

namespace TutorialBoss.States.Webuin
{
    public class WebuinChaseState : BaseTutorialBossState
    {
        public WebuinChaseState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[WebuinChaseState] Entering Chase State.");
            controller.animator.Play("Webuin_ChaseWalk"); // Webuin�� �߰�(�޸���) �ִϸ��̼� ���
            controller.rb.velocity = Vector2.zero; // ���� ������ �ܿ� �ӵ� �ʱ�ȭ
        }

        public override void Execute()
        {
            // ������ ����, �׷α�, �ǰ� ���� ���¿����� ���� ���� ������ �������� �ʽ��ϴ�.
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;

            if (controller.player == null)
            {
                // �÷��̾ ���ٸ� Idle ���·� ���ư��ų� ��� (����� Chase ����)
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // ���� ��������Ʈ ���� ��ȯ
            controller.FaceToPlayer();

            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // ���� ���� üũ
            if (distanceToPlayer <= controller.bossStats.attackRange && !controller.isAttackCooldown)
            {
                // �÷��̾ ���� ���� ���� ���� �ְ�, ��Ÿ���� �ƴ϶�� AttackState�� ��ȯ
                controller.FaceToPlayer(); // ���� ���� �÷��̾ �ٶ󺸵���
                controller.ChangeState(new WebuinAttackState(controller));
                return;
            }
            else
            {
                // �÷��̾ �ν� ���� �ȿ� �ִٸ� �÷��̾� �������� �̵�
                if (distanceToPlayer <= controller.bossStats.detectRange) // Dok2Stats�� detectionRange�� �ִٰ� ����
                {
                    Vector2 moveDirection = controller.player.position - controller.transform.position;
                    controller.rb.velocity = new Vector2(moveDirection.x * controller.bossStats.moveSpeed, controller.rb.velocity.y);
                }
            }
        }

        public override void Exit()
        {
            Debug.Log("[WebuinChaseState] Exit");
            controller.rb.velocity = Vector2.zero; // ���� ���� �� �̵� ����
        }
    }
}