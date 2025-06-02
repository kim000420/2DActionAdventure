namespace Player.States
{
    public interface IPlayerState
    {
        void Enter(PlayerStateController controller);
        void Update(PlayerStateController controller);
        void Exit(PlayerStateController controller);
    }
}