using UnityEngine;
using static ChiftinAI;
namespace Monster.States
{
    public class ChaseState : IMonsterState
    {
        private ChiftinAI chiftin;
        private float accelerationPerSecond;

        public ChaseState(ChiftinAI chiftin)
        {
            this.chiftin = chiftin;
        }

        public void Enter()
        {
            accelerationPerSecond = chiftin.maxMoveSpeed / chiftin.accelerationDuration;
        }

        public void Execute()
        {
            chiftin.FaceToPlayer(); //항상 플레이어 방향을 바라봄
            switch (chiftin.GetPlayerDistanceType())
            {
                case PlayerDistanceType.TooFar:
                    chiftin.ChangeState(new IdleState(chiftin));
                    break;

                case PlayerDistanceType.InAttackRange:
                    chiftin.TryAttack();
                    return;
            }
            if (!chiftin.IsPlayerInRange(chiftin.detectRange))
            {
                chiftin.ChangeState(new IdleState(chiftin));
                return;
            }

            float distance = Vector3.Distance(chiftin.transform.position, chiftin.player.position);
            if (distance <= chiftin.attackRange)
            {
                chiftin.TryAttack();
                return;
            }

            // ✅ 가속 적용
            chiftin.currentMoveSpeed = Mathf.Min(
                chiftin.currentMoveSpeed + accelerationPerSecond * Time.deltaTime,
                chiftin.maxMoveSpeed
            );

            Vector3 dir = (chiftin.player.position - chiftin.transform.position).normalized;
            chiftin.transform.position += dir * chiftin.currentMoveSpeed * Time.deltaTime;

            // ✅ 방향 설정
            if (dir.x != 0)
            {
                chiftin.transform.localScale = new Vector3(-2.5f * Mathf.Sign(dir.x), 2.5f, 1f);
            }

            // ✅ Blend 파라미터는 이동속도 비율로 설정
            float blendValue = chiftin.currentMoveSpeed / chiftin.maxMoveSpeed; // 0~1
            chiftin.animator.SetFloat("Blend", blendValue);
        }

        public void Exit()
        {
            // 다음 상태에서 다시 가속하도록 초기화
            chiftin.currentMoveSpeed = 0f;
        }
    }
}