using Player.States;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerStateController stateController;
    private Dictionary<PlayerState, IPlayerState> stateMap;
    private IPlayerState currentStateInstance;

    public IPlayerState CurrentStateInstance => currentStateInstance;

    private void Awake()
    {
        stateController = GetComponent<PlayerStateController>();
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
            { PlayerState.Hit, new HitState() },
            { PlayerState.Knockback, new KnockbackState() },
            { PlayerState.Dead, new DeadState() },
        };
    }

    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        currentStateInstance?.Update(stateController);
    }
    /// <summary>
    /// enum 기반 상태 전이. 상태 맵에서 기본 FSM 상태로 전환할 때 사용.
    /// CanTransitionTo 검사 포함.
    /// </summary>
    public void ChangeState(PlayerState newState)
    {
        if (!stateMap.ContainsKey(newState)) return;

        // 상태 전이 가능 여부 체크 (각 상태의 CanTransitionTo 이용)
        if (currentStateInstance != null && !currentStateInstance.CanTransitionTo(newState))
        {
            Debug.Log($"[StateMachine] {stateController.CurrentState} → {newState} 전이 거부됨");
            return;
        }

        currentStateInstance?.Exit(stateController);
        stateController.ChangeState(newState);
        currentStateInstance = stateMap[newState];
        currentStateInstance.Enter(stateController);
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
