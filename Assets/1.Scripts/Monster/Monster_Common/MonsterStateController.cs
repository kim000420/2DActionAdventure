using UnityEngine;
using Monster.CommonStates; // ���� ���� �� Ȱ��ȭ
using Monster.States;       // IMonsterState �� Enum �� �ʿ� �ø� ���

public class MonsterStateController : MonoBehaviour
{
    private IMonsterState currentState;

    [Header("���� �̸� (�ִϸ��̼� ���ξ�)")]
    public string monsterName = "Goblin"; // ��: Goblin �� Goblin_Idle

    [Header("�ʼ� ������Ʈ")]
    public Animator animator;
    public Transform player;
    public Rigidbody2D rb;

    [Header("���� �÷���")]
    public bool isAttacking = false;
    public bool isDead = false;

    [Header("�ൿ ����")]
    public float detectRange = 6f;
    public float attackRange = 2f;

    [Header("�̵� ����")]
    public float moveSpeed = 2f;
    public float accelerationTime = 1f; // ���� �ð�
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
