using UnityEngine;
using TutorialBoss.States;
using TutorialBoss.States.Jo;
using System.Collections;
using TutorialBoss.States.Bow;
using TutorialBoss.States.Dok2;
using TutorialBoss.States.Webuin;
using TutorialBoss.AnimEvents;

namespace TutorialBoss.Controller
{
    public class TutorialBossStateController : MonoBehaviour
    {
        [Header("FSM 상태")]
        public ITutorialBossState currentState;

        [Header("공용 컴포넌트")]
        public Animator animator;
        public Rigidbody2D rb;
        public Transform player;
        public TutorialBossStats bossStats;

        [Header("렌더링")]
        public SpriteRenderer spriteRenderer;
        public float defaultLocalScaleX = -1f;

        [Header("기본 상태")]
        public string bossName = "Jo";
        public bool isDead = false;
        public bool isGroggy = false;
        public bool isHitRecovery;

        [Header("점프 상태")]
        public bool isBowJumping = false;
        public float bowJumpForce = 10f;
        public float bowJumpCooldown = 1f;

        [Header("Dok2 보스 관련")]
        public int attack1Count = 0; // Attack1 사용 횟수 카운터

        [Header("Bow 보스 관련")]
        public Transform bowShootPoint; // Bow 보스가 화살을 발사할 위치
        public float bowEscapeDuration = 2f;

        [Header("공격 쿨타임")]
        public bool isAttackCooldown = false;
        private Coroutine attackCooldownCoroutine;

        [Header("지면 감지")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundCheckRadius = 0.2f;
        private bool wasGrounded;

        public float jumpGracePeriod = 0.2f;
        private float jumpGracePeriodTimer;

        [Header("벽 감지")]
        public Transform wallCheck;
        public LayerMask wallLayer;
        public float wallCheckDistance = 0.5f;
        private Collider2D[] ownColliders;


        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            bossStats = GetComponent<TutorialBossStats>();

            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }

            ownColliders = GetComponentsInChildren<Collider2D>();
        }

        private void Start()
        {
            wasGrounded = IsGroundedInternal();
            jumpGracePeriodTimer = 0f;

            switch (bossName)
            {
                case "Jo":
                    ChangeState(new JoChaseState(this));
                    break;
                case "Bow":
                    ChangeState(new BowEscapeState(this));
                    break;
                case "Dok2":
                    ChangeState(new Dok2ChaseState(this));
                    break;
                case "Webuin":
                    ChangeState(new WebuinChaseState(this));
                    break;
            }
        }

        private void Update()
        {
            if (jumpGracePeriodTimer > 0)
            {
                jumpGracePeriodTimer -= Time.deltaTime;
                if (jumpGracePeriodTimer < 0) jumpGracePeriodTimer = 0;
            }

            bool isCurrentlyGrounded = IsGroundedInternal();
            if (isCurrentlyGrounded && !wasGrounded)
            {
                Debug.Log("[TutorialBossStateController] Boss Landed!");
            }
            wasGrounded = isCurrentlyGrounded;

            currentState?.Execute();
        }

        public void ChangeState(ITutorialBossState newState)
        {
            if (currentState == newState) return;

            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }


        public void FaceAwayFromPlayer()
        {
            if (player == null) return;
            Vector2 dir = player.position - transform.position;
            if (dir.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
            }
            else if (dir.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
            }
        }

        public void FaceToPlayer()
        {
            if (player == null) return;
            Vector2 dir = player.position - transform.position;
            if (dir.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
            }
            else if (dir.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
            }
        }

        public bool IsGrounded()
        {
            if (jumpGracePeriodTimer > 0)
            {
                return false;
            }
            return IsGroundedInternal();
        }

        private bool IsGroundedInternal()
        {
            if (groundCheck == null) return false;
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        public void StartJumpGracePeriod()
        {
            jumpGracePeriodTimer = jumpGracePeriod;
            Debug.Log($"[TutorialBossStateController] Jump Grace Period Started for {jumpGracePeriod:F2} seconds.");
        }

        public bool IsWallAhead()
        {
            if (wallCheck == null) return false;

            float currentFacingDirection = Mathf.Sign(transform.localScale.x);

            RaycastHit2D[] hits = Physics2D.RaycastAll(wallCheck.position, Vector2.right * currentFacingDirection, wallCheckDistance, wallLayer);

            foreach (RaycastHit2D hit in hits)
            {
                bool isOwnCollider = false;
                foreach (Collider2D ownCol in ownColliders)
                {
                    if (hit.collider == ownCol)
                    {
                        isOwnCollider = true;
                        break;
                    }
                }

                if (!isOwnCollider)
                {
                    Debug.Log($"[IsWallAhead] Wall detected! Hit collider: {hit.collider.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                    return true;
                }
            }
            return false;
        }

        public void StartAttackCooldown(float duration)
        {
            if (attackCooldownCoroutine != null)
            {
                StopCoroutine(attackCooldownCoroutine);
            }
            attackCooldownCoroutine = StartCoroutine(AttackCooldownRoutine(duration));
        }

        private IEnumerator AttackCooldownRoutine(float duration)
        {
            isAttackCooldown = true;
            yield return new WaitForSeconds(duration);
            isAttackCooldown = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }

            if (wallCheck != null)
            {
                Gizmos.color = Color.blue;
                float currentFacingDirection = Mathf.Sign(transform.localScale.x);
                Gizmos.DrawRay(wallCheck.position, Vector3.right * currentFacingDirection * wallCheckDistance);
            }

            // ⭐ 변경: bowShootPoint Gizmos 그리기 로직에서 bowShotAngle 대신 실제 발사 방향 예측선 제거
            if (bowShootPoint != null && bossName == "Bow")
            {
                Gizmos.color = Color.magenta;
                // ⭐ 변경: 플레이어 조준 각도 표시
                if (player != null)
                {
                    Vector2 startPosition = bowShootPoint.position;
                    Vector2 targetPosition = player.position; // + Vector2.up * (대략적인 arrowAimHeightOffset);
                    Vector2 shootDirection = (targetPosition - startPosition).normalized;
                    Gizmos.DrawRay(bowShootPoint.position, shootDirection * 2f);
                }
            }
        }
    }
}