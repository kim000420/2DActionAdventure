using UnityEngine;
namespace Monster.States
{
    public class AttackState : IMonsterState
    {
        private ChiftinAI chiftin;

        public AttackState(ChiftinAI chiftin)
        {
            this.chiftin = chiftin;
        }

        public void Enter()
        {
            chiftin.isAttacking = true;
            chiftin.attackCount++;

            float roll = Random.value; // 0.0f ~ 1.0f

            if (roll < 0.66f)
            {
                chiftin.animator.Play("Chiftin_Attack1");
            }
            else
            {
                chiftin.animator.Play("Chiftin_Attack_Breath");
            }
        }

        public void Execute()
        {
            chiftin.FaceToPlayer(); //항상 플레이어 방향을 바라봄
        }

        public void Exit()
        {
            chiftin.isAttacking = false;
        }
    }

}