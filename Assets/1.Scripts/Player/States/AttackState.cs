using UnityEngine;

namespace Player.States
{
    public class AttackState : IPlayerState
    {
        private int comboIndex = 0;

        public void Enter(PlayerStateController controller)
        {
            var anim = controller.GetComponent<PlayerAnimationController>();
            string trigger = $"attack_Combo_{(char)('A' + comboIndex)}";
            anim.PlayTrigger(trigger);
            controller.StartCoroutine(EndAfterDelay(controller, 0.4f));

            comboIndex = (comboIndex + 1) % 3;
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