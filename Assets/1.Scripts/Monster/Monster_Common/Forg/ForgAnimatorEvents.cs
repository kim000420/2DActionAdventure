using UnityEngine;
using CommonMonster.Controller;
using CommonMonster.States.Forg; // ForgAttackState 접근을 위해 추가

namespace CommonMonster.AnimEvents
{
    public class ForgAnimatorEvents : MonoBehaviour
    {
        private CommonMonsterController controller;

        private void Awake()
        {
            controller = GetComponent<CommonMonsterController>();
            if (controller == null)
            {
                Debug.LogError("[ForgAnimatorEvents] CommonMonsterController를 찾을 수 없습니다! 이 스크립트는 CommonMonsterController와 같은 GameObject에 있어야 합니다.");
            }
        }

        // --- Forg Attack Animation Events ---

        // Forg_Attack 애니메이션의 특정 프레임에서 호출될 메서드
        public void Forg_LaunchProjectile()
        {
            if (controller == null) return;

            // ⭐ 현재 상태가 ForgAttackState인지 확인하고, 해당 상태의 메서드를 호출 ⭐
            if (controller.currentState is ForgAttackState currentForgAttackState)
            {
                currentForgAttackState.LaunchProjectile();
            }
            else
            {
                Debug.LogWarning("[ForgAnimatorEvents] Forg_LaunchProjectile 호출 시, 현재 상태가 ForgAttackState가 아닙니다.");
            }
        }

        // Forg_Attack 애니메이션의 끝에서 호출될 메서드
        public void Forg_OnAttackAnimationEnd()
        {
            if (controller == null) return;

            // ⭐ 현재 상태가 ForgAttackState인지 확인하고, 해당 상태의 메서드를 호출 ⭐
            if (controller.currentState is ForgAttackState currentForgAttackState)
            {
                currentForgAttackState.OnAttackAnimationEnd();
            }
            else
            {
                Debug.LogWarning("[ForgAnimatorEvents] Forg_OnAttackAnimationEnd 호출 시, 현재 상태가 ForgAttackState가 아닙니다.");
                // 비정상적인 상황이므로, 안전하게 추적 상태로 전환
                controller.ChangeState(new ForgIdleState(controller));
            }
        }
    }
}