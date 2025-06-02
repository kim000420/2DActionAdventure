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
        public GameObject hitbox_Attack1;
        public GameObject hitbox_Attack2;

        // 애니메이션에서 호출: 공격 타이밍 순간
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
            controller.ChangeState(new JoIdleState(controller)); // 즉시 Idle로 전환
            yield return new WaitForSeconds(2f);
            controller.isAttackCooldown = false;
        }
        // 애니메이션 이벤트로 공격 종료 트리거
        public void OnAttackEnd()
        {
            controller.StartCoroutine(AttackCooldownRoutine());
        }
    }
}
