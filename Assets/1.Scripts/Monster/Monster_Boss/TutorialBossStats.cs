using UnityEngine;

namespace TutorialBoss
{
    public class TutorialBossStats : MonoBehaviour
    {
        [Header("체력")]
        public int maxHP = 100;
        public int currentHP = 100;

        [Header("그로기")]
        public float maxGroggy = 999f;
        public float currentGroggy = 0f;

        [Header("전투 범위")]
        public float detectRange = 5f;
        public float attackRange = 1.5f;
        public float escapeTriggerRange = 2.0f;

        [Header("이동속도")]
        public float moveSpeed = 2.5f;

        private void Awake()
        {
            currentHP = maxHP;
            currentGroggy = maxGroggy;
        }

        public void ApplyHit(int damage, int groggy, float knockback, Vector2 attackerPosition)
        {

            var controller = GetComponent<TutorialBoss.Controller.TutorialBossStateController>();
            currentHP -= damage;

            if (currentHP <= 0)
            {
                controller.ChangeState(new TutorialBoss.States.DieState(controller));
                return;
            }

            currentGroggy -= groggy;

            if (currentGroggy <= 0)
            {
                controller.ChangeState(new TutorialBoss.States.GroggyState(controller));
                return;
            }

            //Dok2는 넉백을 받지 않음
            if (controller.bossName == "Dok2") return;

            //넉백 작동부분
            GetComponent<TutorialBoss.Controller.TutorialBossStateController>()
                .ChangeState(new TutorialBoss.States.HitState(
                    GetComponent<TutorialBoss.Controller.TutorialBossStateController>(),
                    attackerPosition,
                    knockback
                ));
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
