using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private PlayerStateController stateController;
    private PlayerAnimationController anim;

    public GameObject[] comboHitboxes;
    private int comboIndex = 0;

    private void Start()
    {
        stateController = GetComponent<PlayerStateController>();
        anim = GetComponent<PlayerAnimationController>();
    }

    public void PerformComboAttack()
    {
        if (!stateController.CanTransitionTo(PlayerState.Attacking)) return;

        stateController.ChangeState(PlayerState.Attacking);
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

        stateController.ChangeState(PlayerState.Idle);
    }
}