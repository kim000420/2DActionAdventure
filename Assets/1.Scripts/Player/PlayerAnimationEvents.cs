using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerStateController controller;
    private PlayerAnimationController anim;
    private PlayerAttackController attack;
    private PlayerSkillController skill;
    private PlayerMotor motor;

    private void Awake()
    {
        controller = GetComponent<PlayerStateController>();
        anim = GetComponent<PlayerAnimationController>();
        attack = GetComponent<PlayerAttackController>();
        skill = GetComponent<PlayerSkillController>();
        motor = GetComponent<PlayerMotor>();
    }

    // Wakeup 애니메이션 종료 시 호출될 함수 (애니메이션 이벤트에서 연결)
    public void OnKnockbackEnd()
    {
        if (anim != null)
        {
            anim.SetBool("isKnockback", false);
        }

        if (controller != null && controller.CurrentState == PlayerState.Knockback)
        {
            controller.RequestStateChange(PlayerState.Idle);
        }
    }
    public void OnLanded()
    {
        anim.SetTrigger("landedFromKnockback");
    }
    // PlayerAnimationEvents.cs 내부
    public void OnAttackEnd()
    {
        var controller = GetComponent<PlayerStateController>();
        var attackController = GetComponent<PlayerAttackController>();

        // 콤보 종료 or 입력 끊김 → Idle 전이 및 초기화 필요
        if (controller != null && attackController != null)
        {
            controller.ForceStateChange(PlayerState.Idle);
            attackController.ResetAttackPhase();
            motor.StopImmediately();
        }
    }
    public void EnableComboHitbox(int step)
    {
        if (attack.comboHitboxes.Length > step)
            attack.EnableHitboxDirect(attack.comboHitboxes[step]);
    }

    public void EnableStrongHitbox()
    {
        attack.EnableHitboxDirect(attack.strongHitbox);
    }

    public void EnableFinishHitbox()
    {
        attack.EnableHitboxDirect(attack.finishHitbox);
    }
    public void EnableKickHitbox(int step)
    {
        if (skill != null)
        {
            skill.OnKickHit(step);
        }
    }
    public void EnableShotgunHitbox()
    {
        if (skill != null)
        {
            skill.OnShotgunHit();
        }
    }
    public void EndSkillCast()
    {
        controller.ForceStateChange(PlayerState.Idle);
        motor.DisableMovementOverride();
        motor.StopImmediately();
        Debug.Log("[Skill] 스킬 종료 및 상태 복귀 (강제)");
    }
    public void PlaySound_Landing() // 착지 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_player_landing);
    }
    public void PlaySound_Walk() // 걷기 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_player_walk);
    }
    public void PlaySound_Roll() // 구르기 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_player_roll);
    }
    public void PlaySound_Attack_Combo1() // 콤보공격 1 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_11);
    }
    public void PlaySound_Attack_Combo2() // 콤보공격 2 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_12);
    }
    public void PlaySound_Attack_Combo3() // 콤보공격 3 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_13);
    }
    public void PlaySound_Attack_Strong() // 강공격 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_14);
    }
    public void PlaySound_Attack_Finish() // 마무리공격 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_02);
    }
    public void PlaySound_GunFire() // 총발사 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_gun_fire);
    }
    public void PlaySound_GunReload() // 총장전 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_gun_reload);
    }
    public void PlaySound_KickA() // 발차기 1,2 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_strong_01);
    }
    public void PlaySound_KickB() // 발차기 3 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_swing_strong_03);
    }
    public void PlaySound_Heal() // 회복 사운드
    {
        SoundManager.Instance?.PlaySFX(SoundID.sfx_player_landing);
    }
}