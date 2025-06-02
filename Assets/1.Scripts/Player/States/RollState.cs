using Player.States;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RollState : IPlayerState
{
    private float rollDuration = 0.9f;
    private float rollTime = 0f;
    private float rollSpeed = 10f;
    private float decelerationTime = 0.5f;
    private bool isDecelerating = false;
    private float direction;
    private int staminaCost = 20;

    public void Enter(PlayerStateController controller)
    {

        var anim = controller.GetComponent<PlayerAnimationController>();
        var input = controller.GetComponent<PlayerInputHandler>();
        var motor = controller.GetComponent<PlayerMotor>();
        var col = controller.GetComponent<CapsuleCollider2D>();
        var stats = controller.GetComponent<PlayerStats>();
        
        //������ ���·� ����
        col.size = new Vector2(0.5f, 0.689999998f);
        col.offset = new Vector2(0, 0.340000004f);

        direction = Mathf.Sign(input.Horizontal);
        if (direction == 0) direction = controller.GetComponent<SpriteRenderer>().flipX ? -1 : 1;

        isDecelerating = false;

        anim.SetBool("isRoll", true);
        rollTime = 0f;
        stats.UseStamina(staminaCost);

        motor.EnableMovementOverride();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        // ���� Ÿ�̸� (0.3�ʰ� ���̾� �浹 ���� �� ����, ���� Ȯ��)
        // ��: controller.StartCoroutine(Invincibility(0.3f));
    }

    public void Update(PlayerStateController controller)
    {
        var motor = controller.GetComponent<PlayerMotor>();
        var anim = controller.GetComponent<PlayerAnimationController>();

        rollTime += Time.deltaTime;

        // ��ü ����: rollDuration = 0.6
        // ���� ������: 0.6 - 0.15 = 0.45�� ��
        if (rollTime < rollDuration - decelerationTime)
        {
            motor.ForceMove(direction * rollSpeed); // ��� ����
        }
        else if (!isDecelerating)
        {
            isDecelerating = true;
            motor.ForceMove(direction * (rollSpeed * 0.3f)); // ���� ����
        }

        if (rollTime >= rollDuration)
        {
            anim.SetBool("isRoll", false);
            controller.RequestStateChange(PlayerState.Idle);
        }
    }


    public void Exit(PlayerStateController controller)
    {
        var motor = controller.GetComponent<PlayerMotor>();
        var col = controller.GetComponent<CapsuleCollider2D>();
        
        // �����·� ����
        col.size = new Vector2(0.5f, 1.29966235f);
        col.offset = new Vector2(0f, 0.649831176f); 

        motor.StopImmediately(); // �ӵ� 0���� �ʱ�ȭ
        motor.DisableMovementOverride();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }
}