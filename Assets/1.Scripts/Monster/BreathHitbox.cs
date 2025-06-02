using UnityEngine;
using System.Collections;

public class BreathHitbox : MonoBehaviour
{
    public int damage = 15;
    public DamageType damageType = DamageType.Normal;
    public KnockbackType knockbackType = KnockbackType.Weak;
    public float horizontalForce = 2f;
    public float verticalForce = 1f;

    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        var stats = other.GetComponent<PlayerStats>();
        if (stats != null)
        {
            float attackerX = transform.position.x;
            stats.TakeDamage(damage, damageType, knockbackType, horizontalForce, verticalForce, attackerX);
            StartCoroutine(ResetHit());
            hasHit = true;
        }
    }

    private IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(3f);
        hasHit = false;
    }
}
