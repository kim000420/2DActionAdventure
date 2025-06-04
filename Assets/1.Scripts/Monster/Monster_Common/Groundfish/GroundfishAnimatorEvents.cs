using UnityEngine;
using System.Collections;
using CommonMonster.Controller; // CommonMonsterController ����

// HitboxTrigger ��ũ��Ʈ�� ��� ���ӽ����̽��� �ִ��� Ȯ���ϰ� �ʿ� �� �߰�
// ���� ���ӽ����̽��� ���ٸ� using UnityEngine; ������ ����մϴ�.
// using YourGameName.Combat; // ����: HitboxTrigger�� Combat ���ӽ����̽��� ���� ���

namespace CommonMonster.AnimEvents
{
    public class GroundfishAnimatorEvents : MonoBehaviour
    {
        public CommonMonsterController controller;
        public HitboxTrigger Hitbox_Attack; // ���� ��Ʈ�ڽ� ��ũ��Ʈ ����

        private Coroutine deactivateHitboxCoroutine; // ��Ʈ�ڽ� ��Ȱ��ȭ �ڷ�ƾ ����

        // Start�� �Ϲ������� ������ ������, �ʿ信 ���� �߰�
        private void Awake() // Start ��� Awake���� �ʱ�ȭ�ϴ� ���� �� ������
        {
            if (controller == null)
            {
                controller = GetComponentInParent<CommonMonsterController>();
            }
        }

        // �ִϸ��̼� �̺�Ʈ���� ȣ��� �޼���: ��Ʈ�ڽ� Ȱ��ȭ
        public void EnableHitbox_Attack1()
        {
            Hitbox_Attack.gameObject.SetActive(true);
            StartCoroutine(DeactivateHitboxAfterDelay(0.3f));
        }

        // ��Ʈ�ڽ��� ���� �ð� �� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
        private IEnumerator DeactivateHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hitbox_Attack.gameObject.SetActive(false);
        }
    }
}