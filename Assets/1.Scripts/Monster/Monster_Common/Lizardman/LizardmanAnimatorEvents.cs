using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using CommonMonster.Controller; // CommonMonsterController 참조
using CommonMonster.States.Lizardman; // LizardmanRangedAttackState 참조

// HitboxTrigger 스크립트가 어느 네임스페이스에 있는지 확인하고 필요 시 추가
// 현재 HitboxTrigger는 네임스페이스 없이 전역에 있으므로 using UnityEngine; 만으로도 접근 가능

namespace CommonMonster.AnimEvents.Lizardman
{
    public class LizardmanAnimatorEvents : MonoBehaviour
    {
        public CommonMonsterController controller;

        [Header("Lizardman Hitboxes")] // 인스펙터에서 직접 할당할 히트박스들
        public HitboxTrigger Hitbox_Attack1; // Melee Attack1 애니메이션용 히트박스 스크립트
        public HitboxTrigger Hitbox_Attack2; // Melee Attack2 애니메이션용 히트박스 스크립트

        private Coroutine deactivateHitboxCoroutine1; // Attack1 히트박스 비활성화 코루틴 참조
        private Coroutine deactivateHitboxCoroutine2; // Attack2 히트박스 비활성화 코루틴 참조


        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<CommonMonsterController>();
                if (controller == null)
                {
                    controller = GetComponent<CommonMonsterController>();
                }
            }

            if (controller == null)
            {
                Debug.LogError("[LizardmanAnimatorEvents] CommonMonsterController를 찾을 수 없습니다!");
            }

            // 초기 비활성화 (안전 장치)
            if (Hitbox_Attack1 != null) Hitbox_Attack1.gameObject.SetActive(false);
            if (Hitbox_Attack2 != null) Hitbox_Attack2.gameObject.SetActive(false);
        }

        // --- 히트박스 활성화/비활성화 로직 (AnimatorEvents 내부에서 관리) ---

        // Animator Event: Attack1 애니메이션의 특정 프레임에서 호출
        public void EnableHitbox_Attack1()
        {
            Hitbox_Attack1.gameObject.SetActive(true);
            // 기존 코루틴이 있다면 중단하고 새로 시작하여 중복 방지
            if (deactivateHitboxCoroutine1 != null) StopCoroutine(deactivateHitboxCoroutine1);
            deactivateHitboxCoroutine1 = StartCoroutine(DeactivateHitboxAfterDelay(Hitbox_Attack1.gameObject, 0.3f)); // 0.3초는 예시, 애니메이션에 맞게 조절
        }

        // Animator Event: Attack2 애니메이션의 특정 프레임에서 호출
        public void EnableHitbox_Attack2()
        {
            Hitbox_Attack2.gameObject.SetActive(true);
            if (deactivateHitboxCoroutine2 != null) StopCoroutine(deactivateHitboxCoroutine2);
            deactivateHitboxCoroutine2 = StartCoroutine(DeactivateHitboxAfterDelay(Hitbox_Attack2.gameObject, 0.3f)); // 0.3초는 예시
        }

        // 히트박스를 일정 시간 후 비활성화하는 코루틴 (재사용)
        private IEnumerator DeactivateHitboxAfterDelay(GameObject hitbox, float delay)
        {
            yield return new WaitForSeconds(delay); 
            hitbox.SetActive(false);
        }

        // --- 원거리 공격 (RangedAttackState) 관련 이벤트 ---

        // Animator Event: Fireball 애니메이션의 특정 프레임에서 호출
        public void Event_LaunchFireball()
        {
            if (controller != null && controller.currentState is LizardmanRangedAttackState rangedState)
            {
                // CommonMonsterController의 투사체 관련 필드를 전달
                rangedState.LaunchFireball(
                    controller.forgProjectilePrefab,
                    controller.forgShootPoint
                );
            }
        }

        // --- 공통 애니메이션 종료 이벤트 (OnAttackAnimationEnd) ---

        // Animator Event: 모든 공격 애니메이션의 끝에서 호출 (상태를 Idle로 전이)
        public void Event_OnAttackAnimationEnd()
        {
            controller.ChangeState(new LizardmanIdleState(controller));
        }
    }
}