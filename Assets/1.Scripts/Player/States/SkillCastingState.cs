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
        if (controller.TryGetComponent(out PlayerMotor motor))
        {
            motor.StopImmediately(); //이동 값 초기화
        }
        Debug.Log("[State] SkillCasting 상태 진입");
    }

    public void Update(PlayerStateController controller)
    {
        // 스킬 시전 중에는 입력 차단이 기본
        // 단, 필요 시 여기서 제한된 입력 일부 허용 가능
    }

    public void Exit(PlayerStateController controller)
    {
        Debug.Log("[State] SkillCasting 상태 종료");
    }

    public void FixedUpdate()
    {
        // 이동 완전 차단
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public bool CanTransitionTo(PlayerState nextState)
    {
        return nextState == PlayerState.Hit ||
               nextState == PlayerState.Knockback ||
               nextState == PlayerState.Dead ||
               nextState == PlayerState.Idle ||
               nextState == PlayerState.Guarding;

    }
}
