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

    public void ChangeState(PlayerState newState)
    {
        if (!stateMap.ContainsKey(newState)) return;

        currentStateInstance?.Exit(stateController);
        stateController.ChangeState(newState);
        currentStateInstance = stateMap[newState];
        currentStateInstance.Enter(stateController);
    }
    public void ChangeState(IPlayerState newState, PlayerStateController controller)
    {
        currentStateInstance?.Exit(controller);
        currentStateInstance = newState;
        currentStateInstance.Enter(controller);
    }

}
