using UnityEngine;
using Monster.States;
public class ExplosionHitboxTrigger : MonoBehaviour
{
    private ThrowableExploder exploder;

    private void Awake()
    {
        exploder = GetComponentInParent<ThrowableExploder>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var stats = other.GetComponent<MonsterStats>();
            if (stats != null)
            {
                stats.ApplyHit(exploder.damage, exploder.groggyDamage, exploder.knockback, exploder.transform.position);
            }
        }
    }
}
