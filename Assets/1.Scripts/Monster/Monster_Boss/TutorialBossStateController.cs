using UnityEngine;
using TutorialBoss.States;
using TutorialBoss.States.Jo;
using System.Collections;
using TutorialBoss.States.Bow;
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
        public TutorialBossStats bossStats; // TutorialBossStats 추가

        [Header("렌더링")]
        public SpriteRenderer spriteRenderer;
        public float defaultLocalScaleX = -1f; // 기본 스프라이트가 왼쪽을 보고 있다면 -1f, 오른쪽이면 1f.

        [Header("기본 상태")]
        public string bossName = "Jo"; // Jo, Bow, Dok2 중 하나로 설정
        public bool isDead = false;
        public bool isGroggy = false;
        public bool isHitRecovery; // 피격 후 일정 시간 동안 비활성

        [Header("점프 상태")]
        public bool isBowJumping = false; // Bow 보스의 점프 중 여부
        public float bowJumpForce = 10f; // Bow 보스 점프 힘
        public float bowJumpCooldown = 1f; // Bow 보스 점프 쿨타임 (또는 착지 후 대기 시간)

        [Header("Bow 보스 관련")]
        public Transform bowShootPoint; // Bow 보스가 화살을 발사할 위치
        public float bowShotAngle = 0f; // Bow 보스 화살 발사 각도 (0은 수평)
        public float bowEscapeDuration = 2f; // Bow 보스의 도망 지속 시간

        public GameObject arrowPrefab; // 화살 프리팹을 유니티 에디터에서 연결
        public float arrowAimHeightOffset = 0.5f; // 플레이어 위치에서 얼마나 위를 조준할지


        [Header("공격 쿨타임")]
        public bool isAttackCooldown = false;
        private Coroutine attackCooldownCoroutine;

        [Header("지면 감지")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundCheckRadius = 0.2f;
        private bool wasGrounded; // 이전 프레임의 지면 상태 저장

        [Header("점프후 지면 감지 유예시간")]
        public float jumpGracePeriod = 0.2f; // 점프 후 groundCheck를 무시할 시간
        private float jumpGracePeriodTimer; // 유예 시간을 카운트할 타이머

        [Header("벽 감지")]
        public Transform wallCheck;
        public LayerMask wallLayer;
        public float wallCheckDistance = 0.5f;
        private Collider2D[] ownColliders;


        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            bossStats = GetComponent<TutorialBossStats>();

            // "Player" 태그를 가진 오브젝트를 찾아 할당
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
            wasGrounded = IsGrounded();
            jumpGracePeriodTimer = 0f;

            switch (bossName)
            {
                case "Jo":
                    ChangeState(new JoChaseState(this));
                    break;
                case "Bow":
                    ChangeState(new BowEscapeState(this));
                    break;
                default:
                    Debug.LogWarning($"[Controller] Unknown boss name: {bossName}.");
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

            bool isCurrentlyGrounded = IsGrounded();
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

        // 플레이어 반대 방향을 바라보도록 Scale 변경
        public void FaceAwayFromPlayer()
        {
            if (player == null) return;
            Vector2 dir = player.position - transform.position;
            if (dir.x > 0) transform.localScale = new Vector3(Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
            else if (dir.x < 0) transform.localScale = new Vector3(-Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
        }
        // 플레이어 방향을 바라보도록 Scale 변경
        public void FaceToPlayer()
        {
            if (player == null) return;
            Vector2 dir = player.position - transform.position;
            if (dir.x > 0) transform.localScale = new Vector3(-Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
            else if (dir.x < 0) transform.localScale = new Vector3(Mathf.Abs(defaultLocalScaleX), transform.localScale.y, transform.localScale.z);
        }

        // 지면 감지
        public bool IsGrounded()
        {
            // 점프 유예 시간 동안에는 항상 false를 반환
            if (jumpGracePeriodTimer > 0)
            {
                // Debug.Log($"[IsGrounded] Grace Period Active! Returning false. Remaining: {jumpGracePeriodTimer:F2}");
                return false;
            }
            return IsGroundedInternal();
        }
        // 실제 물리 감지를 수행하는 내부 함수
        private bool IsGroundedInternal()
        {
            if (groundCheck == null) return false;
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 점프 시 호출될 함수
        public void StartJumpGracePeriod()
        {
            jumpGracePeriodTimer = jumpGracePeriod;
        }
        // 벽 감지
        public bool IsWallAhead()
        {

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
                if (!isOwnCollider) return true;
            }
            return false;
        }

        // 공격 쿨타임 시작
        public void StartAttackCooldown(float cooldownTime)
        {
            if (attackCooldownCoroutine != null) StopCoroutine(attackCooldownCoroutine);
            attackCooldownCoroutine = StartCoroutine(AttackCooldownRoutine(cooldownTime));
        }

        private IEnumerator AttackCooldownRoutine(float cooldownTime)
        {
            isAttackCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            isAttackCooldown = false;
        }

        // 유니티 에디터에서 Gizmos 시각화를 위한 함수
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
                float currentFacingDirection = Mathf.Sign(transform.localScale.x); // 보스가 바라보는 방향
                Gizmos.DrawRay(wallCheck.position, Vector3.right * currentFacingDirection * wallCheckDistance);
            }

            if (bowShootPoint != null)
            {
                Gizmos.color = Color.magenta;
                Vector2 shotDirection = Quaternion.Euler(0, 0, bowShotAngle) * Vector2.right;
                shotDirection.x *= Mathf.Sign(transform.localScale.x);
                Gizmos.DrawRay(bowShootPoint.position, shotDirection.normalized * 2f);
            }
        }
    }
}