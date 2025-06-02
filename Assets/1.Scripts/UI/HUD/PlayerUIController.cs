using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("HP UI")]
    [SerializeField] public Image hpFillImage;
    [SerializeField] private UIFlasher hpFlasher;

    [Header("Stamina UI")]
    [SerializeField] public Image staminaFillImage;
    [SerializeField] private UIFlasher staminaFlasher;

    [Header("Stats Reference")]
    [SerializeField] private PlayerStats playerStats;
    public void Initialize(PlayerStats stats)
    {
        playerStats = stats;

        // 중복 등록 방지
        playerStats.OnHPChanged -= UpdateHPUI;
        playerStats.OnStaminaChanged -= UpdateStaminaUI;

        playerStats.OnHPChanged += UpdateHPUI;
        playerStats.OnStaminaChanged += UpdateStaminaUI;

        // 초기 상태 갱신
        UpdateHPUI(playerStats.currentHP, playerStats.maxHP);
        UpdateStaminaUI(playerStats.currentStamina, playerStats.maxStamina);
    }


    private void Start()
    {
        playerStats.OnHPChanged += UpdateHPUI;
        playerStats.OnStaminaChanged += UpdateStaminaUI;
    }

    private void UpdateHPUI(int current, int max)
    {
        float ratio = (float)current / max;
        hpFillImage.fillAmount = ratio;
        hpFlasher.SetBlinking(ratio <= 0.3f);
    }


    private void UpdateStaminaUI(int current, int max)
    {
        float ratio = (float)current / max;
        staminaFillImage.fillAmount = ratio;
        staminaFlasher.SetBlinking(ratio <= 0.3f);
    }
}
