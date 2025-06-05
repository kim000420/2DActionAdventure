using UnityEngine;
using System.Collections;
using CommonMonster.Stats;      // CommonMonsterStats ����
using CommonMonster.States;     // IMonsterState, BaseMonsterState ����
using CommonMonster.States.Common; // ���� ���� ���� (Hit, Groggy, Die)
using CommonMonster.States.Groundfish;
using CommonMonster.States.Forg;
//using CommonMonster.States.Lizardman;

namespace CommonMonster.Controller
{
    public class CommonMonsterController : MonoBehaviour
    {
        [Header("FSM ����")]
        public IMonsterState currentState; // ���� ���� ���� �������̽�
        
        [Header("���� �̸�")]
        public string monsterName = "DefaultMonster";

        [Header("���� ������Ʈ")]
        public Animator animator;
        public Rigidbody2D rb;
        public SpriteRenderer spriteRenderer;
        public Transform player; // �÷��̾� Transform (Tag "Player"�� ã�� ����)
        public CommonMonsterStats monsterStats; // CommonMonsterStats ����

        [Header("������ ����")]
        [Tooltip("�⺻ ��������Ʈ�� ������ ���� -1f, �������̸� 1f.")]
        public float defaultLocalScaleX = -1f;

        [Header("�⺻ ���� �÷���")]
        public bool isDead = false;
        public bool isGroggy = false;
        public bool isHitRecovery = false; // �ǰ� �� ���� ������ ����

        [Header("�ൿ ���� �÷���")]
        // ���� ��Ÿ���� AnimatorEvents���� �����ϹǷ�, ���⼭�� �÷��׸� ����
        public bool isAttackCooldown = false;
        private Coroutine attackCooldownCoroutine; // ���� ��Ÿ�� �ڷ�ƾ ����

        [Header("���� �� �� ����")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundCheckRadius = 0.2f;
        public bool isJumping = false;
        public Transform wallCheck;
        public LayerMask wallLayer;
        public float wallCheckDistance = 0.5f;

        [Header("Forg ���� ����")] // Forg ���� �߰�
        public Transform forgShootPoint; // Forg�� ����ü�� �߻��� ��ġ
        public GameObject forgProjectilePrefab; // Forg ����ü ������
        [Tooltip("Forg ����ü �߻� �ӵ�")]
        public float forgProjectileSpeed = 10f;

        private void Awake()
        {
            // ������Ʈ ���� �ʱ�ȭ
            if (animator == null) animator = GetComponent<Animator>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (monsterStats == null) monsterStats = GetComponent<CommonMonsterStats>();

            // �÷��̾� Transform ã�� (���� ���� �� �� ����)
            // �±װ� "Player"�� ������Ʈ�� ã��
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.transform;
            }

            // ���� �ʱ�ȭ (Awake���� �ϴ� ���� �Ϲ���)
            if (monsterStats != null)
            {
                monsterStats.currentHp = monsterStats.maxHp;
                monsterStats.currentGroggy = 0;
            }
            
            // ���� �̸� �ʱ�ȭ
            if (string.IsNullOrEmpty(monsterName) || monsterName == "DefaultMonster")
            {
                monsterName = gameObject.name;
            }
        }

        private void Start()
        {
            switch (monsterName)
            {
                case "Groundfish":
                    ChangeState(new GroundfishIdleState(this));
                    break;
                case "Lizardman":
                    break;
                case "Forg":
                    ChangeState(new ForgIdleState(this));
                    break;

            }

        }

        private void Update()
        {
            // �׾��ų� �׷α� ���̰ų� �ǰ� ���� ���� ���� ���� ������ �������� ����
            if (isDead || isGroggy || isHitRecovery)
            {
                // Ư�� ���¿����� �ִϸ��̼��� ����ǵ��� �����ϰ�, �̵��� ����
                if (rb != null) rb.velocity = Vector2.zero;
                return;
            }

            currentState?.Execute(); // ���� ������ Execute �޼��� ȣ��
        }

        // FSM ���� ���� �޼���
        public void ChangeState(IMonsterState newState)
        {
            if (currentState != null)
            {
                currentState.Exit(); // ���� ���� ����
            }
            currentState = newState; // �� ���� ����
            currentState.Enter(); // �� ���� ����
        }

        // �÷��̾� �������� ���� ��������Ʈ ������
        public void FaceToPlayer()
        {
            if (player == null) return;

            float direction = player.position.x - transform.position.x;
            if (Mathf.Abs(direction) > 0.01f) // ���� ���� ���� ����
            {
                if (direction > 0) // �÷��̾ �����ʿ� �ִٸ�
                {
                    transform.localScale = new Vector3(defaultLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
                else // �÷��̾ ���ʿ� �ִٸ�
                {
                    transform.localScale = new Vector3(-defaultLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
            }
        }

        // ���� ��Ÿ�� ���� �ڷ�ƾ
        public void StartAttackCooldown(float cooldownTime)
        {
            if (attackCooldownCoroutine != null) StopCoroutine(attackCooldownCoroutine);
            attackCooldownCoroutine = StartCoroutine(AttackCooldownRoutine(cooldownTime));
        }

        private IEnumerator AttackCooldownRoutine(float cooldownTime)
        {

            Debug.Log($"���� ��Ÿ�� ����");
            isAttackCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            Debug.Log($"���� ��Ÿ�� ����");
            isAttackCooldown = false;
        }

        // ���� ����
        public bool IsGrounded()
        {
            if (groundCheck == null || groundLayer == 0)
            {
                return true; // ������ ���� �⺻������ ���鿡 �ִٰ� ����
            }
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // �� ���� (���Ͱ� �ٶ󺸴� ��������)
        public bool IsWallAhead(float directionX) // directionX: -1 (����) �Ǵ� 1 (������)
        {
            if (wallCheck == null || wallLayer == 0)
            {
                return false;
            }
            // ������ ���� �̵� ����� Sprite ������ ��ġ��Ų �� ����ؾ� ��
            // directionX�� ���� transform.localScale.x�� ��ȣ�� �Ѱܹ���
            RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * directionX, wallCheckDistance, wallLayer);

            // �ڽ��� �ݶ��̴��� �����ϵ��� �߰� �˻� �ʿ� (��: LayerMask ���� �Ǵ� Tag/GetComponent<>() �˻�)
            return hit.collider != null && hit.collider.gameObject != gameObject;
        }

        // ����Ƽ �����Ϳ��� Gizmos �ð�ȭ�� ���� �Լ�
        private void OnDrawGizmosSelected()
        {
            if (monsterStats == null) return;

            // �ν� ���� Gizmos
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, monsterStats.detectionRange);

            // ���� ���� Gizmos (���� Ÿ�Կ� ���� ���Ǻ�)
            if (monsterStats.monsterType == MonsterType.MeleeOnly || monsterStats.monsterType == MonsterType.Hybrid)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, monsterStats.meleeAttackRange);
            }
            if (monsterStats.monsterType == MonsterType.RangedOnly || monsterStats.monsterType == MonsterType.Hybrid)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, monsterStats.rangedAttackRange);
            }

            // ���� ���� Gizmos
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }

            // �� ���� Gizmos
            if (wallCheck != null)
            {
                Gizmos.color = Color.blue;
                // ���� ������ localScale.x �������� ���� �׸���
                float currentFacingDirection = Mathf.Sign(transform.localScale.x);
                Gizmos.DrawRay(wallCheck.position, Vector3.right * currentFacingDirection * wallCheckDistance);
            }
            // Forg ����ü �߻� ���� Gizmos (Forg�� ��츸)
            if (forgShootPoint != null && monsterName == "Forg") // "Forg" ���� �̸����� Ȯ��
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(forgShootPoint.position, 0.1f); // ���� ���� �߻� ���� ǥ��

                // �÷��̾� ���ؼ� Gizmos
                if (player != null)
                {
                    Vector2 startPosition = forgShootPoint.position;
                    Vector2 targetPosition = player.position;
                    Vector2 shootDirection = (targetPosition - startPosition).normalized;
                    Gizmos.DrawRay(forgShootPoint.position, shootDirection * monsterStats.rangedAttackRange);
                }
            }
        }
    }
}