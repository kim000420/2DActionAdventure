using UnityEngine;

namespace Player.States
{
    public class AttackState : IPlayerState
    {
        public void Enter(PlayerStateController controller)
        {
            // 공격 흐름 시작 요청 → Controller가 콤보 A부터 시작
            var attack = controller.GetComponent<PlayerAttackController>();
            attack?.BeginComboStep(0); // 0단계 콤보 시작

            // 이동 차단
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