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
    [SerializeField] private float explosionType; // 0 = ����ź, 0.5 = ����ź, 1 = ����ź
    [SerializeField] private string explosionTrigger = "Explosion_A"; //Explosion_B = ����ź ����Ʈ
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
            transform.Rotate(0f, 0f, 360f * Time.deltaTime); // 1�ʿ� 1���� ȸ��
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
    // �浹 �Ǵ� ������ �� �Ǿ��� �� ȣ��Ǵ� ���� �ִϸ��̼� Ʈ���� �Լ�
    private void TriggerExplosionAnimation()
    {
        if (hasExploded) return;
        hasExploded = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.rotation = Quaternion.identity;

        anim.SetFloat("ExplosionType", explosionType);
        anim.SetTrigger(explosionTrigger);

        // ��Ʈ�ڽ� �ѱ�
        if (explosionHitbox != null)
            explosionHitbox.SetActive(true);

        Invoke(nameof(DisableExplosionHitbox), 0.3f);
    }
    // �ִϸ��̼� ������ �����ӿ��� ȣ��Ǿ� ������Ʈ ����
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
