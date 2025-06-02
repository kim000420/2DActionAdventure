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

        Debug.Log("[State] Dead ���� ����");
    }
    public void Update(PlayerStateController controller)
    { 
    }
    public void Exit(PlayerStateController controller)
    {
        Debug.Log("[State] Dead ���� ����");
        controller.ChangeState(PlayerState.Idle);
    }

}
