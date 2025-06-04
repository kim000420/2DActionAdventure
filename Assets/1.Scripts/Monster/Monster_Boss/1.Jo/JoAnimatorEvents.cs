using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;
using TutorialBoss.States.Jo;

namespace TutorialBoss.AnimEvents
{
    public class JoAnimatorEvents : MonoBehaviour
    {
        public TutorialBossStateController controller;

        [Header("히트박스")]
        public GameObject Hitbox_Attack1;
        public GameObject Hitbox_Attack2;

        // 애니메이션에서 호출: 공격 타이밍 순간
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

        // 애니메이션 이벤트로 공격 종료 트리거
        public void OnAttackEnd()
        {
            // 2f 동안 기다린 후 ChaseState로 변환
            StartCoroutine(WaitForAttackCooldownAndTransition());
        }

        private IEnumerator WaitForAttackCooldownAndTransition()
        {
            yield return new WaitForSeconds(1.5f);
            controller.ChangeState(new JoChaseState(controller));
        }
    }
}
