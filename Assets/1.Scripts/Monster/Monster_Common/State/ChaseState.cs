using UnityEngine;
using Monster.States; // IMonsterState 사용

namespace Monster.CommonStates
{
    public class ChaseState : IMonsterState
    {
        private MonsterStateController controller;
        private float accelerationPerSecond;

        public ChaseState(MonsterStateController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            accelerationPerSecond = controller.moveSpeed / controller.accelerationTime;
        }

        public void Execute()
        {
            controller.FaceToPlayer();

            switch (controller.GetPlayerDistanceType())
            {
                case PlayerDistanceType.TooFar:
                    controller.ChangeState(new IdleState(controller));
                    return;

                case PlayerDistanceType.InAttackRange:
                    // controller.ChangeState(new AttackState(controller)); // 구현 후 주석 해제
                    return;
            }

            // 가속 이동
            controller.currentMoveSpeed = Mathf.Min(
                controller.currentMoveSpeed + accelerationPerSecond * Time.deltaTime,
                controller.moveSpeed
            );

            Vector3 dir = (controller.player.position - controller.transform.position).normalized;
            controller.transform.position += dir * controller.currentMoveSpeed * Time.deltaTime;

            // 애니메이션 전환
            controller.animator.Play($"{controller.monsterName}_Run");

            // 방향 반영 (이미 FaceToPlayer에도 있지만 이중 확인)
            if (dir.x != 0)
            {
                float faceDir = Mathf.Sign(dir.x);
                controller.transform.localScale = new Vector3(-1f * faceDir, 1f, 1f);
            }

            // Blend 파라미터 적용 (이동 비율 기반)
            float blend = controller.currentMoveSpeed / controller.moveSpeed;
            controller.animator.SetFloat("Blend", blend);
        }

        public void Exit()
        {
            controller.currentMoveSpeed = 0f;
        }
    }
}
