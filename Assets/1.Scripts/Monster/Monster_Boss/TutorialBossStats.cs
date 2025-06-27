using TutorialBoss.States;
using UnityEngine;

namespace TutorialBoss
{
    public class TutorialBossStats : MonoBehaviour
    {
        public BossUIManager bossUI;

        [Header("ü��")]
        public int maxHP = 100;
        public int currentHP = 100;

        [Header("�׷α�")]
        public float maxGroggy = 999f;
        public float currentGroggy = 0f;

        [Header("���� ����")]
        public float detectRange = 5f;
        public float attackRange = 1.5f;
        public float escapeTriggerRange = 2.0f;

        [Header("�̵��ӵ�")]
        public float moveSpeed = 2.5f;

        private void Awake()
        {
            currentHP = maxHP;
            currentGroggy = maxGroggy;
        }
        private void OnEnable()
        {
            var controller = GetComponent<TutorialBoss.Controller.TutorialBossStateController>();
            if (controller != null)
            {
                string bossId = controller.bossName; // Jo, Bow, Dok2 ��
                BossManager.Instance?.RegisterBoss(bossId, gameObject);
            }
            if (bossUI == null)
            {
                var ui = GameObject.FindObjectOfType<BossUIManager>();
                if (ui != null)
                {
                    bossUI = ui;
                }
            }

            bossUI?.ShowUI();
            UpdateUI();
        }

        public void ApplyHit(int damage, int groggy, float knockback, Vector2 attackerPosition)
        {

            var controller = GetComponent<TutorialBoss.Controller.TutorialBossStateController>();
            //�̹� �׾��ٸ� ���� ����
            if (controller.isDead) return;

            currentHP -= damage;
            UpdateUI();

            if (currentHP <= 0)
            {
                bossUI?.HideUI();
                controller.ChangeState(new TutorialBoss.States.DieState(controller));
                return;
            }

            currentGroggy -= groggy;
            UpdateUI();

            if (currentGroggy <= 0)
            {
                controller.ChangeState(new TutorialBoss.States.GroggyState(controller));
                return;
            }

            //Dok2�� �˹��� ���� ����
            if (controller.bossName == "Dok2") return;

            //�˹� �۵��κ�
            GetComponent<TutorialBoss.Controller.TutorialBossStateController>()
                .ChangeState(new TutorialBoss.States.HitState(
                    GetComponent<TutorialBoss.Controller.TutorialBossStateController>(),
                    attackerPosition,
                    knockback
                ));
        }

        private void UpdateUI()
        {
            bossUI?.UpdateHP((float)currentHP / maxHP);
            bossUI?.UpdateGroggy((float)currentGroggy / maxGroggy);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, escapeTriggerRange);
        }
    }
}
