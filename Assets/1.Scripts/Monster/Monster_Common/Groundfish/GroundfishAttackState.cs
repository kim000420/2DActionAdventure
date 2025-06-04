using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States;
using System.Collections; // Coroutine ����� ���� �߰�

namespace CommonMonster.States.Groundfish
{
    public class GroundfishAttackState : BaseMonsterState
    {
        public GroundfishAttackState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            // ���� ���� �ִϸ��̼� ���
            controller.animator.Play($"{controller.monsterName}_Attack"); // �Ǵ� Groundfish_AttackJump ��

            // ������ ������ ������ ����
            PerformJumpAttack();
        }

        public override void Execute()
        {
            // ���鿡 �����ߴ��� ���������� Ȯ��
            if (controller.IsGrounded())
            {
                // ���� �� ���� ��Ÿ�� ���� �� IdleState�� ��ȯ
                controller.StartAttackCooldown(controller.monsterStats.jumpCooldown); // ���� ��Ÿ���� ���� ��Ÿ������ ���
                controller.ChangeState(new GroundfishIdleState(controller));
            }
        }

        public override void Exit()
        {
            Debug.Log($"[{controller.monsterName} AttackState] Exit");
            // ���� ���� ���� �� �ʿ信 ���� �ӵ� �ʱ�ȭ ��
            controller.rb.velocity = Vector2.zero; // ���� �� �ٷ� ���ߵ���
        }

        private void PerformJumpAttack()
        {
            if (controller.rb == null || controller.monsterStats == null) return;

            // ���� ���Ͱ� �ٶ󺸴� ���� (SpriteRenderer�� LocalScale.x ��ȣ)
            float horizontalFacingDirection = Mathf.Sign(controller.transform.localScale.x);

            // Y��(����)�� �������� -45������ +45�� ������ ���� ���� ����
            // ��, ���� ��(X��) �������� 45�� ~ 135�� ������ ������ �ǹ��մϴ�.
            // 45��: �մ밢�� ��, 90��: ���� ����, 135��: �ڴ밢�� ��
            float angleFromVerticalCenter = Random.Range(-45f, 45f); // Y�� ���� ��� ����
            float absoluteAngleFromHorizontal = 90f + angleFromVerticalCenter; // X�� ���� ���� ����

            // ������ �������� ��ȯ
            float angleRad = absoluteAngleFromHorizontal * Mathf.Deg2Rad;

            // ������ ���� ���� ���
            // cos(angleRad)�� angleFromVerticalCenter�� -45�� �� ���(������), 45�� �� ����(����)
            // �̸� ���Ͱ� �ٶ󺸴� ���⿡ ���� ���մϴ�.
            float jumpX = Mathf.Cos(angleRad) * controller.monsterStats.jumpForce * horizontalFacingDirection;

            // ������ ���� ���� ���
            // sin(angleRad)�� 45��~135�� �������� �׻� ��� (����)
            float jumpY = Mathf.Sin(angleRad) * controller.monsterStats.jumpForce;

            // ���� ���� ���� ����
            Vector2 jumpVelocity = new Vector2(jumpX, jumpY);

            // Rigidbody�� ���� �� ����
            // ���� �ӵ� �ʱ�ȭ (���� ������ �̵��� ������ ����)
            controller.rb.velocity = Vector2.zero;
            controller.rb.AddForce(jumpVelocity, ForceMode2D.Impulse);}
    }
}