using UnityEngine;
using System.Collections; // Coroutine ����� ���� �߰�
using CommonMonster.Controller; // CommonMonsterController ����
using CommonMonster.States.Lizardman; // LizardmanRangedAttackState ����

// HitboxTrigger ��ũ��Ʈ�� ��� ���ӽ����̽��� �ִ��� Ȯ���ϰ� �ʿ� �� �߰�
// ���� HitboxTrigger�� ���ӽ����̽� ���� ������ �����Ƿ� using UnityEngine; �����ε� ���� ����

namespace CommonMonster.AnimEvents.Lizardman
{
    public class LizardmanAnimatorEvents : MonoBehaviour
    {
        public CommonMonsterController controller;

        [Header("Lizardman Hitboxes")] // �ν����Ϳ��� ���� �Ҵ��� ��Ʈ�ڽ���
        public HitboxTrigger Hitbox_Attack1; // Melee Attack1 �ִϸ��̼ǿ� ��Ʈ�ڽ� ��ũ��Ʈ
        public HitboxTrigger Hitbox_Attack2; // Melee Attack2 �ִϸ��̼ǿ� ��Ʈ�ڽ� ��ũ��Ʈ

        private Coroutine deactivateHitboxCoroutine1; // Attack1 ��Ʈ�ڽ� ��Ȱ��ȭ �ڷ�ƾ ����
        private Coroutine deactivateHitboxCoroutine2; // Attack2 ��Ʈ�ڽ� ��Ȱ��ȭ �ڷ�ƾ ����


        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<CommonMonsterController>();
                if (controller == null)
                {
                    controller = GetComponent<CommonMonsterController>();
                }
            }

            if (controller == null)
            {
                Debug.LogError("[LizardmanAnimatorEvents] CommonMonsterController�� ã�� �� �����ϴ�!");
            }

            // �ʱ� ��Ȱ��ȭ (���� ��ġ)
            if (Hitbox_Attack1 != null) Hitbox_Attack1.gameObject.SetActive(false);
            if (Hitbox_Attack2 != null) Hitbox_Attack2.gameObject.SetActive(false);
        }

        // --- ��Ʈ�ڽ� Ȱ��ȭ/��Ȱ��ȭ ���� (AnimatorEvents ���ο��� ����) ---

        // Animator Event: Attack1 �ִϸ��̼��� Ư�� �����ӿ��� ȣ��
        public void EnableHitbox_Attack1()
        {
            Hitbox_Attack1.gameObject.SetActive(true);
            // ���� �ڷ�ƾ�� �ִٸ� �ߴ��ϰ� ���� �����Ͽ� �ߺ� ����
            if (deactivateHitboxCoroutine1 != null) StopCoroutine(deactivateHitboxCoroutine1);
            deactivateHitboxCoroutine1 = StartCoroutine(DeactivateHitboxAfterDelay(Hitbox_Attack1.gameObject, 0.3f)); // 0.3�ʴ� ����, �ִϸ��̼ǿ� �°� ����
        }

        // Animator Event: Attack2 �ִϸ��̼��� Ư�� �����ӿ��� ȣ��
        public void EnableHitbox_Attack2()
        {
            Hitbox_Attack2.gameObject.SetActive(true);
            if (deactivateHitboxCoroutine2 != null) StopCoroutine(deactivateHitboxCoroutine2);
            deactivateHitboxCoroutine2 = StartCoroutine(DeactivateHitboxAfterDelay(Hitbox_Attack2.gameObject, 0.3f)); // 0.3�ʴ� ����
        }

        // ��Ʈ�ڽ��� ���� �ð� �� ��Ȱ��ȭ�ϴ� �ڷ�ƾ (����)
        private IEnumerator DeactivateHitboxAfterDelay(GameObject hitbox, float delay)
        {
            yield return new WaitForSeconds(delay); 
            hitbox.SetActive(false);
        }

        // --- ���Ÿ� ���� (RangedAttackState) ���� �̺�Ʈ ---

        // Animator Event: Fireball �ִϸ��̼��� Ư�� �����ӿ��� ȣ��
        public void Event_LaunchFireball()
        {
            if (controller != null && controller.currentState is LizardmanRangedAttackState rangedState)
            {
                // CommonMonsterController�� ����ü ���� �ʵ带 ����
                rangedState.LaunchFireball(
                    controller.forgProjectilePrefab,
                    controller.forgShootPoint
                );
            }
        }

        // --- ���� �ִϸ��̼� ���� �̺�Ʈ (OnAttackAnimationEnd) ---

        // Animator Event: ��� ���� �ִϸ��̼��� ������ ȣ�� (���¸� Idle�� ����)
        public void Event_OnAttackAnimationEnd()
        {
            controller.ChangeState(new LizardmanIdleState(controller));
        }
    }
}