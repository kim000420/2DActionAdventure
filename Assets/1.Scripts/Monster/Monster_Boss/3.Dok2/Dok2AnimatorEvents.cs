using UnityEngine;
using TutorialBoss.Controller;
using TutorialBoss.States.Dok2;
using System.Collections; // Coroutine 사용을 위해 추가

namespace TutorialBoss.AnimEvents
{
    public class Dok2AnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;
        public HitboxTrigger Hitbox_Attack1; // 근접 공격 히트박스 (Unity 에디터에서 연결)
        public HitboxTrigger Hitbox_Attack2;

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<TutorialBossStateController>();
            }
        }

        // 애니메이션 이벤트: 근접 공격 히트박스 활성화 (예: 공격 프레임 시작 시)
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

        // 애니메이션 이벤트: 근접 공격 히트박스 비활성화 (지연 후)
        private IEnumerator DeactivateMeleeHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hitbox_Attack1.gameObject.SetActive(false);
            Hitbox_Attack2.gameObject.SetActive(false);
        }

        // 애니메이션 이벤트: 공격 애니메이션이 끝났을 때
        public void OnAttackEnd()
        {
            // 2f 동안 기다린 후 ChaseState로 변환
            StartCoroutine(WaitForAttackCooldownAndTransition());
        }

        private IEnumerator WaitForAttackCooldownAndTransition()
        {
            yield return new WaitForSeconds(1.5f);
            controller.ChangeState(new Dok2ChaseState(controller));
        }

        public void OnDok2GroggyAnimationEnd()
        {
            // 그로기 상태가 끝났으므로 Dok2ChaseState로 전환
            controller.bossStats.currentGroggy = controller.bossStats.maxGroggy;
            controller.ChangeState(new Dok2ChaseState(controller));

        }
    }
}