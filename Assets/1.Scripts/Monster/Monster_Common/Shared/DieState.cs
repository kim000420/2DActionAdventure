using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using CommonMonster.Controller; // CommonMonsterController ����
using CommonMonster.States;     // BaseMonsterState ����

namespace CommonMonster.States.Common
{
    public class DieState : BaseMonsterState
    {
        private Coroutine destroyCoroutine; // ������Ʈ �ı� �ڷ�ƾ ����

        public DieState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log($"[{controller.monsterName} DieState] Enter");

            controller.isDead = true; // ���� ��� �÷��� ����
            controller.rb.velocity = Vector2.zero; // ��� ������ ����
            controller.rb.bodyType = RigidbodyType2D.Kinematic; // ������ ���� ���� (�� �̻� �и��� �ʵ���)
            controller.animator.Play($"{controller.monsterName}_Die"); // ��� �ִϸ��̼� ���

            // (���� ����) �ݶ��̴� ��Ȱ��ȭ: �÷��̾�/�ٸ� ������Ʈ���� �浹 ����
            Collider2D monsterCollider = controller.GetComponent<Collider2D>();
            if (monsterCollider != null)
            {
                monsterCollider.enabled = false;
            }

            // ��� �� ���� �ð� �� ������Ʈ �ı� �ڷ�ƾ ����
            // �ִϸ��̼� ���̿� ���� �ð��� �����ϴ� ���� �����ϴ�.
            float destroyDelay = 2.0f; // ����: ��� �ִϸ��̼� ��� �� 2�� �� �ı�
            // TODO: ���� �ִϸ��̼� ���̿� ���� �����ϰų�, �ִϸ��̼� �̺�Ʈ�� ȣ��ǵ��� ���� ����
            destroyCoroutine = controller.StartCoroutine(DestroyAfterDelay(destroyDelay));
        }

        public override void Execute()
        {
            // ��� ���¿����� Ư���� ���� ������ �ʿ� ����
        }

        public override void Exit()
        {
            Debug.Log($"[{controller.monsterName} DieState] Exit (Should not be called if object is destroyed)");
            // �� ���´� ���� ������Ʈ �ı��� ������ ������ Exit�� ȣ����� ���� �� ����
            // ������ Ȥ�� �� ��츦 ����Ͽ� ���� ���� ����
            if (destroyCoroutine != null)
            {
                controller.StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Debug.Log($"[{controller.monsterName}] GameObject will be destroyed.");
            // ���� ���� ������Ʈ �ı�
            if (controller != null && controller.gameObject != null)
            {
                controller.gameObject.SetActive(false);
            }
        }
    }
}