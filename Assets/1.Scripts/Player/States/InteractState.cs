using Player.States;
using UnityEngine;

public class InteractState : IPlayerState
{
    public void Enter(PlayerStateController controller)
    {
        Debug.Log("[State] Enter Interact");
        // ��ȣ�ۿ� �ִϸ��̼� Ʈ���� ��
        // controller.Animator.SetTrigger("Interact");  (�ִٸ�)
    }

    public void Update(PlayerStateController controller)
    {
        // �ƹ� �Էµ� ���� �ʰ� ��ȣ�ۿ� ���� �� ���
        // �ð��� ���� �� ���� ���� or �ܺ� �̺�Ʈ�� ����
    }

    public void Exit(PlayerStateController controller)
    {
        Debug.Log("[State] Exit Interact");
    }
}
