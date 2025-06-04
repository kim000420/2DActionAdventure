using UnityEngine;
using TutorialBoss.Controller;
using TutorialBoss.States.Dok2;
using System.Collections; // Coroutine ����� ���� �߰�

namespace TutorialBoss.AnimEvents
{
    public class Dok2AnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;
        public HitboxTrigger Hitbox_Attack1; // ���� ���� ��Ʈ�ڽ� (Unity �����Ϳ��� ����)
        public HitboxTrigger Hitbox_Attack2;

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<TutorialBossStateController>();
            }
        }

        // �ִϸ��̼� �̺�Ʈ: ���� ���� ��Ʈ�ڽ� Ȱ��ȭ (��: ���� ������ ���� ��)
        public void EnableHitbox_Attack1()
        {
            Hitbox_Attack1.gameObject.SetActive(true);
            StartCoroutine(DeactivateMeleeHitboxAfterDelay(0.3f));
        }
        public void EnableHitbox_Attack2()
        {
            Hitbox_Attack2.gameObject.SetActive(true);
            StartCoroutine(DeactivateMeleeHitboxAfterDelay(0.3f));
        }

        // �ִϸ��̼� �̺�Ʈ: ���� ���� ��Ʈ�ڽ� ��Ȱ��ȭ (���� ��)
        private IEnumerator DeactivateMeleeHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hitbox_Attack1.gameObject.SetActive(false);
            Hitbox_Attack2.gameObject.SetActive(false);
        }

        // �ִϸ��̼� �̺�Ʈ: ���� �ִϸ��̼��� ������ ��
        public void OnAttackEnd()
        {
            // 2f ���� ��ٸ� �� ChaseState�� ��ȯ
            StartCoroutine(WaitForAttackCooldownAndTransition());
        }

        private IEnumerator WaitForAttackCooldownAndTransition()
        {
            yield return new WaitForSeconds(1.5f);
            controller.ChangeState(new Dok2ChaseState(controller));
        }

        public void OnDok2GroggyAnimationEnd()
        {
            // �׷α� ���°� �������Ƿ� Dok2ChaseState�� ��ȯ
            controller.bossStats.currentGroggy = controller.bossStats.maxGroggy;
            controller.ChangeState(new Dok2ChaseState(controller));

        }
    }
}