﻿using UnityEngine;
using System.Collections;
using TutorialBoss.Controller;
using TutorialBoss.States.Jo;
using TutorialBoss.States.Bow;
using TutorialBoss.States.Webuin;

namespace TutorialBoss.States
{
    public class HitState : BaseTutorialBossState
    {
        private Vector2 attackerPos;
        private float knockbackForce;
        private Coroutine hitCoroutine;

        public HitState(TutorialBossStateController controller, Vector2 attackerPosition, float force)
            : base(controller)
        {
            attackerPos = attackerPosition;
            knockbackForce = force;
        }

        public override void Enter()
        {
            controller.animator.Play($"{controller.bossName}_Hit");

            // 넉백 적용
            if (controller.rb != null)
            {
                Vector2 dir = ((Vector2)controller.transform.position - attackerPos).normalized;
                if (dir == Vector2.zero) dir = Vector2.left;

                controller.rb.velocity = Vector2.zero;
                controller.rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }

            // 기존 코루틴 중단 후 갱신
            if (hitCoroutine != null)
                controller.StopCoroutine(hitCoroutine);

            hitCoroutine = controller.StartCoroutine(ReturnToIdleAfterDelay());
        }

        private IEnumerator ReturnToIdleAfterDelay()
        {
            float knockbackDuration = Mathf.Clamp(knockbackForce * 0.2f, 0.3f, 0.8f);

            yield return new WaitForSeconds(knockbackDuration);

            switch (controller.bossName)
            {
                case "Jo":
                    controller.StartCoroutine(HitRecovery());
                    controller.ChangeState(new JoIdleState(controller));
                    break;
                case "Bow":
                    controller.StartCoroutine(HitRecovery());
                    controller.ChangeState(new BowEscapeState(controller));
                    break;
                case "Webuin":
                    controller.StartCoroutine(HitRecovery());
                    controller.ChangeState(new WebuinChaseState(controller));
                    break;
            }
        }

        private IEnumerator HitRecovery()
        {
            controller.isHitRecovery = true;
            yield return new WaitForSeconds(1f);
            controller.isHitRecovery = false;
        }


        public override void Exit()
        {
            if (controller.rb != null)
                controller.rb.velocity = Vector2.zero;

            // 코루틴 정리 (선택사항, StopCoroutine으로 충분)
            hitCoroutine = null;
        }

        public override void Execute()
        {
        }
    }
}
