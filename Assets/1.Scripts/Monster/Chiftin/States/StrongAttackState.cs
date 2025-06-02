using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Monster.States
{
    public class StrongAttackState : IMonsterState
    {
        private ChiftinAI chiftin;
        private float moveTime = 0.2f;
        private float timer = 0f;
        private Vector3 startPos;
        private Vector3 endPos;

        public StrongAttackState(ChiftinAI chiftin)
        {
            this.chiftin = chiftin;
        }

        public void Enter()
        {
            chiftin.isAttacking = true;
            chiftin.animator.Play("Chiftin_Attack_Strong");

            // 전진 방향 계산
            float dir = Mathf.Sign(chiftin.transform.localScale.x);
            startPos = chiftin.transform.position;
            endPos = startPos + new Vector3(dir * 1.5f, 0f, 0f);
            timer = 0f;

            chiftin.attackCount = 0; // 카운트 초기화
        }

        public void Execute()
        {
            if (timer < moveTime)
            {
                timer += Time.deltaTime;
                chiftin.transform.position = Vector3.Lerp(startPos, endPos, timer / moveTime);
            }
        }

        public void Exit()
        {
            chiftin.isAttacking = false;
        }
    }
}

