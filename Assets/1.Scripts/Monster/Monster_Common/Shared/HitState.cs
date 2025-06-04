using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using CommonMonster.Controller; // CommonMonsterController ����
using CommonMonster.States; // BaseMonsterState ����
using CommonMonster.Stats;

namespace CommonMonster.States.Common
{
    public class HitState : BaseMonsterState
    {
        private Vector2 attackerPosition; // �������� ��ġ
        private float knockbackForce;     // ������ �˹� ��
        private Coroutine hitRecoveryCoroutine; // �ǰ� ���� �ڷ�ƾ ����

        // ������: ��Ʈ�ѷ�, ������ ��ġ, �˹� ���� ���ڷ� ����
        public HitState(CommonMonsterController controller, Vector2 attackerPosition, float knockbackForce)
            : base(controller)
        {
            this.attackerPosition = attackerPosition;
            this.knockbackForce = knockbackForce;
        }

        public override void Enter()
        {
            // 1. �ǰ� �ִϸ��̼� ���
            // CommonMonsterController�� monsterName ������ �����Ǿ� �־�� ��
            controller.animator.Play($"{controller.monsterName}_Hit");

            // 2. �˹� ����
            if (controller.rb != null)
            {
                // ���� �ӵ� �ʱ�ȭ (���� �̵��� �˹��� �������� ���� ����)
                controller.rb.velocity = Vector2.zero;

                // �˹� ���� ��� (���� ��ġ - ������ ��ġ)
                Vector2 knockbackDirection = ((Vector2)controller.transform.position - attackerPosition).normalized;

                // ���� ������ ������ �ʾҴٸ� (���� ��ġ), �⺻ ���� (��: ����) ����
                if (knockbackDirection == Vector2.zero)
                {
                    knockbackDirection = Vector2.left;
                }

                // ���� �˹� �߰� (���� ����: ���Ͱ� ���� �ణ Ƣ�� ������ ȿ��)
                // �ʿ信 ���� ���� �˹� ���� �����ϰų� ������ �� �ֽ��ϴ�.
                // ��: knockbackDirection.y = 0.5f; (���� �˹鿡 ���� ���� �˹� ���� ����)
                // ���� �׻� ���� �˹鸸 ���Ѵٸ� knockbackDirection.y = 0; �� normalized

                // �˹� �� ����
                controller.rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // 3. �ǰ� ���� �ڷ�ƾ ����
            // �̹� ���� ���� �ڷ�ƾ�� �ִٸ� �ߴ��ϰ� ���� �����Ͽ� �ߺ� ���� ����
            if (controller.isHitRecovery) // �̹� HitState�� ���������� (��: �ߺ� �ǰ�), �ڷ�ƾ�� ���� ���۵��� �ʾҴٸ�
            {
                if (hitRecoveryCoroutine != null) controller.StopCoroutine(hitRecoveryCoroutine);
            }
            controller.isHitRecovery = true; // ��Ʈ�ѷ��� �ǰ� ���� �÷��� ����
            hitRecoveryCoroutine = controller.StartCoroutine(HitRecoveryRoutine());
        }

        public override void Execute()
        {
            // HitState������ �ַ� �ִϸ��̼� ��� �� �˹� ó���� Enter���� ������,
            // �ڷ�ƾ�� ���� ���� ��ȯ�� �̷�����Ƿ� Execute������ Ư���� ���� ������ ���� �� �ֽ��ϴ�.
            // �ʿ��ϴٸ� �߰����� �ִϸ��̼� ���� Ȯ��, �÷��̾� ���� �ߴ� ���� ���� �� �ֽ��ϴ�.
        }

        public override void Exit()
        {
            // ���� ���� �� ������ٵ� �ӵ� �ʱ�ȭ (���� ����: �˹� �Ŀ��� �������� ���� �� �����Ƿ�)
            if (controller.rb != null)
            {
                controller.rb.velocity = Vector2.zero;
            }

            // �ǰ� ���� �÷��� ���� (�ڷ�ƾ���� �̹� ����������, ���� ��ġ)
            controller.isHitRecovery = false;

            // ���� ���� �ǰ� �ڷ�ƾ�� �ִٸ� ������ �ߴ�
            if (hitRecoveryCoroutine != null)
            {
                controller.StopCoroutine(hitRecoveryCoroutine);
                hitRecoveryCoroutine = null;
            }
        }

        // �ǰ� ���� �ð� �� ���¸� ������� ������ �ڷ�ƾ
        private IEnumerator HitRecoveryRoutine()
        {
            // �˹� �ִϸ��̼� ���� �Ǵ� �˹� ���� ����Ͽ� ���� �ð� ����
            float recoveryDuration = Mathf.Max(0.2f, knockbackForce * 0.05f); // �ּ� 0.2��, �˹� ���� ���

            yield return new WaitForSeconds(recoveryDuration);

            // �ǰ� ���� �÷��� ����
            controller.isHitRecovery = false;

            // ���� �̸��� ���� ������ ���� ���·� ��ȯ
            switch (controller.monsterName)
            {
               // case 0:

            }
            hitRecoveryCoroutine = null; // �ڷ�ƾ ���� ����
        }
    }
}