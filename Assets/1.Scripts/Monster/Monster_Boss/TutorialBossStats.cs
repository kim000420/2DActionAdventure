using UnityEngine;

namespace TutorialBoss
{
    public class TutorialBossStats : MonoBehaviour
    {
        [Header("보스 식별")]
        public string bossName = "Jo";

        [Header("체력")]
        public int maxHP = 100;
        public int currentHP = 100;

        [Header("그로기 수치")]
        public float currentGroggy = 0f;
        public float maxGroggy = 999f; // 튜토리얼 목적상 절대 진입 안 하도록 설정
        
        [Header("전투 범위")]
        public float detectRange = 5f;
        public float attackRange = 1.5f;

        [Header("이동 관련")]
        public float moveSpeed = 2.5f;

        [Header("넉백 저항")]
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

            // 넉백 전용 HitState 진입
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
