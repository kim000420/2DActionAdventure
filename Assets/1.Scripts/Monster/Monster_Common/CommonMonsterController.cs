using UnityEngine;
using System.Collections;
using CommonMonster.Stats;      // CommonMonsterStats 참조
using CommonMonster.States;     // IMonsterState, BaseMonsterState 참조
using CommonMonster.States.Common; // 공통 상태 참조 (Hit, Groggy, Die)
using CommonMonster.States.Groundfish;
using CommonMonster.States.Forg;
//using CommonMonster.States.Lizardman;

namespace CommonMonster.Controller
{
    public class CommonMonsterController : MonoBehaviour
    {
        [Header("FSM 상태")]
        public IMonsterState currentState; // 현재 몬스터 상태 인터페이스
        
        [Header("몬스터 이름")]
        public string monsterName = "DefaultMonster";

        [Header("공용 컴포넌트")]
        public Animator animator;
        public Rigidbody2D rb;
        public SpriteRenderer spriteRenderer;
        public Transform player; // 플레이어 Transform (Tag "Player"로 찾을 예정)
        public CommonMonsterStats monsterStats; // CommonMonsterStats 참조

        [Header("렌더링 설정")]
        [Tooltip("기본 스프라이트가 왼쪽을 보면 -1f, 오른쪽이면 1f.")]
        public float defaultLocalScaleX = -1f;

        [Header("기본 상태 플래그")]
        public bool isDead = false;
        public bool isGroggy = false;
        public bool isHitRecovery = false; // 피격 후 경직 중인지 여부

        [Header("행동 제어 플래그")]
        // 공격 쿨타임은 AnimatorEvents에서 관리하므로, 여기서는 플래그만 가짐
        public bool isAttackCooldown = false;
        private Coroutine attackCooldownCoroutine; // 공격 쿨타임 코루틴 참조

        [Header("지면 및 벽 감지")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundCheckRadius = 0.2f;
        public bool isJumping = false;
        public Transform wallCheck;
        public LayerMask wallLayer;
        public float wallCheckDistance = 0.5f;

        [Header("Forg 보스 관련")] // Forg 관련 추가
        public Transform forgShootPoint; // Forg가 투사체를 발사할 위치
        public GameObject forgProjectilePrefab; // Forg 투사체 프리팹
        [Tooltip("Forg 투사체 발사 속도")]
        public float forgProjectileSpeed = 10f;

        private void Awake()
        {
            // 컴포넌트 참조 초기화
            if (animator == null) animator = GetComponent<Animator>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (monsterStats == null) monsterStats = GetComponent<CommonMonsterStats>();

            // 플레이어 Transform 찾기 (게임 시작 시 한 번만)
            // 태그가 "Player"인 오브젝트를 찾음
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.transform;
            }

            // 스탯 초기화 (Awake에서 하는 것이 일반적)
            if (monsterStats != null)
            {
                monsterStats.currentHp = monsterStats.maxHp;
                monsterStats.currentGroggy = 0;
            }
            
            // 몬스터 이름 초기화
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
            // 죽었거나 그로기 중이거나 피격 경직 중일 때는 상태 로직을 실행하지 않음
            if (isDead || isGroggy || isHitRecovery)
            {
                // 특정 상태에서는 애니메이션이 재생되도록 유지하고, 이동만 멈춤
                if (rb != null) rb.velocity = Vector2.zero;
                return;
            }

            currentState?.Execute(); // 현재 상태의 Execute 메서드 호출
        }

        // FSM 상태 변경 메서드
        public void ChangeState(IMonsterState newState)
        {
            if (currentState != null)
            {
                currentState.Exit(); // 이전 상태 종료
            }
            currentState = newState; // 새 상태 설정
            currentState.Enter(); // 새 상태 진입
        }

        // 플레이어 방향으로 몬스터 스프라이트 뒤집기
        public void FaceToPlayer()
        {
            if (player == null) return;

            float direction = player.position.x - transform.position.x;
            if (Mathf.Abs(direction) > 0.01f) // 아주 작은 값은 무시
            {
                if (direction > 0) // 플레이어가 오른쪽에 있다면
                {
                    transform.localScale = new Vector3(defaultLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
                else // 플레이어가 왼쪽에 있다면
                {
                    transform.localScale = new Vector3(-defaultLocalScaleX, transform.localScale.y, transform.localScale.z);
                }
            }
        }

        // 공격 쿨타임 시작 코루틴
        public void StartAttackCooldown(float cooldownTime)
        {
            if (attackCooldownCoroutine != null) StopCoroutine(attackCooldownCoroutine);
            attackCooldownCoroutine = StartCoroutine(AttackCooldownRoutine(cooldownTime));
        }

        private IEnumerator AttackCooldownRoutine(float cooldownTime)
        {

            Debug.Log($"공격 쿨타임 시작");
            isAttackCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            Debug.Log($"공격 쿨타임 종료");
            isAttackCooldown = false;
        }

        // 지면 감지
        public bool IsGrounded()
        {
            if (groundCheck == null || groundLayer == 0)
            {
                return true; // 안전을 위해 기본적으로 지면에 있다고 가정
            }
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 벽 감지 (몬스터가 바라보는 방향으로)
        public bool IsWallAhead(float directionX) // directionX: -1 (왼쪽) 또는 1 (오른쪽)
        {
            if (wallCheck == null || wallLayer == 0)
            {
                return false;
            }
            // 몬스터의 실제 이동 방향과 Sprite 방향을 일치시킨 후 사용해야 함
            // directionX는 보통 transform.localScale.x의 부호를 넘겨받음
            RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * directionX, wallCheckDistance, wallLayer);

            // 자신의 콜라이더는 무시하도록 추가 검사 필요 (예: LayerMask 설정 또는 Tag/GetComponent<>() 검사)
            return hit.collider != null && hit.collider.gameObject != gameObject;
        }

        // 유니티 에디터에서 Gizmos 시각화를 위한 함수
        private void OnDrawGizmosSelected()
        {
            if (monsterStats == null) return;

            // 인식 범위 Gizmos
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, monsterStats.detectionRange);

            // 공격 범위 Gizmos (몬스터 타입에 따라 조건부)
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

            // 지면 감지 Gizmos
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }

            // 벽 감지 Gizmos
            if (wallCheck != null)
            {
                Gizmos.color = Color.blue;
                // 현재 몬스터의 localScale.x 방향으로 레이 그리기
                float currentFacingDirection = Mathf.Sign(transform.localScale.x);
                Gizmos.DrawRay(wallCheck.position, Vector3.right * currentFacingDirection * wallCheckDistance);
            }
            // Forg 투사체 발사 지점 Gizmos (Forg일 경우만)
            if (forgShootPoint != null && monsterName == "Forg") // "Forg" 몬스터 이름으로 확인
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(forgShootPoint.position, 0.1f); // 작은 구로 발사 지점 표시

                // 플레이어 조준선 Gizmos
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