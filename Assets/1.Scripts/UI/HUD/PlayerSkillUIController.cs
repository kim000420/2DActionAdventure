using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillUIController : MonoBehaviour
{
    [Header("E 스킬 UI 구성")]
    [SerializeField] private TextMeshProUGUI useCountText;

    [Header("쿨다운 오버레이")]
    [SerializeField] private Image kickCooldownOverlay;
    [SerializeField] private Image subCooldownOverlay;
    [SerializeField] private Image foodCooldownOverlay;

    [Header("스킬 참조")]
    [SerializeField] private PlayerSkillController skillController;
    [SerializeField] private PlayerStats playerStats;
    public void Initialize(PlayerStats stats, PlayerSkillController controller)
    {
        playerStats = stats;
        skillController = controller;

        UpdateFoodUseCount();
    }

    private void Start()
    {
        UpdateFoodUseCount(); // 시작 시 초기 표시
    }
    private void Update()
    {
        UpdateCooldownOverlays();
    }

    private void UpdateCooldownOverlays()
    {
        float kickRemain = skillController.GetRemainingCooldown("Kick");
        float kickTotal = skillController.GetSkillCooldown("Kick");
        kickCooldownOverlay.fillAmount = kickRemain / kickTotal;

        float subRemain = skillController.GetRemainingCooldown("Sub");
        float subTotal = skillController.GetSkillCooldown("Sub");
        subCooldownOverlay.fillAmount = subRemain / subTotal;

        float FoodRemain = skillController.GetRemainingCooldown("Food");
        float FoodTotal = skillController.GetSkillCooldown("Food");
        foodCooldownOverlay.fillAmount = FoodRemain / FoodTotal;
    }
    public void UpdateFoodUseCount()
    {
        int count = playerStats.foodItem.useCount;
        useCountText.text = $"x{count}";

        // 0일 경우 텍스트를 흐리게
        if (count <= 0)
        {
            useCountText.color = new Color(1f, 1f, 1f, 0.3f); // 흐리게 처리
        }
        else
        {
            useCountText.color = Color.white;
        }
    }
}
