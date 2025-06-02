using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster.States
{
    public class DieState : IMonsterState
    {
        private ChiftinAI chiftin;

        public DieState(ChiftinAI chiftin) { this.chiftin = chiftin; }

        public void Enter()
        {
            chiftin.animator.Play("Chiftin_Die");
            Object.Destroy(chiftin.gameObject, 3f);
        }

        public void Execute() { }

        public void Exit() { }
    }

}