using Monster.States;
using UnityEngine;
using CommonMonster.States;

public class PlayerHitboxTrigger : MonoBehaviour
{
    [Header("Hitbox Multipliers")]
    public float damageMultiplier = 1.0f;
    public float groggyMultiplier = 1.0f;

    public float knockbackForce = 5f;

    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            var commonMonsterStats = other.GetComponent<CommonMonster.Stats.CommonMonsterStats>();
            var TutorialBossStats = other.GetComponent<TutorialBoss.TutorialBossStats>();
            var attackerStats = transform.root.GetComponent<PlayerStats>();

            if (attackerStats != null)
            {
                Vector2 attackerPos = transform.root.position;

                int baseDamage = attackerStats.attackPower;
                int baseGroggy = attackerStats.groggyPower;

                float finalDamage = baseDamage * damageMultiplier;
                float finalGroggy = baseGroggy * groggyMultiplier;

                float chance = attackerStats.criticalChance;
                float critMult = attackerStats.criticalMultiplier;

                bool isCritical = Random.value < chance;
                if (isCritical)
                {
                    finalDamage *= critMult;
                    Debug.Log("[크리티컬 Hit!] " + Mathf.RoundToInt(finalDamage));
                }
                // 일반몹일떄
                if (commonMonsterStats != null)
                {
                    if (isCritical)
                    {
                        CameraEffectManager.Instance.Shake(ShakeStrength.Medium);
                        CameraEffectManager.Instance.FlashWhite(0.2f);
                    }
                    else
                    {
                        CameraEffectManager.Instance.Shake(ShakeStrength.Weak);
                    }
                    commonMonsterStats.ApplyHit(
                        Mathf.RoundToInt(finalDamage),
                        Mathf.RoundToInt(finalGroggy),
                        knockbackForce,
                        attackerPos
                    );
                    hasHit = true;
                }
                // 튜토보스일때
                else if (TutorialBossStats != null)
                {
                    if (isCritical)
                    {
                        CameraEffectManager.Instance.Shake(ShakeStrength.Medium);
                        CameraEffectManager.Instance.FlashWhite(0.2f);
                    }
                    else
                    {
                        CameraEffectManager.Instance.Shake(ShakeStrength.Weak);
                    }

                    TutorialBossStats.ApplyHit(
                        Mathf.RoundToInt(finalDamage),
                        Mathf.RoundToInt(finalGroggy),
                        knockbackForce,
                        attackerPos
                    );
                    hasHit = true;
                }
            }
        }
    }

}
