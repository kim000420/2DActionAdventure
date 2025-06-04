using CommonMonster.Controller;
using UnityEngine;
using CommonMonster.States.Common;

namespace CommonMonster.Stats
{
    // ���� Ÿ�� ������ ����
    public enum MonsterType
    {
        MeleeOnly,    // �ٰŸ� ���ݸ� ������ ����
        RangedOnly,   // ���Ÿ� ���ݸ� ������ ���� (����ü �߻�)
        Hybrid,       // �ٰŸ�/���Ÿ� ��� ������ ����
    }

    public class CommonMonsterStats : MonoBehaviour
    {
        [Header("���� �⺻ ����")]
        [Tooltip("�� ������ ���� �� �ൿ Ư���� �����մϴ�.")]
        public MonsterType monsterType = MonsterType.MeleeOnly; // �����Ϳ��� ���� Ÿ�� ����

        [Header("���� �� ���� ����")]
        [Tooltip("������ �ִ� ü��.")]
        public int maxHp = 100;
        public int currentHp; // ���� ü��

        [Tooltip("������ �̵� �ӵ�.")]
        public float moveSpeed = 3f;

        [Tooltip("���Ͱ� �˹�Ǵ� ������ �����մϴ�. 1f�� �⺻ �˹�, 0.5f�� ���� �˹�.")]
        public float knockbackResistance = 1f;

        [Header("�׷α� ����")]
        [Tooltip("���Ͱ� �׷α� ���¿� ������ ���� �ʿ��� �ִ� �׷α� ������.")]
        public int maxGroggy = 10;
        public int currentGroggy; // ���� �׷α� ������ (�ǰ� �� ����)

        [Tooltip("�׷α� ���°� ���ӵǴ� �ð�.")]
        public float groggyDuration = 3f;

        [Header("�ൿ ���� ���� (Gizmos ����)")]
        [Tooltip("�÷��̾ �ν��ϰ� ������ �����ϴ� �ִ� �Ÿ�.")]
        public float detectionRange = 7f;

        [Tooltip("�ٰŸ� ������ ������ �ִ� �Ÿ�. MeleeOnly �� Hybrid Ÿ�Կ��� ��ȿ�մϴ�.")]
        public float meleeAttackRange = 1.5f;

        [Tooltip("���Ÿ� ����(����ü �߻�)�� ������ �ִ� �Ÿ�. RangedOnly �� Hybrid Ÿ�Կ��� ��ȿ�մϴ�.")]
        public float rangedAttackRange = 6f;

        // [Header("���� ���� (JumperOnly, Hybrid Ÿ�� ����)")] // �ʿ� �� ��� �߰�
        [Tooltip("���� ���� �Ǵ� ȸ�� ���� �� ����� ��.")]
        public float jumpForce = 8f;
        [Tooltip("���� �� ���� ������ �������� �������� ��Ÿ��.")]
        public float jumpCooldown = 1f;


        private CommonMonsterController controller;
        private void Awake()
        {
            currentHp = maxHp; // ���� ���� �� ���� ü���� �ִ� ü������ �ʱ�ȭ
            currentGroggy = 0; // �׷α� ������ �ʱ�ȭ
        }

        public void ApplyHit(int damage, int groggy, float knockbackForce, Vector2 attackerPosition)
        {
            var controller = GetComponent<CommonMonster.Controller.CommonMonsterController>();
            if (controller.isDead) return; // �̹� �׾��ٸ� �� �̻� ���ظ� ���� ����

            currentHp -= damage;

            // ü�� üũ: ��� ���� ����
            if (currentHp <= 0)
            {
                currentHp = 0; // ü���� 0 ���Ϸ� �������� �ʵ���
                controller.ChangeState(new DieState(controller));
                return; // ��������� �׷α�/�ǰ� ������ �������� ����
            }

            // �׷α� ������ ����
            currentGroggy -= groggy;

            // �׷α� ������ üũ: �׷α� ���� ���� (��� ���°� �ƴ϶��)
            if (currentGroggy <= 0 && !controller.isGroggy)
            {
                // controller.ChangeState(new GroggyState(controller));
                currentGroggy = maxGroggy; // �׷α� ���� �� ������ �ʱ�ȭ
                return; // �׷α� ���·� ���̵Ǿ����� �ǰ� ���·� �������� ����
            }

            // �ǰ� ���� ���� (����� �ƴϰ� �׷α⵵ �ƴ϶��)
            if (!controller.isHitRecovery) // �̹� �ǰ� ���� ���� �ƴ϶��
            {
                // �˹� ������ ������ ���� �˹� �� ���
                float finalKnockbackForce = knockbackForce * knockbackResistance;

                controller.ChangeState(new HitState(
                    controller,
                    attackerPosition,
                    finalKnockbackForce
                ));
            }
        }
        // ����Ƽ �����Ϳ��� ���� �� Gizmos �ð�ȭ
        private void OnDrawGizmosSelected()
        {
            // ���� ��ġ �������� Gizmos �׸���
            Vector3 center = transform.position;

            // �ν� ���� (��� ���� Ÿ��)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, detectionRange);

            // �ٰŸ� ���� ���� (MeleeOnly, Hybrid Ÿ�Ը�)
            if (monsterType == MonsterType.MeleeOnly || monsterType == MonsterType.Hybrid)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(center, meleeAttackRange);
            }

            // ���Ÿ� ���� ���� (RangedOnly, Hybrid Ÿ�Ը�)
            if (monsterType == MonsterType.RangedOnly || monsterType == MonsterType.Hybrid)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(center, rangedAttackRange);
            }
        }
    }
}