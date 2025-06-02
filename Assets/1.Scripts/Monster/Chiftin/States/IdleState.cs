using UnityEngine;
namespace Monster.States
{
    public class IdleState : IMonsterState
    {
        private ChiftinAI chiftin;

        public IdleState(ChiftinAI chiftin)
        {
            this.chiftin = chiftin;
        }

        public void Enter()
        {
            chiftin.animator.Play("Chiftin_Idle"); // BlendTree 노드로 강제 이동
            chiftin.animator.SetFloat("Blend", 0f); // Idle 애니메이션 재생 유도
        }
        public void Execute()
        {
            chiftin.FaceToPlayer(); //항상 플레이어 방향을 바라봄
            if (chiftin.isAttackCooldown)
                return; // 쿨타임 중엔 아무 것도 하지 않음

            switch (chiftin.GetPlayerDistanceType())
            {
                case PlayerDistanceType.OutOfAttack:
                    chiftin.ChangeState(new ChaseState(chiftin));
                    break;

                case PlayerDistanceType.InAttackRange:
                    chiftin.TryAttack();
                    break;
            }
        }

        public void Exit() { }
    }

}
