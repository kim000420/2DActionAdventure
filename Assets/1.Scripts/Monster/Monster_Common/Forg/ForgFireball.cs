using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가

namespace CommonMonster.Projectiles
{
    public class ForgFireball : MonoBehaviour
    {
        [Header("투사체 설정")]
        [Tooltip("투사체의 수명 (초). 이 시간 후 자동으로 소멸합니다.")]
        public float lifetime = 3f; // 투사체 수명 (3초 후 자동 소멸)

        [Tooltip("투사체의 회전 보간 속도. 높을수록 즉각적으로 회전합니다.")]
        public float rotationSpeed = 10f;

        // ⭐ 추가: 투사체 기본 이미지 방향 설정 ⭐
        [Tooltip("투사체 스프라이트의 기본 방향 (유니티 에디터에서 0도 회전 시 바라보는 방향).")]
        // X축 양의 방향(오른쪽)을 0도로 기준 잡았을 때, 스프라이트가 어느 방향을 보고 있는지 설정합니다.
        // 예를 들어, 스프라이트가 위를 보고 있다면 90, 왼쪽은 180, 아래는 270 또는 -90
        public float defaultSpriteDirectionAngle = 0f; // 기본값은 오른쪽 (0도)

        [Header("충돌 효과")]
        [Tooltip("투사체가 소멸할 때 재생될 파티클 효과 프리팹 (예: 폭발 효과).")]
        public GameObject hitEffectPrefab;

        private bool isDestroying = false; // 투사체가 소멸 중인지 여부 (중복 소멸 방지)

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogWarning("[ForgFireball] Rigidbody2D 컴포넌트가 없습니다. 투사체가 움직이지 않을 수 있습니다.");
                enabled = false; // Rigidbody2D 없으면 이 스크립트 비활성화
            }
        }

        private void OnEnable()
        {
            isDestroying = false; // 오브젝트 풀링을 사용하는 경우 재활성화 시 초기화
            StartCoroutine(DestroyAfterLifetime()); // 수명 코루틴 시작
        }

        void Update()
        {
            // 투사체의 속도 벡터를 가져옵니다.
            Vector2 velocity = rb.velocity;

            // 속도가 0이 아닐 때만 회전합니다. (정지해 있을 때는 회전하지 않음)
            // 아주 작은 속도에도 회전하지 않도록 임계값 설정
            if (velocity.magnitude > 0.01f)
            {
                // 속도 벡터의 방향을 각도로 변환합니다 (라디안 -> Degrees).
                // Atan2는 X축 양의 방향을 0도로 기준으로 합니다.
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

                // ⭐ 추가: 스프라이트의 기본 방향을 고려한 오프셋 적용 ⭐
                // Atan2 결과는 X+ 방향을 0도로 보므로, 스프라이트가 바라보는 기본 방향을 빼줍니다.
                // 예를 들어, 스프라이트가 위(90도)를 바라보고 있다면, Atan2 결과에서 90을 빼줘야 합니다.
                float finalAngle = angle - defaultSpriteDirectionAngle;

                // 현재 회전에서 목표 회전으로 부드럽게 보간 (선택 사항)
                Quaternion targetRotation = Quaternion.Euler(0, 0, finalAngle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 또는 즉시 회전 적용 (더 즉각적인 반응)
                // transform.rotation = Quaternion.Euler(0, 0, finalAngle);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 이미 소멸 중이면 중복 처리 방지
            if (isDestroying) return;

            // 플레이어, 땅, 벽, 장애물과 충돌 시
            if (other.CompareTag("Player") || other.CompareTag("Ground") || other.CompareTag("Wall"))
            {
                // 충돌 효과 재생 후 투사체 소멸
                // (데미지, 그로기, 넉백은 HitboxTrigger가 담당)
                SpawnHitEffectAndDestroy();
            }
        }

        // 투사체 수명 관리 코루틴
        private IEnumerator DestroyAfterLifetime()
        {
            yield return new WaitForSeconds(lifetime);

            // 아직 소멸하지 않았다면 (플레이어/환경과 충돌하지 않고 시간 초과 시)
            if (gameObject != null && !isDestroying)
            {
                Debug.Log("[ForgFireball] 수명 만료. 소멸합니다.");
                SpawnHitEffectAndDestroy();
            }
        }

        // 충돌/수명 만료 시 호출될 소멸 처리 메서드
        private void SpawnHitEffectAndDestroy()
        {
            if (isDestroying) return; // 이미 소멸 중이면 중복 실행 방지
            isDestroying = true;

            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(effect, ps.main.duration);
                }
                else
                {
                    Destroy(effect, 2f); // 파티클 시스템이 없으면 2초 후 파괴
                }
            }
            Destroy(gameObject); // 투사체 자체 파괴
        }
    }
}