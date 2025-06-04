using CommonMonster.Controller;
using UnityEngine;
using CommonMonster.States.Common;

namespace CommonMonster.Stats
{
    // 몬스터 타입 열거형 정의
    public enum MonsterType
    {
        MeleeOnly,    // 근거리 공격만 가능한 몬스터
        RangedOnly,   // 원거리 공격만 가능한 몬스터 (투사체 발사)
        Hybrid,       // 근거리/원거리 모두 가능한 몬스터
    }

    public class CommonMonsterStats : MonoBehaviour
    {
        [Header("몬스터 기본 정보")]
        [Tooltip("이 몬스터의 공격 및 행동 특성을 정의합니다.")]
        public MonsterType monsterType = MonsterType.MeleeOnly; // 에디터에서 몬스터 타입 선택

        [Header("생존 및 전투 스탯")]
        [Tooltip("몬스터의 최대 체력.")]
        public float maxHp = 100f;
        [HideInInspector] // 인스펙터에서 직접 수정하지 않고 코드에서 관리
        public float currentHp; // 현재 체력

        [Tooltip("몬스터의 기본 공격력.")]
        public float attackDamage = 10f;

        [Tooltip("몬스터의 이동 속도.")]
        public float moveSpeed = 3f;

        [Tooltip("몬스터가 넉백되는 정도를 조절합니다. 1f는 기본 넉백, 0.5f는 절반 넉백.")]
        public float knockbackResistance = 1f;

        [Header("그로기 스탯")]
        [Tooltip("몬스터가 그로기 상태에 빠지기 위해 필요한 최대 그로기 게이지.")]
        public float maxGroggy = 10f;
        [HideInInspector] // 인스펙터에서 직접 수정하지 않고 코드에서 관리
        public float currentGroggy; // 현재 그로기 게이지 (피격 시 증가)

        [Tooltip("그로기 상태가 지속되는 시간.")]
        public float groggyDuration = 3f;

        [Header("행동 범위 설정 (Gizmos 포함)")]
        [Tooltip("플레이어를 인식하고 추적을 시작하는 최대 거리.")]
        public float detectionRange = 7f;

        [Tooltip("근거리 공격이 가능한 최대 거리. MeleeOnly 및 Hybrid 타입에만 유효합니다.")]
        public float meleeAttackRange = 1.5f;

        [Tooltip("원거리 공격(투사체 발사)이 가능한 최대 거리. RangedOnly 및 Hybrid 타입에만 유효합니다.")]
        public float rangedAttackRange = 6f;

        // [Header("점프 스탯 (JumperOnly, Hybrid 타입 관련)")] // 필요 시 헤더 추가
        [Tooltip("점프 공격 또는 회피 점프 시 적용될 힘.")]
        public float jumpForce = 8f;
        [Tooltip("점프 후 다음 점프가 가능해질 때까지의 쿨타임.")]
        public float jumpCooldown = 1f;


        private CommonMonsterController controller;
        private void Awake()
        {
            currentHp = maxHp; // 게임 시작 시 현재 체력을 최대 체력으로 초기화
            currentGroggy = 0f; // 그로기 게이지 초기화
        }

        public void ApplyHit(float damage, float groggyAmount, float knockbackForce, Vector2 attackerPosition)
        {
            if (controller == null)
            {
                return;
            }
            if (controller.isDead) return; // 이미 죽었다면 더 이상 피해를 받지 않음

            currentHp -= damage;

            // 체력 체크: 사망 상태 전이
            if (currentHp <= 0)
            {
                currentHp = 0; // 체력은 0 이하로 내려가지 않도록
                // controller.ChangeState(new DieState(controller));
                return; // 사망했으면 그로기/피격 로직은 수행하지 않음
            }

            // 그로기 게이지 증가
            currentGroggy -= groggyAmount;

            // 그로기 게이지 체크: 그로기 상태 전이 (사망 상태가 아니라면)
            if (currentGroggy <= 0 && !controller.isGroggy)
            {
                // controller.ChangeState(new GroggyState(controller));
                currentGroggy = maxGroggy; // 그로기 진입 시 게이지 초기화
                return; // 그로기 상태로 전이되었으면 피격 상태로 전이하지 않음
            }

            // 피격 상태 전이 (사망도 아니고 그로기도 아니라면)
            if (!controller.isHitRecovery) // 이미 피격 경직 중이 아니라면
            {
                // 넉백 저항을 적용한 최종 넉백 힘 계산
                float finalKnockbackForce = knockbackForce * knockbackResistance;

                controller.ChangeState(new HitState(
                    controller,
                    attackerPosition,
                    finalKnockbackForce
                ));
            }
        }
        // 유니티 에디터에서 선택 시 Gizmos 시각화
        private void OnDrawGizmosSelected()
        {
            // 몬스터 위치 기준으로 Gizmos 그리기
            Vector3 center = transform.position;

            // 인식 범위 (모든 몬스터 타입)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, detectionRange);

            // 근거리 공격 범위 (MeleeOnly, Hybrid 타입만)
            if (monsterType == MonsterType.MeleeOnly || monsterType == MonsterType.Hybrid)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(center, meleeAttackRange);
            }

            // 원거리 공격 범위 (RangedOnly, Hybrid 타입만)
            if (monsterType == MonsterType.RangedOnly || monsterType == MonsterType.Hybrid)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(center, rangedAttackRange);
            }
        }
    }
}