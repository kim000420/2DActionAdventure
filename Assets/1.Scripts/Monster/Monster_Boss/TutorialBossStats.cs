using UnityEngine;

namespace TutorialBoss
{
    public class TutorialBossStats : MonoBehaviour
    {
        [Header("���� �ĺ�")]
        public string bossName = "Jo";

        [Header("ü��")]
        public int maxHP = 100;
        public int currentHP = 100;

        [Header("�׷α� ��ġ")]
        public float currentGroggy = 0f;
        public float maxGroggy = 999f; // Ʃ�丮�� ������ ���� ���� �� �ϵ��� ����
        
        [Header("���� ����")]
        public float detectRange = 5f;
        public float attackRange = 1.5f;

        [Header("�̵� ����")]
        public float moveSpeed = 2.5f;

        [Header("�˹� ����")]
        public float knockbackResistance = 1f;
        private void Awake()
        {
            currentHP = maxHP;
        }
        public void ApplyHit(int damage, int groggy, float knockback, Vector2 attackerPosition)
        {
            currentHP -= damage;

            if (currentHP <= 0)
            {
                var controller = GetComponent<TutorialBoss.Controller.TutorialBossStateController>();
                controller.ChangeState(new TutorialBoss.States.DieState(controller));
                return;
            }

            currentGroggy += groggy;

            if (currentGroggy >= maxGroggy)
            {
                var controller = GetComponent<TutorialBoss.Controller.TutorialBossStateController>();
                controller.ChangeState(new TutorialBoss.States.GroggyState(controller));
                currentGroggy = 0;
                return;
            }

            // �˹� ���� HitState ����
            GetComponent<TutorialBoss.Controller.TutorialBossStateController>()
                .ChangeState(new TutorialBoss.States.HitState(
                    GetComponent<TutorialBoss.Controller.TutorialBossStateController>(),
                    attackerPosition,
                    knockback * knockbackResistance
                ));
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
