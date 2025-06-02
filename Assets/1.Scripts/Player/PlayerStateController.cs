using UnityEngine;
using Player.States;

public class PlayerStateController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    public bool Is(PlayerState state) => CurrentState == state;
    public bool IsBusy() => CurrentState is PlayerState.Attacking or PlayerState.SkillCasting or PlayerState.Knockback;
    private void Awake()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
    }
    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == newState) return;
        Debug.Log($"[State] {CurrentState} → {newState}");
        CurrentState = newState;
    }
    public void ChangeState(IPlayerState newState)
    {
        if (newState is KnockbackState)
            CurrentState = PlayerState.Knockback;
        else if (newState is GuardState)
            CurrentState = PlayerState.Guarding;
        else if (newState is CrouchState)
            CurrentState = PlayerState.Crouching;

        StateMachine.ChangeState(newState, this);
    }

    public bool CanTransitionTo(PlayerState nextState)
    {
        // 스킬 시전 상태는 Idle,Moving일 때만 진입 가능
        if (nextState == PlayerState.SkillCasting &&
            CurrentState != PlayerState.Idle &&
            CurrentState != PlayerState.Moving)
            return false;
        // 공격 중 → 다른 상태 차단 (단, 피격/사망 허용)
        if (CurrentState == PlayerState.Attacking)
        {
            return nextState is PlayerState.Attacking or PlayerState.Hit or PlayerState.Knockback or PlayerState.Dead;
        }

        // 스킬 중 → 피격/사망/가드만 허용
        if (CurrentState == PlayerState.SkillCasting)
        {
            return nextState is PlayerState.Hit or PlayerState.Knockback or PlayerState.Dead or PlayerState.Guarding;
        }

        return true;
    }


    public void RequestStateChange(PlayerState newState)
    {
        if (!CanTransitionTo(newState)) return;
        Debug.Log($"[State] {CurrentState} → {newState}");
        GetComponent<PlayerStateMachine>().ChangeState(newState);
    }

    //스킬을 사용할 수 있는 상태
    public bool IsControllable()
    {
        return CurrentState is PlayerState.Idle or PlayerState.Moving;
    }

    //강제 상태 전이
    public void ForceStateChange(PlayerState state)
    {
        CurrentState = state;
        GetComponent<PlayerStateMachine>().ChangeState(state);
    }


}