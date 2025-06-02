using UnityEngine;
using TMPro;

public class StatsUIController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private TextMeshProUGUI textMaxHP;
    [SerializeField] private TextMeshProUGUI textMaxStamina;
    [SerializeField] private TextMeshProUGUI textAttackPower;
    [SerializeField] private TextMeshProUGUI textGroggyPower;
    [SerializeField] private TextMeshProUGUI textCritChance;
    [SerializeField] private TextMeshProUGUI textCritMultiplier;
    [SerializeField] private TextMeshProUGUI textCooldownReduction;
    [SerializeField] private TextMeshProUGUI textKickLevel;
    public void SetStats(PlayerStats stats)
    {
        Debug.Log("[StatsUI] SetStats »£√‚µ ");
        playerStats = stats;

        if (isActiveAndEnabled)
            UpdateUI();
    }

    private void OnEnable()
    {
        Debug.Log("[StatsUI] OnEnable »£√‚µ ");

        if (playerStats != null)
            UpdateUI();
    }


    private void UpdateUI()
    {
        textMaxHP.text = $"{playerStats.maxHP}";
        textMaxStamina.text = $"{playerStats.maxStamina}";
        textAttackPower.text = $"{playerStats.attackPower}";
        textGroggyPower.text = $"{playerStats.groggyPower}";
        textCritChance.text = $"{playerStats.criticalChance * 100f}%";
        textCritMultiplier.text = $"{playerStats.criticalMultiplier}";
        textCooldownReduction.text = $"{playerStats.skillCooldownReduction * 100f}%";
        textKickLevel.text = $"{playerStats.kickLevel}";
    }
}
