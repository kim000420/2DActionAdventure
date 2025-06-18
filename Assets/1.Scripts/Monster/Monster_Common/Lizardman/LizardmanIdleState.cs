using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using CommonMonster.Controller;
using CommonMonster.States.Common; // BaseMonsterState ����
using CommonMonster.States.Lizardman; // Lizardman ���� ���� ���� (ChaseState for Lizardman)

namespace CommonMonster.States.Lizardman
{
    public class LizardmanIdleState : BaseMonsterState
    {
        private Coroutine idleDelayRoutine; // Idle ���� ���� �� ������ �ڷ�ƾ
        private bool isDelayFinished = false; // ������ �Ϸ� ����

        // ������
        public LizardmanIdleState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[LizardmanIdleState] Entering Idle State.");
            controller.animator.Play("Lizardman_Idle");

            // ������ ���� ���� �÷��� �ʱ�ȭ
            isDelayFinished = false;
            idleDelayRoutine = controller.StartCoroutine(IdleDelayRoutine());

            // Idle ���� ���� �� �̵� ����
            controller.rb.velocity = Vector2.zero;
        }

        public override void Execute()
        {
            // ���, �׷α�, �ǰ� ���� �߿��� ���� ���� ������ �������� ����
            if (controller.isDead || controller.isGroggy || controller.isHitRecovery)
            {
                return;
            }

            // �����̰� ������ �ʾ����� ���� ���·� �������� ����
            if (!isDelayFinished)
            {
                return;
            }

            // �÷��̾� �ν� ���� Ȯ�� (ChaseState�� ���� ����)
            float distanceToPlayer = Vector2.Distance(controller.transform.position, controller.player.position);

            // �÷��̾ �ν� ���� �ȿ� ������ ChaseState�� ����
            if (distanceToPlayer <= controller.monsterStats.detectionRange)
            {
                Debug.Log("[LizardmanIdleState] Player detected. Transitioning to ChaseState.");
                controller.ChangeState(new LizardmanChaseState(controller)); // ���� �ܰ迡�� ������ LizardmanChaseState
            }
        }

        public override void Exit()
        {
            Debug.Log("[LizardmanIdleState] Exiting Idle State.");
            // ������ �ڷ�ƾ�� ���� ���� ���̶�� ������ �ߴ�
            if (idleDelayRoutine != null)
            {
                controller.StopCoroutine(idleDelayRoutine);
                idleDelayRoutine = null;
            }
            isDelayFinished = false; // �÷��� �ʱ�ȭ
        }

        // Idle �ִϸ��̼� �ð� + �߰� �����̸� ���� �ڷ�ƾ
        private IEnumerator IdleDelayRoutine()
        {
            // 1�ʰ� ���� ���� �Ұ��� ������
            yield return new WaitForSeconds(1.0f);

            isDelayFinished = true; // ������ �Ϸ� �÷��� ����
            Debug.Log("[LizardmanIdleState] Idle delay finished.");
        }
    }
}