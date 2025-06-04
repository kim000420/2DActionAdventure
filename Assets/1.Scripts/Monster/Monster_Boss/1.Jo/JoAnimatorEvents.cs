using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;
using TutorialBoss.States.Jo;

namespace TutorialBoss.AnimEvents
{
    public class JoAnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;

        [Header("��Ʈ�ڽ�")]
        public GameObject Hitbox_Attack1;
        public GameObject Hitbox_Attack2;

        // �ִϸ��̼ǿ��� ȣ��: ���� Ÿ�̹� ����
        public void EnableHitbox_Attack1() 
        {
            Hitbox_Attack1.SetActive(true);
            StartCoroutine(DeactivateMeleeHitboxAfterDelay(0.3f));
        }

        public void EnableHitbox_Attack2()
        {
            Hitbox_Attack2.SetActive(true);
            StartCoroutine(DeactivateMeleeHitboxAfterDelay(0.3f));
        }

        private IEnumerator DeactivateMeleeHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hitbox_Attack1.gameObject.SetActive(false);
            Hitbox_Attack2.gameObject.SetActive(false);
        }

        // �ִϸ��̼� �̺�Ʈ�� ���� ���� Ʈ����
        public void OnAttackEnd()
        {
            // 2f ���� ��ٸ� �� ChaseState�� ��ȯ
            StartCoroutine(WaitForAttackCooldownAndTransition());
        }

        private IEnumerator WaitForAttackCooldownAndTransition()
        {
            yield return new WaitForSeconds(1.5f);
            controller.ChangeState(new JoChaseState(controller));
        }
    }
}
