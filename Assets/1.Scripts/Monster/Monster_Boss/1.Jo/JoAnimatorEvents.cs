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
        public GameObject hitbox_Attack1;
        public GameObject hitbox_Attack2;

        // �ִϸ��̼ǿ��� ȣ��: ���� Ÿ�̹� ����
        public void EnableHitbox_Attack1() 
        {
            hitbox_Attack1.SetActive(true);
            Invoke(nameof(DisableHitbox_Attack1), 0.3f);
        } 
        public void DisableHitbox_Attack1() => hitbox_Attack1.SetActive(false);

        public void EnableHitbox_Attack2()
        {
            hitbox_Attack1.SetActive(true);
            Invoke(nameof(DisableHitbox_Attack2), 0.3f);
        }
        public void DisableHitbox_Attack2() => hitbox_Attack2.SetActive(false);

        private IEnumerator AttackCooldownRoutine()
        {
            controller.isAttackCooldown = true;
            controller.ChangeState(new JoIdleState(controller)); // ��� Idle�� ��ȯ
            yield return new WaitForSeconds(2f);
            controller.isAttackCooldown = false;
        }
        // �ִϸ��̼� �̺�Ʈ�� ���� ���� Ʈ����
        public void OnAttackEnd()
        {
            controller.StartCoroutine(AttackCooldownRoutine());
        }
    }
}
