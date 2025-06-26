using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;
using TutorialBoss.States.Webuin;

namespace TutorialBoss.AnimEvents
{
    public class WebuinAnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;

        [Header("��Ʈ�ڽ�")]
        public GameObject Hitbox_Attack;

        // �ִϸ��̼ǿ��� ȣ��: ���� Ÿ�̹� ����
        public void EnableHitbox_Attack()
        {
            Hitbox_Attack.SetActive(true);
            StartCoroutine(DeactivateMeleeHitboxAfterDelay(0.3f));
        }

        private IEnumerator DeactivateMeleeHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hitbox_Attack.gameObject.SetActive(false);
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
            controller.ChangeState(new WebuinChaseState(controller));
        }
    }
}
