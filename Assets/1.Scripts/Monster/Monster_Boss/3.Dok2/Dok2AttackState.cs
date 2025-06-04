using UnityEngine;
using TutorialBoss.Controller;
using System.Collections; // Coroutine 사용을 위해 추가

namespace TutorialBoss.States.Dok2
{
    public class Dok2AttackState : BaseTutorialBossState
    {
        public Dok2AttackState(TutorialBossStateController controller) : base(controller) { }

        public override void Enter()
        {
            controller.rb.velocity = Vector2.zero; // 공격 중 이동 멈춤

            // 공격 쿨타임 중이 아니라면 공격 실행
            if (!controller.isAttackCooldown)
            {
                if (controller.attack1Count >= 3)
                {
                    // Attack1을 3번 사용했다면 Attack2 사용 후 스택 초기화
                    controller.animator.Play($"{controller.bossName}_Attack2");
                    controller.attack1Count = 0; // 스택 초기화
                }
                else
                {
                    // Attack1 사용
                    controller.animator.Play($"{controller.bossName}_Attack1");
                    controller. attack1Count++;
                }
            }
            else
            {
                // 쿨타임 중이라면 즉시 ChaseState로 전환 (원치 않는 공격 방지)
                controller.ChangeState(new Dok2ChaseState(controller));
            }
        }

        public override void Execute()
        {
        
        }

        public override void Exit()
        {
        }
    }
}