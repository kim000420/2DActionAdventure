using UnityEngine;
using TutorialBoss.Controller;

namespace TutorialBoss.States.Dok2
{
    public class Dok2ChaseState : BaseTutorialBossState
    {
        public Dok2ChaseState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_Walk"); // Dok2_Run �ִϸ��̼� ���
            controller.rb.velocity = Vector2.zero; // ���� ������ �ܿ� �ӵ� �ʱ�ȭ
        }

        public override void Execute()
        {
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery) return;

            if (controller.player == null)
            {
                // �÷��̾ ���ٸ� Idle ���·� ���ư��ų� ��� (����� Chase ����)
                controller.rb.velocity = Vector2.zero;
                return;
            }

            // �÷��̾ �ٶ�
            controller.FaceToPlayer();

            // �÷��̾���� �Ÿ� ���
            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // ���� ���� üũ
            if (distanceToPlayer <= controller.bossStats.attackRange && !controller.isAttackCooldown)
            {
                // �÷��̾ ���� ���� ���� ���� �ְ�, ��Ÿ���� �ƴ϶�� AttackState�� ��ȯ
                controller.FaceToPlayer(); // ���� ���� �÷��̾ �ٶ󺸵���
                controller.ChangeState(new Dok2AttackState(controller));
                return;
            }
            else
            {
                // �÷��̾ �ν� ���� �ȿ� �ִٸ� �÷��̾� �������� �̵�
                if (distanceToPlayer <= controller.bossStats.detectRange) // Dok2Stats�� detectionRange�� �ִٰ� ����
                {
                    Vector2 direction = (controller.player.position - controller.transform.position).normalized;
                    controller.transform.position += (Vector3)(direction * controller.bossStats.moveSpeed * Time.deltaTime);
                }
            }
        }

        public override void Exit()
        {
            Debug.Log("[Dok2ChaseState] Exit");
        }
    }
}