using System.Collections;
using UnityEngine;

public enum DamageType { Normal, GuardBreak }
public enum KnockbackType { None, Weak, Strong }

public class HitboxTrigger : MonoBehaviour
{
    public int damage = 10;
    public DamageType damageType = DamageType.Normal;
    public KnockbackType knockbackType = KnockbackType.Weak;
    public float horizontalForce = 5f;
    public float verticalForce = 2f;
    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            var stats = other.GetComponent<PlayerStats>();
            var controller = other.GetComponent<PlayerStateController>();

            if (stats != null && controller != null)
            {
                float attackerX = transform.position.x;
                stats.TakeDamage(damage, damageType, knockbackType, horizontalForce, verticalForce, attackerX);
                StartCoroutine(ResetHitFlagAfterDelay());
                hasHit = true;
            }
        }
    }

    private IEnumerator ResetHitFlagAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        hasHit = false;
    }
}