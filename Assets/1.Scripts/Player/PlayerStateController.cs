using UnityEngine;
using Player.States;

public class PlayerStateController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
    }

    // 외부 전이 요청 → 실제 로직은 StateMachine에 위임
    public void RequestStateChange(PlayerState newState)
    {
        StateMachine.ChangeState(newState);
    }

    // 강제 전이 요청 (검사 없이)
    public void SetStateInstantly(PlayerState newState)
    {
        StateMachine.ForceChangeState(newState);
    }

    // 상태 판단용 보조 메서드
    public bool Is(PlayerState state) => StateMachine.CurrentEnumState == state;
    public bool IsBusy() => StateMachine.CurrentEnumState is PlayerState.Attacking or PlayerState.SkillCasting or PlayerState.Knockback;
    public bool IsControllable() => StateMachine.CurrentEnumState is PlayerState.Idle or PlayerState.Moving;
}