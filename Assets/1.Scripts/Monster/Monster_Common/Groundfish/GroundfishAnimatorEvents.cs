using UnityEngine;
using System.Collections;
using CommonMonster.Controller; // CommonMonsterController 참조

// HitboxTrigger 스크립트가 어느 네임스페이스에 있는지 확인하고 필요 시 추가
// 만약 네임스페이스가 없다면 using UnityEngine; 만으로 충분합니다.
// using YourGameName.Combat; // 예시: HitboxTrigger가 Combat 네임스페이스에 있을 경우

namespace CommonMonster.AnimEvents
{
    public class GroundfishAnimatorEvents : MonoBehaviour
    {
        public CommonMonsterController controller;
        public HitboxTrigger Hitbox_Attack; // 공격 히트박스 스크립트 참조

        private Coroutine deactivateHitboxCoroutine; // 히트박스 비활성화 코루틴 참조

        // Start는 일반적으로 사용되지 않지만, 필요에 따라 추가
        private void Awake() // Start 대신 Awake에서 초기화하는 것이 더 안전함
        {
            if (controller == null)
            {
                controller = GetComponentInParent<CommonMonsterController>();
            }
        }

        // 애니메이션 이벤트에서 호출될 메서드: 히트박스 활성화
        public void EnableHitbox_Attack1()
        {
            Hitbox_Attack.gameObject.SetActive(true);
            StartCoroutine(DeactivateHitboxAfterDelay(0.3f));
        }

        // 히트박스를 일정 시간 후 비활성화하는 코루틴
        private IEnumerator DeactivateHitboxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hitbox_Attack.gameObject.SetActive(false);
        }
    }
}