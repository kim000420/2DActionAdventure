using Player.States;
using UnityEngine;

public class DeadState : IPlayerState
{
    private PlayerStateController controller;
    private PlayerAnimationController anim;
    private Rigidbody2D rb;
    public void Enter(PlayerStateController controller)
    {
        this.controller = controller;
        anim = controller.GetComponent<PlayerAnimationController>();
        rb = controller.GetComponent<Rigidbody2D>();

        anim.PlayTrigger("dead");
        rb.velocity = Vector2.zero;

        Debug.Log("[State] Dead 상태 진입");
    }
    public void Update(PlayerStateController controller)
    { 
    }
    public void Exit(PlayerStateController controller)
    {
        Debug.Log("[State] Dead 상태 종료");
        controller.RequestStateChange(PlayerState.Idle);
    }
    public bool CanTransitionTo(PlayerState nextState)
    {
        // 사망 상태에서는 어떤 상태로도 전이 불가능
        return false;
    }
}
