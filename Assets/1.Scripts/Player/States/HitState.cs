using UnityEngine;

namespace Player.States
{
    public class HitState : IPlayerState
    {
        public void Enter(PlayerStateController controller)
        {
            controller.GetComponent<PlayerAnimationController>().PlayTrigger("HitDamage");
            controller.StartCoroutine(EndAfterDelay(controller, 0.4f));
        }

        public void Update(PlayerStateController controller) { }
        public void Exit(PlayerStateController controller) { }
        public bool CanTransitionTo(PlayerState nextState)
        {
            // Hit 상태에서는 Knockback, Dead 외에는 제한
            return nextState is PlayerState.Knockback or PlayerState.Dead;
        }
        private System.Collections.IEnumerator EndAfterDelay(PlayerStateController controller, float delay)
        {
            yield return new WaitForSeconds(delay);
            controller.ChangeState(PlayerState.Idle);
        }
    }
}