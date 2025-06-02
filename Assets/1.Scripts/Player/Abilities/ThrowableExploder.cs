using UnityEngine;
using Monster.States;

public class ThrowableExploder : MonoBehaviour
{
    [Header("Explosion Settings")]
    public Rigidbody2D rb;

    public float lifeTime = 2f;
    public int damage = 50;
    public int groggyDamage = 40;
    public float knockback = 5f;

    [Header("Animation Settings")]
    [SerializeField] private Animator anim;
    [SerializeField] private float explosionType; // 0 = 수류탄, 0.5 = 섬광탄, 1 = 연막탄
    [SerializeField] private string explosionTrigger = "Explosion_A"; //Explosion_B = 연막탄 이펙트
    [SerializeField] private GameObject explosionHitbox;

    private bool hasExploded = false;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke(nameof(TriggerExplosionAnimation), lifeTime);
    }
    private void Update()
    {
        if (!hasExploded)
        {
            transform.Rotate(0f, 0f, 360f * Time.deltaTime); // 1초에 1바퀴 회전
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) TriggerExplosionAnimation();
    }
    private void DisableExplosionHitbox()
    {
        if (explosionHitbox != null)
            explosionHitbox.SetActive(false);
    }
    // 충돌 또는 수명이 다 되었을 때 호출되는 폭발 애니메이션 트리거 함수
    private void TriggerExplosionAnimation()
    {
        if (hasExploded) return;
        hasExploded = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.rotation = Quaternion.identity;

        anim.SetFloat("ExplosionType", explosionType);
        anim.SetTrigger(explosionTrigger);

        // 히트박스 켜기
        if (explosionHitbox != null)
            explosionHitbox.SetActive(true);

        Invoke(nameof(DisableExplosionHitbox), 0.3f);
    }
    // 애니메이션 마지막 프레임에서 호출되어 오브젝트 제거
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
