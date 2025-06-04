using UnityEngine;
using TutorialBoss.Controller;
using System.Collections; // Coroutine ����� ���� �߰�

namespace TutorialBoss.States.Dok2
{
    public class Dok2AttackState : BaseTutorialBossState
    {
        public Dok2AttackState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.rb.velocity = Vector2.zero; // ���� �� �̵� ����

            // ���� ��Ÿ�� ���� �ƴ϶�� ���� ����
            if (!controller.isAttackCooldown)
            {
                if (controller.attack1Count >= 3)
                {
                    // Attack1�� 3�� ����ߴٸ� Attack2 ��� �� ���� �ʱ�ȭ
                    controller.animator.Play($"{controller.bossName}_Attack2");
                    controller.attack1Count = 0; // ���� �ʱ�ȭ
                }
                else
                {
                    // Attack1 ���
                    controller.animator.Play($"{controller.bossName}_Attack1");
                    controller. attack1Count++;
                }
            }
            else
            {
                // ��Ÿ�� ���̶�� ��� ChaseState�� ��ȯ (��ġ �ʴ� ���� ����)
                controller.ChangeState(new Dok2ChaseState(controller));
            }
        }

        public override void Execute()
        {
        
        }

        public override void Exit()
        {
        }
    }
}