using UnityEngine;
using Monster.CommonStates; // 상태 구현 후 활성화
using Monster.States;       // IMonsterState 및 Enum 등 필요 시만 사용

public class MonsterStateController : MonoBehaviour
{
    private IMonsterState currentState;

    [Header("몬스터 이름 (애니메이션 접두어)")]
    public string monsterName = "Goblin"; // 예: Goblin → Goblin_Idle

    [Header("필수 컴포넌트")]
    public Animator animator;
    public Transform player;
    public Rigidbody2D rb;

    [Header("상태 플래그")]
    public bool isAttacking = false;
    public bool isDead = false;

    [Header("행동 범위")]
    public float detectRange = 6f;
    public float attackRange = 2f;

    [Header("이동 설정")]
    public float moveSpeed = 2f;
    public float accelerationTime = 1f; // 가속 시간
    [HideInInspector] public float currentMoveSpeed = 0f;

    private void Start()
    {
        ChangeState(new Monster.CommonStates.IdleState(this));
    }

    private void Update()
    {
        if (!isDead)
        {
            currentState?.Execute();
        }
    }

    public void ChangeState(IMonsterState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public PlayerDistanceType GetPlayerDistanceType()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > detectRange)
            return PlayerDistanceType.TooFar;
        else if (dist > attackRange)
            return PlayerDistanceType.OutOfAttack;
        else
            return PlayerDistanceType.InAttackRange;
    }

    public void FaceToPlayer()
    {
        if (isAttacking || isDead) return;

        float dir = player.position.x - transform.position.x;
        if (Mathf.Abs(dir) > 0.01f)
        {
            float faceDir = Mathf.Sign(dir);
            transform.localScale = new Vector3(-1f * faceDir, 1f, 1f);
        }
    }

    public void OnDeath()
    {
        isDead = true;
        ChangeState(new Monster.CommonStates.DieState(this));
    }

    public void OnHit()
    {
        if (!isDead)
            ChangeState(new Monster.CommonStates.HitState(this));
    }

}
