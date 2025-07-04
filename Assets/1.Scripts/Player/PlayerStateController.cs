using UnityEngine;
using Player.States;

public class PlayerStateController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
    }

    // �ܺ� ���� ��û �� ���� ������ StateMachine�� ����
    public void RequestStateChange(PlayerState newState)
    {
        StateMachine.ChangeState(newState);
    }

    // ���� ���� ��û (�˻� ����)
    public void SetStateInstantly(PlayerState newState)
    {
        StateMachine.ForceChangeState(newState);
    }

    // ���� �Ǵܿ� ���� �޼���
    public bool Is(PlayerState state) => StateMachine.CurrentEnumState == state;
    public bool IsBusy() => StateMachine.CurrentEnumState is PlayerState.Attacking or PlayerState.SkillCasting or PlayerState.Knockback;
    public bool IsControllable() => StateMachine.CurrentEnumState is PlayerState.Idle or PlayerState.Moving;
}