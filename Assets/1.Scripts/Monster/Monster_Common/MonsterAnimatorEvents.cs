using UnityEngine;
using Monster.CommonStates;

public class MonsterAnimatorEvents : MonoBehaviour
{
    [Header("FSM 컨트롤러")]
    public MonsterStateController controller;

    [Header("히트박스")]
    public GameObject hitbox_Attack;

    public void EnableHitbox_Attack()
    {
        if (hitbox_Attack != null)
            hitbox_Attack.SetActive(true);
    }

    public void DisableHitbox_Attack()
    {
        if (hitbox_Attack != null)
            hitbox_Attack.SetActive(false);
    }

    public void OnAttackAnimationEnd()
    {
        if (controller != null && !controller.isDead)
        {
            controller.ChangeState(new IdleState(controller));
        }
    }
}

