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

        private System.Collections.IEnumerator EndAfterDelay(PlayerStateController controller, float delay)
        {
            yield return new WaitForSeconds(delay);
            controller.ChangeState(PlayerState.Idle);
        }
    }
}