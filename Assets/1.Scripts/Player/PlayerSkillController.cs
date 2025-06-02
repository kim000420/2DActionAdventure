using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    private PlayerStateController controller;
    private PlayerStats stats;
    private PlayerAnimationController anim;
    private PlayerMotor motor;
    private PlayerSkillUIController skillUI;

    [Header("Kick Combo")]
    public GameObject[] kickHitboxes;
    private int currentKickStep = 0;
    private int kickMaxStep = 0;
    
    [Header("Sub Weapon")]
    public GameObject shotgunHitbox;
    
    [Header("Throwable Prefabs")]
    public GameObject grenadePrefab;
    public GameObject flashbangPrefab;
    public Transform throwPoint;
    public float throwSpeed = 10f;


    private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();
    private Dictionary<string, float> skillLastUsedTime = new Dictionary<string, float>();
    private Dictionary<string, int> skillStaminaCosts = new Dictionary<string, int>();

    private void Awake()
    {
        controller = GetComponent<PlayerStateController>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<PlayerAnimationController>();
        motor = GetComponent<PlayerMotor>();
        skillUI = FindObjectOfType<PlayerSkillUIController>();

        // 초기 쿨타임 설정 (기본값)
        skillCooldowns["Kick"] = 5f;
        skillCooldowns["Sub"] = 5f;
        skillCooldowns["Food"] = 2f;

        // 초기 스태미나 소모 설정
        skillStaminaCosts["Kick"] = 40;
        skillStaminaCosts["Sub"] = 20;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) TryUseKick();
        if (Input.GetKeyDown(KeyCode.W)) TryUseSubWeapon();
        if (Input.GetKeyDown(KeyCode.E)) TryUseFood();
    }
    private bool IsSkillAvailable(string skillKey)
    {
        if (!skillLastUsedTime.ContainsKey(skillKey)) return true;

        float cooldown = skillCooldowns[skillKey] * (1f - stats.skillCooldownReduction);
        float elapsed = Time.time - skillLastUsedTime[skillKey];
        return elapsed >= cooldown;
    }
    private bool HasEnoughStamina(string skillKey)
    {
        if (!skillStaminaCosts.ContainsKey(skillKey)) return true;
        return stats.currentStamina >= skillStaminaCosts[skillKey];
    }
    private void ConsumeStamina(string skillKey)
    {
        if (skillStaminaCosts.ContainsKey(skillKey))
        {
            stats.UseStamina(skillStaminaCosts[skillKey]);
        }
    }

    private void StartCooldown(string skillKey)
    {
        skillLastUsedTime[skillKey] = Time.time;
    }

    private void TryUseKick()
    {
        if (stats.kickLevel <= 0) return;
        if (!controller.CanTransitionTo(PlayerState.SkillCasting)) return;
        if (!IsSkillAvailable("Kick") || !HasEnoughStamina("Kick")) return;

        controller.RequestStateChange(PlayerState.SkillCasting);
        motor.EnableMovementOverride();

        currentKickStep = 0;
        kickMaxStep = Mathf.Clamp(stats.kickLevel, 1, 3);
        PlayKickStep(currentKickStep);
        StartCooldown("Kick");
        ConsumeStamina("Kick");
    }

    private void PlayKickStep(int step)
    {
        anim.PlayTrigger($"AnyWeapon_Skill_Kick_{(char)('A' + step)}");

        if (step >= 1) // Kick_B, Kick_C만 전진 포함
            motor.ForwardImpulse(1.0f, 0.2f); // 거리/시간은 조정 가능
    }

    public void OnKickHit(int step)
    {
        if (step >= 0 && step < kickHitboxes.Length)
        {
            EnableHitboxDirect(kickHitboxes[step]);
        }
    }
    //Kick ABC에서 호출 다음 Kick사용할수 있는지 체크후 넘김
    public void OnKickNext(int step)
    {
        if (step + 1 < kickMaxStep)
        {
            currentKickStep++;
            PlayKickStep(currentKickStep);
        }
    }
    private void EnableHitboxDirect(GameObject hitbox)
    {
        if (hitbox == null) return;

        Vector3 pos = hitbox.transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * motor.LastDirection;
        hitbox.transform.localPosition = pos;

        Vector3 scale = hitbox.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * motor.LastDirection;
        hitbox.transform.localScale = scale;

        hitbox.SetActive(true);
        Invoke(nameof(DisableAllHitboxes), 0.3f);
    }
    private void DisableAllHitboxes()
    {
        foreach (var hitbox in kickHitboxes)
        {
            if (hitbox != null)
                hitbox.SetActive(false);
        }

        if (shotgunHitbox != null)
            shotgunHitbox.SetActive(false);
    }

    private void TryUseSubWeapon()
    {
        if (!controller.CanTransitionTo(PlayerState.SkillCasting)) return;
        if (!IsSkillAvailable("Sub") || !HasEnoughStamina("Sub")) return;

        controller.RequestStateChange(PlayerState.SkillCasting);
        motor.EnableMovementOverride();

        switch (stats.subWeapon)
        {
            case SubWeaponType.Shotgun:
                anim.PlayTrigger("SubWeapon_Skill_Shotgun");
                break;
            case SubWeaponType.Grenade:
                anim.PlayTrigger("SubWeapon_Skill_Throw");
                break;
            case SubWeaponType.Flashbang:
                anim.PlayTrigger("SubWeapon_Skill_Throw");
                break;
            case SubWeaponType.Smoke:
                anim.PlayTrigger("SubWeapon_Skill_Throw");
                break;
        }

        Debug.Log($"[Skill] 보조무기 사용: {stats.subWeapon}");
        StartCooldown("Sub");
        ConsumeStamina("Sub");
    }
    public void OnShotgunHit()
    {
        EnableHitboxDirect(shotgunHitbox);
    }

    private void TryUseFood()
    {
        if (stats.foodItem == null || stats.foodItem.useCount <= 0) return;
        if (!controller.CanTransitionTo(PlayerState.SkillCasting)) return;
        if (!IsSkillAvailable("Food")) return;

        controller.RequestStateChange(PlayerState.SkillCasting);
        motor.EnableMovementOverride();
        anim.PlayTrigger("AnyWeapon_Skill_Food");
        stats.foodItem.useCount--;

        stats.Heal(stats.foodItem.healAmount);

        skillUI?.UpdateFoodUseCount();

        Debug.Log("[Skill] 음식 섭취 시작");
        StartCooldown("Food");
    }
    public void ThrowSubWeapon()
    {
        GameObject prefab = null;

        switch (stats.subWeapon)
        {
            case SubWeaponType.Grenade:
                prefab = grenadePrefab;
                break;
            case SubWeaponType.Flashbang:
                prefab = flashbangPrefab;
                break;
        }

        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = new Vector2(motor.LastDirection, 0f); // 정면
            rb.velocity = dir.normalized * throwSpeed;
        }
    }
    public float GetRemainingCooldown(string skillKey)
    {
        if (!skillLastUsedTime.ContainsKey(skillKey)) return 0f;

        float cooldown = skillCooldowns[skillKey] * (1f - stats.skillCooldownReduction);
        float elapsed = Time.time - skillLastUsedTime[skillKey];
        return Mathf.Clamp(cooldown - elapsed, 0f, cooldown);
    }

    public bool IsSkillUsable(string skillKey) => IsSkillAvailable(skillKey);
    public float GetSkillCooldown(string skillKey)
    {
        if (!skillCooldowns.ContainsKey(skillKey)) return 1f;
        return skillCooldowns[skillKey] * (1f - stats.skillCooldownReduction);
    }
}
