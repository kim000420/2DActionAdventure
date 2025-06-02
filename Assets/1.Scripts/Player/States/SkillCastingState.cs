using Player.States;
using UnityEngine;

public class SkillCastingState : IPlayerState
{
    private PlayerStateController controller;
    private PlayerAnimationController anim;
    private Rigidbody2D rb;

    public void Enter(PlayerStateController controller)
    {
        this.controller = controller;
        anim = controller.GetComponent<PlayerAnimationController>();
        rb = controller.GetComponent<Rigidbody2D>();

        Debug.Log("[State] SkillCasting ���� ����");
    }

    public void Update(PlayerStateController controller)
    {
        // ��ų ���� �߿��� �Է� ������ �⺻
        // ��, �ʿ� �� ���⼭ ���ѵ� �Է� �Ϻ� ��� ����
    }

    public void Exit(PlayerStateController controller)
    {
        Debug.Log("[State] SkillCasting ���� ����");
    }

    public void FixedUpdate()
    {
        // �̵� ���� ����
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public bool CanTransitionTo(PlayerState nextState)
    {
        return nextState == PlayerState.Hit ||
               nextState == PlayerState.Knockback ||
               nextState == PlayerState.Dead ||
               nextState == PlayerState.Guarding;

    }
}
