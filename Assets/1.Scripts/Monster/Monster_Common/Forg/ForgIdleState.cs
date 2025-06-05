using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using CommonMonster.Controller;
using CommonMonster.States.Common; // IdleState, ChaseState, GroggyState �� ���� ���� ����
using CommonMonster.States.Forg; // Forg ���� ���� ���� (ChaseState for Forg)

namespace CommonMonster.States.Forg
{
    public class ForgIdleState : BaseMonsterState
    {
        private Coroutine idleDelayRoutine; // Idle ���� ���� �� ������ �ڷ�ƾ
        private bool isDelayFinished = false; // ������ �Ϸ� ����

        public ForgIdleState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log("[ForgIdleState] Entering Idle State.");
            controller.animator.Play("Forg_Idle"); // Forg�� Idle �ִϸ��̼� ���

            // ������ ���� ���� �÷��� �ʱ�ȭ
            isDelayFinished = false;
            // ������ �ڷ�ƾ ����
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
            if (distanceToPlayer <= controller.monsterStats.detectionRange)
            {
                // ChaseState�� ���� (ChaseState�� ���� �ܰ迡�� ����)
                // ����� �ӽ÷� Debug.Log��. ���� ���� �� ForgChaseState�� ����
                controller.ChangeState(new ForgChaseState(controller)); // ���� �ܰ迡�� ������ ForgChaseState
            }
        }

        public override void Exit()
        {
            Debug.Log("[ForgIdleState] Exiting Idle State.");
            // ������ �ڷ�ƾ�� ���� ���� ���̶�� �ߴ�
            if (idleDelayRoutine != null)
            {
                controller.StopCoroutine(idleDelayRoutine);
                idleDelayRoutine = null;
            }
            isDelayFinished = false; // �÷��� �ʱ�ȭ
        }

        // 1�ʰ� ������ �ִ� ������ �ڷ�ƾ
        private IEnumerator IdleDelayRoutine()
        {
            yield return new WaitForSeconds(1.0f); // 1�� ���
            isDelayFinished = true; // ������ �Ϸ� �÷��� ����
            Debug.Log("[ForgIdleState] Idle delay finished. State transition possible.");
        }
    }
}