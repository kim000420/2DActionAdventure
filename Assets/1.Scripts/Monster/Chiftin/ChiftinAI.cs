using UnityEngine;
using Monster.States;
public class ChiftinAI : MonoBehaviour
{
    private IMonsterState currentState;

    public Animator animator;
    public Transform player;



    [Header("행동 범위")]
    public float attackRange = 2f;         // 공격 가능 범위
    public float approachRange = 5f;       // 접근 범위 (이 범위 안까진 따라감)
    public float detectRange = 10f;        // 탐지 범위 (이 범위 넘으면 무시)

    public bool canUseSlidingAttack = true;  // 한 번만 사용 가능
    public int attackCount = 0;

    public bool isGroggy = false;
    public bool isDead = false;

    public bool isAttacking = false;
    public bool isAttackCooldown = false;


    [Header("이동 속도")]
    public float maxMoveSpeed = 3f;
    public float currentMoveSpeed = 0f;
    public float accelerationDuration = 1f; // 1초에 최대 속도 도달

    [Header("슬라이딩 공격 설정")]
    public float slideDistance = 2.5f;
    public float slideDuration = 0.3f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        if (isDead || isGroggy) return; // 그로기 시 아무것도 하지 않음
        currentState?.Execute();
    }
    public void TryAttack()
    {
        if (attackCount >= 3)
        {
            ChangeState(new StrongAttackState(this));
            canUseSlidingAttack = true;
            return;
        }

        bool inAttackRange = Vector3.Distance(transform.position, player.position) <= attackRange;
        bool atMaxSpeed = Mathf.Abs(currentMoveSpeed - maxMoveSpeed) < 0.1f; // ← 수정됨

        if (canUseSlidingAttack && inAttackRange && atMaxSpeed)
        {
            canUseSlidingAttack = false;
            ChangeState(new SlidingAttackState(this));
            return;
        }

        ChangeState(new AttackState(this));
        canUseSlidingAttack = true;
    }
    public void ChangeState(IMonsterState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    public void OnGroggyEnd()
    {
        isGroggy = false;
        ChangeState(new IdleState(this));
    }

    public void OnDeath()
    {
        isDead = true;
        ChangeState(new DieState(this));
    }

    public PlayerDistanceType GetPlayerDistanceType()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectRange)
            return PlayerDistanceType.TooFar;
        else if (distance > attackRange)
            return PlayerDistanceType.OutOfAttack;
        else
            return PlayerDistanceType.InAttackRange;
    }
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // 공격 가능 범위 (빨강)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 접근 가능 범위 (노랑)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, approachRange);

        // 탐지 가능 범위 (파랑)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
    public void FaceToPlayer()
    {
        if (isAttacking) return; // 공격 중엔 방향 전환 X

        float dirX = player.position.x - transform.position.x;

        if (Mathf.Abs(dirX) > 0.01f)
        {
            float faceDir = Mathf.Sign(dirX);
            transform.localScale = new Vector3(-2.5f * faceDir, 2.5f, 1f);
        }
    }
}