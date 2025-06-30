using UnityEngine;
using Player.States;
using System.Linq;

public enum SubWeaponType
{
    Shotgun,
    Grenade,
    Flashbang,
    Smoke
}

[System.Serializable]
public class FoodItem
{
    public string foodName;
    public int healAmount;
    public int useCount;
}


public class PlayerStats : MonoBehaviour
{
    [Header("체력")]
    public int maxHP = 100;
    public int currentHP;
    [Header("스테미너")]
    public int maxStamina = 100;
    public int currentStamina;
    [Header("기초 스텟")]
    public int attackPower = 10;
    public int groggyPower = 1;
    [Header("치명 스텟")]
    public float criticalChance = 0.1f;
    public float criticalMultiplier = 1.5f;
    [Header("스킬 쿨감 스텟")]
    public float skillCooldownReduction = 0f;

    [Header("발차기 스킬렙")]
    public int kickLevel = 1; // 1~3: 콤보 단계 허용

    [Header("장착중인 서브웨폰")]
    public SubWeaponType subWeapon = SubWeaponType.Shotgun;

    [Header("착용중인 회복 음식")]
    public FoodItem foodItem;


    public float staminaRecoveryRate = 1f;
    private float recoveryTimer = 0.1f;
    private bool isGuarding = false;

    public System.Action<int, int> OnHPChanged;
    public System.Action<int, int> OnStaminaChanged;

    private PlayerStateController controller;
    private PlayerAttackController attackController;

    private void Awake()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        controller = GetComponent<PlayerStateController>();
        attackController = GetComponent<PlayerAttackController>();
    }

    private void Update()
    {
        AutoRecoverStamina();
    }

    public void UseStamina(int amount)
    {
        currentStamina = Mathf.Max(currentStamina - amount, 0);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina); // 스태미너 변경 이벤트
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP); // 체력 회복 이벤트
    }

    public void TakeDamage(int amount, DamageType type, KnockbackType knockbackType, float forceX, float forceY, float attackerX)
    {
        var state = controller.StateMachine.CurrentStateInstance as GuardState;
        if (state != null && type != DamageType.GuardBreak && amount <= currentStamina)
        {
            float guardElapsed = state.TimeSinceEntered;
            state.OnGuardHit(amount, type, guardElapsed, forceX, forceY, attackerX, controller);
            return;
        }

        currentHP = Mathf.Max(currentHP - amount, 0);
        OnHPChanged?.Invoke(currentHP, maxHP); // 체력 감소 이벤트

        // 넉백 방향 계산을 이곳에서 처리 (공격자 기준)
        float direction = transform.position.x < attackerX ? -1f : 1f;
        Vector2 knockback = new Vector2(forceX * direction, forceY);

        controller.ChangeState(new KnockbackState(knockbackType, knockback));
        attackController.ResetAttackPhase();
        //사망처리
        if (currentHP <= 0)
        {
            if (ShouldBypassDeath())
            {
                Debug.Log("[Respawn] 사망 방지 조건 → 즉시 복귀");
                RespawnWithoutDeath(); // 씬 이동 없이 포지션 + HP 복원
                return;
            }

            // Dead 상태 전이
            controller.RequestStateChange(PlayerState.Dead);

            // 사망 연출 후 귀환 처리 (1초 후)
            Invoke(nameof(RespawnToLastSavedPoint), 2f);
        }
    }

    private void AutoRecoverStamina()
    {
        var state = controller.StateMachine.CurrentStateInstance;
        if (state is GuardState) return;
        if (state is SkillCastingState) return;

        recoveryTimer += Time.deltaTime;
        if (recoveryTimer >= 1f)
        {
            currentStamina = Mathf.Min(currentStamina + (int)staminaRecoveryRate, maxStamina);
            recoveryTimer = 0f;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina); // 자동 회복 이벤트
        }
    }

    public void SetGuarding(bool value)
    {
        isGuarding = value;
    }

    public void RestoreToFull()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        OnHPChanged?.Invoke(currentHP, maxHP);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
    private void RespawnToLastSavedPoint()
    {
        SceneTransitionManager.Instance.RespawnToLastSavedPoint();
    }
    private bool ShouldBypassDeath()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // ✅ 씬 기반 예시
        if (currentScene == "Village_halbe_Guild4" || currentScene == "Home_Out_2-2")
            return true;

        return false;
    }

    private void RespawnWithoutDeath()
    {
        // 현재 위치 or 지정된 리스폰 위치로 이동
        string id = SceneTransitionManager.Instance.currentSpawnId;

        var point = GameObject.FindObjectsOfType<SpawnPoint>()
            .FirstOrDefault(p => p.spawnID == id);

        if (point != null)
            transform.position = point.transform.position;

        RestoreToFull(); // 체력/스태미너 회복
        controller.ForceStateChange(PlayerState.Idle);
    }

}
