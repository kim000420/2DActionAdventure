using UnityEngine;
using TutorialBoss.States;
using TutorialBoss.States.Jo;

namespace TutorialBoss.Controller
{
    public class TutorialBossStateController : MonoBehaviour
    {
        [Header("FSM ����")]
        private ITutorialBossState currentState;

        [Header("���� ������Ʈ")]
        public Animator animator;
        public Rigidbody2D rb;
        public Transform player;

        [Header("������")]
        public SpriteRenderer spriteRenderer;

        [Header("�⺻ ����")]
        public string bossName = "Jo"; // Jo, Bow, Dok2 �� �ϳ��� ����
        public bool isDead = false;
        public bool isGroggy = false;
        //public bool isAttacking = false;

        [Header("���� ��Ÿ��")]
        public bool isAttackCooldown = false;

        [Header("�°��� �����ൿ ��Ÿ��")]
        public bool isHitRecovery; // �ǰ� �� ���� �ð� ���� ��Ȱ��

        [Header("Bow ���� - �̵� �� ȸ��")]
        public float bowJumpForce = 7f; // Bow�� ���� ��
        public float bowJumpCooldown = 2f; // ���� �� ���� ���������� ��Ÿ��
        public bool isBowJumping = false; // Bow�� ���� ���� ������
        public float bowEscapeDistance = 5f; // �÷��̾�κ��� ������ �Ÿ�
        public float bowMinEscapeDistance = 3f; // ���� ���¸� ������ �ּ� �Ÿ�
        public float bowEscapeDuration = 1.5f; // ���� ���� ���� �ð�
                                               // --- Bow ���� ���� �ʵ� �߰� �� ---
        [Header("���� ����")]
        public Transform groundCheck; // ���� ������ ���� Transform (GroundCheck ������Ʈ)
        public LayerMask groundLayer; // �������� �ν��� LayerMask
        public float groundCheckRadius = 0.2f; // GroundCheck ������Ʈ�� ���� �ݰ�

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
                case "Bow": // Bow ���� �ʱ� ���� �߰�
                    //ChangeState(new States.Bow.BowIdleState(this)); // BowIdleState�� �ʱ� ���·� ����
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
        // --- ���ο� ���� �Լ� �߰� (���� ���� Ȯ�ο�) ---
        public bool IsGrounded()
        {
            if (groundCheck == null)
            {
                Debug.LogWarning("[TutorialBossStateController] GroundCheck Transform�� �Ҵ���� �ʾҽ��ϴ�. ���� ������ �Ұ����մϴ�.");
                return true; // �⺻������ true ��ȯ�Ͽ� ������ ���� �ʵ���
            }
            // GroundCheck ������Ʈ ��ġ���� groundLayer�� ����ִ��� OverlapCircle�� Ȯ��
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // ����Ƽ �����Ϳ��� GroundCheckRadius �ð�ȭ�� ���� Gizmos
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
