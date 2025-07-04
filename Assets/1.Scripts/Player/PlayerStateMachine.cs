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
    /// enum ��� ���� ����. ���� �ʿ��� �⺻ FSM ���·� ��ȯ�� �� ���.
    /// CanTransitionTo �˻� ����.
    /// </summary>
    public void ChangeState(PlayerState newState)
    {
        if (!stateMap.ContainsKey(newState)) return;
        
        //  ���� ���� ���� ����
        if (currentEnumState == newState)
        {
            Debug.LogWarning($"[FSM] �ߺ� ���� ���� �õ�: {newState} �� ���õ�");
            return;
        }

        // ���� ���� ���� ���� üũ (�� ������ CanTransitionTo �̿�)
        if (currentStateInstance != null && !currentStateInstance.CanTransitionTo(newState))
        {
            Debug.Log($"[FSM] ���� ���� �źε�: {currentEnumState} �� {newState}");
            return;
        }

        Debug.Log($"[FSM] ���� ����: {currentEnumState} �� {newState}");

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
            Debug.LogWarning("[FSM] Dead ���¿��� ���� ���� �õ� �� ���õ�");
            return;
        }

        currentStateInstance?.Exit(controller);
        currentStateInstance = stateMap[newState];
        currentEnumState = newState;

        Debug.Log($"[FSM] ���� ���� ����: �� {newState}");
        currentStateInstance.Enter(controller);
    }
    /// <summary>
    /// ���� ������ ���� �ν��Ͻ��� ����.
    /// KnockbackState �� �Ķ���� ��� ���� ���̿� ����.
    /// </summary>
    public void ChangeState(IPlayerState newState, PlayerStateController controller)
    {
        currentStateInstance?.Exit(controller);
        currentStateInstance = newState;
        currentStateInstance.Enter(controller);
    }

}
