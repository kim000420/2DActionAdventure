using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using CommonMonster.Controller; // CommonMonsterController ����
using CommonMonster.States;
using CommonMonster.Stats;
using CommonMonster.States.Groundfish;

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
            switch(controller.monsterName)
            {
                case "Groundfish":
                    break;
                case "Lizardman":
                    controller.animator.Play($"{controller.monsterName}_Hit");
                    break;
                case "Forg":
                    controller.animator.Play($"{controller.monsterName}_Hit");
                    break;
            }    

            // 2. �˹� ����
            if (controller.rb != null)
            {
                // �˹� ���� ��� (���� ��ġ - ������ ��ġ)
                Vector2 knockbackDirection = ((Vector2)controller.transform.position - attackerPosition).normalized;
                // ���� �ӵ� �ʱ�ȭ (���� �̵��� �˹��� �������� ���� ����)
                controller.rb.velocity = Vector2.zero;
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
            float recoveryDuration = Mathf.Max(0.2f, knockbackForce * 0.1f); // �ּ� 0.2��, �˹� ���� ���

            yield return new WaitForSeconds(recoveryDuration);

            // �ǰ� ���� �÷��� ����
            controller.isHitRecovery = false;

            // ���� �̸��� ���� ������ ���� ���·� ��ȯ
            switch (controller.monsterName)
            {
                case "Groundfish":
                    controller.ChangeState(new GroundfishIdleState(controller));
                    break;
                case "Lizardman":
                    break;
                case "Forg":
                    break;
            }
            hitRecoveryCoroutine = null; // �ڷ�ƾ ���� ����
        }
    }
}