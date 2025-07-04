using UnityEngine;

namespace Player.States
{
    public class AttackState : IPlayerState
    {
        public void Enter(PlayerStateController controller)
        {
            // ���� �帧 ���� ��û �� Controller�� �޺� A���� ����
            var attack = controller.GetComponent<PlayerAttackController>();
            attack?.BeginComboStep(0); // 0�ܰ� �޺� ����

            // �̵� ����
            controller.GetComponent<PlayerMotor>()?.EnableMovementOverride();
        }

        public void Update(PlayerStateController controller) { }
        public void Exit(PlayerStateController controller)
        {
            var attack = controller.GetComponent<PlayerAttackController>();
            attack?.ResetAttackPhase();

            controller.GetComponent<PlayerMotor>()?.DisableMovementOverride();
        }
        public bool CanTransitionTo(PlayerState nextState)
        {
            return nextState is PlayerState.Attacking or
                                 PlayerState.Hit or
                                 PlayerState.Knockback or
                                 PlayerState.Idle or
                                 PlayerState.Dead;
        }
    }
}