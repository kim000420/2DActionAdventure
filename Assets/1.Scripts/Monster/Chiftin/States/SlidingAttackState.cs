using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Monster.States
{
    public class SlidingAttackState : IMonsterState
    {
        private ChiftinAI chiftin;
        private float timer = 0f;
        private Vector3 startPos;
        private Vector3 endPos;

        public SlidingAttackState(ChiftinAI chiftin)
        {
            this.chiftin = chiftin;
        }

        public void Enter()
        {
            chiftin.isAttacking = true;

            float direction = -Mathf.Sign(chiftin.transform.localScale.x);
            startPos = chiftin.transform.position;
            endPos = startPos + new Vector3(direction * chiftin.slideDistance, 0f, 0f);

            chiftin.animator.Play("Chiftin_Attack_Sliding");
        }

        public void Execute()
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / chiftin.slideDuration);

            // 커브 기반 전진 처리
            float curveT = chiftin.slideCurve.Evaluate(t);
            chiftin.transform.position = Vector3.Lerp(startPos, endPos, curveT);
        }

        public void Exit()
        {
            chiftin.isAttacking = false;
        }
    }
}




