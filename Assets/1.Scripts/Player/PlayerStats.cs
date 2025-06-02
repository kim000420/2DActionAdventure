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
            // Dead 상태 전이
            controller.RequestStateChange(PlayerState.Dead);

            // 사망 연출 후 귀환 처리 (1초 후)
            Invoke(nameof(RespawnToHomeScene), 1f);
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

    private void RespawnAtReturnPoint()
    {
        SpawnPoint returnPoint = FindObjectsOfType<SpawnPoint>()
            .FirstOrDefault(p => p.spawnType == SpawnType.Return);

        if (returnPoint != null)
        {
            transform.position = returnPoint.transform.position;
            controller.ForceStateChange(PlayerState.Idle);
            RestoreToFull(); // 체력/스태미너 회복
            Debug.Log("[Respawn] 귀환 완료");
        }
        else
        {
            Debug.LogWarning("[Respawn] Return 스폰포인트를 찾을 수 없습니다.");
        }
    }

    public void RestoreToFull()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        OnHPChanged?.Invoke(currentHP, maxHP);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
    private void RespawnToHomeScene()
    {
        SceneTransitionManager.Instance.RespawnToScene("Home", "R_home_bed");
    }

}
