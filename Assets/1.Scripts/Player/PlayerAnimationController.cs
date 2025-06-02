using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerMotor motor;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        motor = GetComponent<PlayerMotor>();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("speed", speed);
    }

    public void SetJump(bool value)
    {
        animator.SetBool("isJump", value);
    }

    public void SetVerticalVelocity(float y)
    {
        animator.SetFloat("verticalVelocity", y);
    }

    public void SetBool(string param, bool value)
    {
        animator.SetBool(param, value);
    }

    public void PlayTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void UpdateDirection(float horizontal)
    {
        if (horizontal != 0)
            spriteRenderer.flipX = horizontal < 0;
    }

    public void UpdateLocomotionBlend()
    {
        float moveSpeed = Mathf.Abs(motor.GetHorizontalVelocity());
        float blendValue = Mathf.InverseLerp(0f, motor.moveSpeed, moveSpeed);
        SetSpeed(blendValue);
    }

    public void UpdateJumpParameters()
    {
        SetVerticalVelocity(motor.GetVerticalVelocity());
    }
    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
}