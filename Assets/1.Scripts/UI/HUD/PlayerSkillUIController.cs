using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillUIController : MonoBehaviour
{
    [Header("E ��ų UI ����")]
    [SerializeField] private TextMeshProUGUI useCountText;

    [Header("��ٿ� ��������")]
    [SerializeField] private Image kickCooldownOverlay;
    [SerializeField] private Image subCooldownOverlay;
    [SerializeField] private Image foodCooldownOverlay;

    [Header("��ų ����")]
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
        UpdateFoodUseCount(); // ���� �� �ʱ� ǥ��
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

        // 0�� ��� �ؽ�Ʈ�� �帮��
        if (count <= 0)
        {
            useCountText.color = new Color(1f, 1f, 1f, 0.3f); // �帮�� ó��
        }
        else
        {
            useCountText.color = Color.white;
        }
    }
}
