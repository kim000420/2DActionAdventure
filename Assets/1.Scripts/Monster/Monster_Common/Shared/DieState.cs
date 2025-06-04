using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가
using CommonMonster.Controller; // CommonMonsterController 참조
using CommonMonster.States;     // BaseMonsterState 참조

namespace CommonMonster.States.Common
{
    public class DieState : BaseMonsterState
    {
        private Coroutine destroyCoroutine; // 오브젝트 파괴 코루틴 참조

        public DieState(CommonMonsterController controller) : base(controller) { }

        public override void Enter()
        {
            Debug.Log($"[{controller.monsterName} DieState] Enter");

            controller.isDead = true; // 몬스터 사망 플래그 설정
            controller.rb.velocity = Vector2.zero; // 모든 움직임 멈춤
            controller.rb.bodyType = RigidbodyType2D.Kinematic; // 물리적 영향 무시 (더 이상 밀리지 않도록)
            controller.animator.Play($"{controller.monsterName}_Die"); // 사망 애니메이션 재생

            // (선택 사항) 콜라이더 비활성화: 플레이어/다른 오브젝트와의 충돌 방지
            Collider2D monsterCollider = controller.GetComponent<Collider2D>();
            if (monsterCollider != null)
            {
                monsterCollider.enabled = false;
            }

            // 사망 후 일정 시간 뒤 오브젝트 파괴 코루틴 시작
            // 애니메이션 길이에 맞춰 시간을 조절하는 것이 좋습니다.
            float destroyDelay = 2.0f; // 예시: 사망 애니메이션 재생 후 2초 뒤 파괴
            // TODO: 실제 애니메이션 길이에 맞춰 조정하거나, 애니메이션 이벤트로 호출되도록 변경 가능
            destroyCoroutine = controller.StartCoroutine(DestroyAfterDelay(destroyDelay));
        }

        public override void Execute()
        {
            // 사망 상태에서는 특별한 지속 로직이 필요 없음
        }

        public override void Exit()
        {
            Debug.Log($"[{controller.monsterName} DieState] Exit (Should not be called if object is destroyed)");
            // 이 상태는 보통 오브젝트 파괴로 끝나기 때문에 Exit이 호출되지 않을 수 있음
            // 하지만 혹시 모를 경우를 대비하여 정리 로직 포함
            if (destroyCoroutine != null)
            {
                controller.StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Debug.Log($"[{controller.monsterName}] GameObject will be destroyed.");
            // 몬스터 게임 오브젝트 파괴
            if (controller != null && controller.gameObject != null)
            {
                controller.gameObject.SetActive(false);
            }
        }
    }
}