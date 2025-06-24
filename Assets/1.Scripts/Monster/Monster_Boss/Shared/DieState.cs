using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;

namespace TutorialBoss.States
{
    public class DieState : BaseTutorialBossState
    {
        public DieState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.isDead = true;
            controller.animator.Play($"{controller.bossName}_Die");

            controller.rb.velocity = Vector2.zero;
            controller.rb.bodyType = RigidbodyType2D.Kinematic;
            controller.tag = "Untagged";

            controller.StartCoroutine(DieEndDelay());
        }

        public override void Execute() { }

        public override void Exit() { }

        private IEnumerator DieEndDelay()
        {
            yield return new WaitForSeconds(1.5f);

            var deathTrigger = controller.GetComponent<MonsterDeathTrigger>();
            if (deathTrigger != null)
            {
                deathTrigger.OnMonsterDie();
            }

            controller.gameObject.SetActive(false);
        }
    }
}
