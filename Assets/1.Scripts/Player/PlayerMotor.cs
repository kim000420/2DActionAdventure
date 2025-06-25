using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("이동")]
    public float moveSpeed = 8f; //최고 속도
    public float acceleration = 20f; //이동 딜레이
    public float deceleration = 30f; //이동 멈춤 딜레이
    public float turnDeceleration = 40f; //이동중 반대방향 딜레이

    [Header("점프")]
    public float jumpPower = 12f;
    private float jumpLockTimer = 0f; //점프 지연 타이머

    private float currentVelocityX = 0f;
    private float inputX = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool wasGrounded = false;

    private bool isOverridingMovement = false;
    public float LastDirection { get; private set; } = 1f;

    private PlayerStateController stateController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        wasGrounded = IsGrounded();
    }
    public void Move(float direction)
    {
        var stateController = GetComponent<PlayerStateController>();

        // ✅ 이동할 때만 방향 갱신
        if (direction != 0)
            LastDirection = Mathf.Sign(direction);

        if (stateController != null && stateController.CurrentState == PlayerState.Attacking)
        {
            inputX = 0f;
            return;  // ❌ 방향 갱신 안 함
        }

        inputX = direction;

    }

    private void Update()
    {
        bool isGrounded = IsGrounded();

        if (jumpLockTimer > 0f)
            jumpLockTimer -= Time.deltaTime;

        if (!wasGrounded && isGrounded)
        {
            var controller = GetComponent<PlayerStateController>();
            if (controller != null && controller.CurrentState == PlayerState.Knockback)
            {
                var events = GetComponent<PlayerAnimationEvents>();
                if (events != null)
                    events.OnLanded(); // Knockback 상태에서만 트리거 실행
            }
        }
        wasGrounded = isGrounded;

    }

    private void FixedUpdate()
    {
        if (isOverridingMovement)
        {
            return; // ✅ Roll 중이면 기존 움직임 무시
        }    
        
        // ✅ Interact 상태도 이동 중단
        var stateController = GetComponent<PlayerStateController>();
        if (stateController != null && stateController.CurrentState == PlayerState.Interacting)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // 수평 이동 중단
            currentVelocityX = 0f;
            return;
        }

        float targetSpeed = inputX * moveSpeed;

        if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentVelocityX) && Mathf.Abs(currentVelocityX) > 0.01f && Mathf.Abs(inputX) > 0.01f)
        {
            currentVelocityX = Mathf.MoveTowards(currentVelocityX, 0, turnDeceleration * Time.fixedDeltaTime);
        }
        else if (Mathf.Abs(inputX) > 0.01f)
        {
            currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocityX = Mathf.MoveTowards(currentVelocityX, 0, deceleration * Time.fixedDeltaTime);
        }

        Vector2 velocity = rb.velocity;
        velocity.x = currentVelocityX;
        rb.velocity = velocity;
    }

    public void Jump()
    {
        Vector2 velocity = rb.velocity;
        velocity.y = jumpPower;
        rb.velocity = velocity;
        jumpLockTimer = 1f; // 1초 동안 착지 판정 무시
    }

    public bool IsGrounded()
    {
        if (jumpLockTimer > 0f) return false; // 잠금 중이면 false
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    public float GetHorizontalVelocity() => rb.velocity.x;
    public float GetVerticalVelocity() => rb.velocity.y;

    public void ForceMove(float speed)
    {
        Vector2 velocity = rb.velocity;
        velocity.x = speed;
        rb.velocity = velocity;
    }

    public void StopImmediately()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = 0f;
        rb.velocity = velocity;
        currentVelocityX = 0f; // 이동 가속 상태 초기화
        inputX = 0f;
    }

    public void ForceMove(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
    public void EnableMovementOverride() => isOverridingMovement = true;
    public void DisableMovementOverride() => isOverridingMovement = false; 
    public void ForwardImpulse(float distance, float duration)
    {
        StartCoroutine(ForwardImpulseRoutine(distance, duration));
    }

    private System.Collections.IEnumerator ForwardImpulseRoutine(float distance, float duration)
    {
        float time = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.right * distance * LastDirection;

        EnableMovementOverride();

        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        DisableMovementOverride();
    }
}
