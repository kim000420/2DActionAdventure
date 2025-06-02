using Player.States;
using UnityEngine;

public class InteractState : IPlayerState
{
    public void Enter(PlayerStateController controller)
    {
        Debug.Log("[State] Enter Interact");
        // 상호작용 애니메이션 트리거 등
        // controller.Animator.SetTrigger("Interact");  (있다면)
    }

    public void Update(PlayerStateController controller)
    {
        // 아무 입력도 받지 않고 상호작용 중일 때 대기
        // 시간이 지난 후 상태 복귀 or 외부 이벤트로 종료
    }

    public void Exit(PlayerStateController controller)
    {
        Debug.Log("[State] Exit Interact");
    }
}
