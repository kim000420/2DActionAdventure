using UnityEngine;
namespace Monster.States
{
    public class GroggyState : IMonsterState
    {
        private ChiftinAI chiftin;
        private Rigidbody2D rb;

        public GroggyState(ChiftinAI chiftin)
        {
            this.chiftin = chiftin;
            this.rb = chiftin.GetComponent<Rigidbody2D>();
        }

        public void Enter()
        {
            chiftin.animator.Play("Chiftin_Groggy");

            if (rb != null)
            {
                rb.velocity = Vector2.zero; // 즉시 멈춤
            }
        }

        public void Execute()
        {
            // 아무것도 하지 않음
        }

        public void Exit()
        {
            // 필요 시 이펙트 제거 등
        }
    }
}