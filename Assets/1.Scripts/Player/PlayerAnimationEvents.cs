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
        Debug.Log("[Skill] 스킬 종료 및 상태 복귀 (강제)");
    }
}