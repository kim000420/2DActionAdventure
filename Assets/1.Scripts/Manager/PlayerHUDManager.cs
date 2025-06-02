using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("HUD 컴포넌트")]
    [SerializeField] private PlayerUIController hpStaminaUI;
    [SerializeField] private PlayerSkillUIController skillUI;

    public void SetPlayer(PlayerStats playerStats, PlayerSkillController skillController)
    {
        // 체력/스태미너 HUD 초기화
        hpStaminaUI.Initialize(playerStats);

        // 스킬 HUD 초기화
        skillUI.Initialize(playerStats, skillController);
    }
}
