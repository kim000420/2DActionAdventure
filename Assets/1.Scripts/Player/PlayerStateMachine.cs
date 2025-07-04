using Player.States;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerStateController controller;
    private Dictionary<PlayerState, IPlayerState> stateMap;
    private IPlayerState currentStateInstance;
    private PlayerState currentEnumState;

    public PlayerState CurrentEnumState => currentEnumState;
    public IPlayerState CurrentStateInstance => currentStateInstance;
    private void Awake()
    {
        controller = GetComponent<PlayerStateController>();
        stateMap = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, new IdleState() },
            { PlayerState.Moving, new MoveState() },
            { PlayerState.Jumping, new JumpState() },
            { PlayerState.Rolling, new RollState() },
            { PlayerState.Guarding, new GuardState() },
            { PlayerState.Crouching, new CrouchState() },
            { PlayerState.Attacking, new AttackState() },
            { PlayerState.SkillCasting, new SkillCastingState() },
            { PlayerState.Interacting, new InteractState() },
            { PlayerState.Knockback, new KnockbackState(KnockbackType.Weak, Vector2.zero) },
            { PlayerState.Dead, new DeadState() }
        };
    }

    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        currentStateInstance?.Update(controller);
    }
    /// <summary>
    /// enum 기반 상태 전이. 상태 맵에서 기본 FSM 상태로 전환할 때 사용.
    /// CanTransitionTo 검사 포함.
    /// </summary>
    public void ChangeState(PlayerState newState)
    {
        if (!stateMap.ContainsKey(newState)) return;
        
        //  동일 상태 전이 방지
        if (currentEnumState == newState)
        {
            Debug.LogWarning($"[FSM] 중복 상태 전이 시도: {newState} → 무시됨");
            return;
        }

        // 상태 전이 가능 여부 체크 (각 상태의 CanTransitionTo 이용)
        if (currentStateInstance != null && !currentStateInstance.CanTransitionTo(newState))
        {
            Debug.Log($"[FSM] 상태 전이 거부됨: {currentEnumState} → {newState}");
            return;
        }

        Debug.Log($"[FSM] 상태 전이: {currentEnumState} → {newState}");

        currentStateInstance?.Exit(controller);
        currentStateInstance = stateMap[newState];
        currentEnumState = newState;

        currentStateInstance.Enter(controller);
    }
    public void ForceChangeState(PlayerState newState)
    {
        if (!stateMap.ContainsKey(newState)) return;

        if (currentEnumState == PlayerState.Dead)
        {
            Debug.LogWarning("[FSM] Dead 상태에서 강제 전이 시도 → 무시됨");
            return;
        }

        currentStateInstance?.Exit(controller);
        currentStateInstance = stateMap[newState];
        currentEnumState = newState;

        Debug.Log($"[FSM] 상태 강제 전이: → {newState}");
        currentStateInstance.Enter(controller);
    }
    /// <summary>
    /// 직접 생성된 상태 인스턴스로 전이.
    /// KnockbackState 등 파라미터 기반 상태 전이에 적합.
    /// </summary>
    public void ChangeState(IPlayerState newState, PlayerStateController controller)
    {
        currentStateInstance?.Exit(controller);
        currentStateInstance = newState;
        currentStateInstance.Enter(controller);
    }

}
