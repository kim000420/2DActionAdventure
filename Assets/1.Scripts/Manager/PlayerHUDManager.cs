using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("HUD ������Ʈ")]
    [SerializeField] private PlayerUIController hpStaminaUI;
    [SerializeField] private PlayerSkillUIController skillUI;

    public void SetPlayer(PlayerStats playerStats, PlayerSkillController skillController)
    {
        // ü��/���¹̳� HUD �ʱ�ȭ
        hpStaminaUI.Initialize(playerStats);

        // ��ų HUD �ʱ�ȭ
        skillUI.Initialize(playerStats, skillController);
    }
}
