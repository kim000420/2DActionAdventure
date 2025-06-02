using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("화살 속성")]
    public float initialSpeed = 10f; // 화살의 초기 발사 속도
    public float decelerationRate = 0.98f; // 매 프레임 수평 속도를 줄이는 비율 (0.98이면 2% 감소)

    [Header("삭제 조건")]
    public float destroyAfterSeconds = 4f; // 일정 시간 후 자체 삭제 (초)

    private Rigidbody2D rb;
    private bool hasFired = false; // 화살이 발사되었는지 확인하는 플래그

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("[Arrow] Rigidbody2D 컴포넌트가 없습니다! 화살이 작동하지 않을 수 있습니다.");
        }
        // 기본적으로 중력은 0으로 설정 (발사 시점에만 중력 적용)
        // 만약 항상 중력의 영향을 받는다면, 유니티 에디터에서 Rigidbody2D의 Gravity Scale을 조절하세요.
        // 여기서는 발사 후 감속과 함께 중력을 서서히 적용하는 방식을 가정합니다.
        rb.gravityScale = 0f;
    }

    private void OnEnable()
    {
        // 오브젝트 풀링을 사용하는 경우, 재활성화 시 초기화
        hasFired = false;
        // 자체 삭제 타이머 시작
        Invoke("DestroyArrow", destroyAfterSeconds);
    }

    // 화살 발사 메서드 (TutorialBossStateController에서 호출)
    public void Fire(Vector2 direction, float currentBossScaleX)
    {
        if (rb == null) return;

        // 화살의 초기 속도 설정
        // ⭐ 중요: 보스가 바라보는 방향에 따라 화살 스프라이트를 뒤집음
        // Bow는 좌측이 디폴트 (-1) 이므로, 화살도 좌측을 바라보는 것이 디폴트.
        // 보스가 우측을 바라보고 있다면 (+1), 화살도 우측을 바라보도록 localScale.x를 뒤집음.
        transform.localScale = new Vector3(Mathf.Sign(currentBossScaleX) * Mathf.Abs(transform.localScale.x),
                                           transform.localScale.y, transform.localScale.z);

        // 투사체가 바라보는 방향 (화살 스프라이트 방향)과 이동 방향 일치
        // 화살은 좌측을 바라보는 가로모양 (rotation.z = 0)이 디폴트이므로, 
        // 방향에 따라 y-회전을 추가할 필요는 없고, localScale.x로 뒤집는 것으로 충분합니다.
        // 다만, 날아가는 방향을 바라보게 하고 싶다면 회전 로직이 필요할 수 있습니다.
        // 현재는 localScale.x만으로 방향을 맞춥니다.

        // 초기 속도 적용
        rb.velocity = direction.normalized * initialSpeed;
        rb.gravityScale = 1.0f; // 발사 시 중력 적용 시작

        hasFired = true;
        Debug.Log($"[Arrow] Arrow Fired! Direction: {direction}, Initial Speed: {initialSpeed}, Current Velocity: {rb.velocity}");
    }

    private void FixedUpdate()
    {
        if (!hasFired) return;

        // 수평 속도 감속
        if (Mathf.Abs(rb.velocity.x) > 0.1f) // 너무 작아지면 멈춤
        {
            rb.velocity = new Vector2(rb.velocity.x * decelerationRate, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // 거의 멈췄으면 0으로
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 플레이어에게 접촉
        if (other.CompareTag("Player"))
        {
            DestroyArrow();
        }
        // 2. 바닥에 접촉
        else if (other.CompareTag("Ground"))
        {
            DestroyArrow();
        }
    }

    // 화살 삭제 메서드
    private void DestroyArrow()
    {
        CancelInvoke("DestroyArrow");

        Destroy(gameObject); 
    }
}