using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using TutorialBoss.Controller;
using TutorialBoss.States; // BaseTutorialBossState ����

namespace TutorialBoss.States.Webuin
{
    public class WebuinAttackState : BaseTutorialBossState
    {
        public WebuinAttackState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            if (!controller.isAttackCooldown)
            {
                controller.animator.Play("Webuin_Attack");
            }
            else
            {
                // ��Ÿ�� ���̶�� ��� ChaseState�� ��ȯ (��ġ �ʴ� ���� ����)
                controller.ChangeState(new WebuinChaseState(controller));
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