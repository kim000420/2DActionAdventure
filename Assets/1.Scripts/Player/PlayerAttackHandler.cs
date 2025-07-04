using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private PlayerStateController Controller;
    private PlayerAnimationController anim;

    public GameObject[] comboHitboxes;
    private int comboIndex = 0;

    private void Start()
    {
        Controller = GetComponent<PlayerStateController>();
        anim = GetComponent<PlayerAnimationController>();
    }

    public void PerformComboAttack()
    {
        if (!Controller.StateMachine.CurrentStateInstance.CanTransitionTo(PlayerState.Attacking)) return;

        Controller.RequestStateChange(PlayerState.Attacking);
        string triggerName = $"attack_Combo_{(char)('A' + comboIndex)}";
        anim.PlayTrigger(triggerName);

        EnableHitbox(comboIndex);
        comboIndex = (comboIndex + 1) % comboHitboxes.Length;
    }

    private void EnableHitbox(int index)
    {
        if (comboHitboxes[index] == null) return;
        comboHitboxes[index].SetActive(true);
        Invoke(nameof(DisableHitboxes), 0.2f);
    }

    private void DisableHitboxes()
    {
        foreach (var hitbox in comboHitboxes)
            if (hitbox != null)
                hitbox.SetActive(false);

        Controller.RequestStateChange(PlayerState.Idle);
    }
}