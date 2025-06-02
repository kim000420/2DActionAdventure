using UnityEngine;
using TutorialBoss.States;
using TutorialBoss.States.Jo;

namespace TutorialBoss.Controller
{
    public class TutorialBossStateController : MonoBehaviour
    {
        [Header("FSM 상태")]
        private ITutorialBossState currentState;

        [Header("공용 컴포넌트")]
        public Animator animator;
        public Rigidbody2D rb;
        public Transform player;

        [Header("렌더링")]
        public SpriteRenderer spriteRenderer;

        [Header("기본 상태")]
        public string bossName = "Jo"; // Jo, Bow, Dok2 중 하나로 설정
        public bool isDead = false;
        public bool isGroggy = false;
        //public bool isAttacking = false;

        [Header("공격 쿨타임")]
        public bool isAttackCooldown = false;

        [Header("맞고나서 다음행동 쿨타임")]
        public bool isHitRecovery; // 피격 후 일정 시간 동안 비활성

        [Header("Bow 보스 - 이동 및 회피")]
        public float bowJumpForce = 7f; // Bow의 점프 힘
        public float bowJumpCooldown = 2f; // 점프 후 다음 점프까지의 쿨타임
        public bool isBowJumping = false; // Bow가 현재 점프 중인지
        public float bowEscapeDistance = 5f; // 플레이어로부터 도망갈 거리
        public float bowMinEscapeDistance = 3f; // 도망 상태를 유지할 최소 거리
        public float bowEscapeDuration = 1.5f; // 도망 상태 지속 시간
                                               // --- Bow 보스 관련 필드 추가 끝 ---
        [Header("지면 감지")]
        public Transform groundCheck; // 지면 감지를 위한 Transform (GroundCheck 오브젝트)
        public LayerMask groundLayer; // 지면으로 인식할 LayerMask
        public float groundCheckRadius = 0.2f; // GroundCheck 오브젝트의 감지 반경

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            switch (bossName)
            {
                case "Jo":
                    ChangeState(new JoChaseState(this));
                    break;
                case "Bow": // Bow 보스 초기 상태 추가
                    //ChangeState(new States.Bow.BowIdleState(this)); // BowIdleState를 초기 상태로 설정
                    break;
            }

        }

        private void Update()
        {
            currentState?.Execute();
        }

        public void ChangeState(ITutorialBossState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        // --- 새로운 헬퍼 함수 추가 (점프 상태 확인용) ---
        public bool IsGrounded()
        {
            if (groundCheck == null)
            {
                Debug.LogWarning("[TutorialBossStateController] GroundCheck Transform이 할당되지 않았습니다. 지면 감지가 불가능합니다.");
                return true; // 기본적으로 true 반환하여 점프를 막지 않도록
            }
            // GroundCheck 오브젝트 위치에서 groundLayer와 닿아있는지 OverlapCircle로 확인
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 유니티 에디터에서 GroundCheckRadius 시각화를 위한 Gizmos
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
