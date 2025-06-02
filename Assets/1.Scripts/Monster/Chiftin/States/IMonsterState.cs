using UnityEngine;
namespace Monster.States
{
    public interface IMonsterState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}