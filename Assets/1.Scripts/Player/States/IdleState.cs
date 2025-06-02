using UnityEngine;

namespace Player.States
{
    public class IdleState : IPlayerState
    {
        private float entryDelay = 0.1f;
        private float delayTimer = 0f;
        public void Enter(PlayerStateController controller)
        {
            controller.GetComponent<PlayerAnimationController>().SetSpeed(0);
            delayTimer = entryDelay;
        }

        /*public void Update(PlayerStateController controller)
        {
            var input = controller.GetComponent<PlayerInputHandler>();
            if (Mathf.Abs(input.Horizontal) > 0)
            {
                controller.ChangeState(PlayerState.Moving);
            }
            else if (input.JumpPressed)
            {
                controller.ChangeState(PlayerState.Jumping);
            }
        }*/
        public void Update(PlayerStateController controller)
        {
            var input = controller.GetComponent<PlayerInputHandler>();

            if (input.CrouchHeld) controller.RequestStateChange(PlayerState.Crouching);
            if (input.GuardHeld) controller.RequestStateChange(PlayerState.Guarding);

            delayTimer -= Time.deltaTime;
            if (delayTimer > 0f)
                return; // 아직 입력 무시 중

            if (Mathf.Abs(input.Horizontal) > 0)
            {
                controller.RequestStateChange(PlayerState.Moving);
            }

            if (input.JumpPressed)
            {
                controller.RequestStateChange(PlayerState.Jumping);
            }
        }


        public void Exit(PlayerStateController controller) { }
    }
}