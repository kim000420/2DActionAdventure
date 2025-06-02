using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class SkillPanelUIController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [Header("서브웨폰 버튼")]
    [SerializeField] private List<Button> subWeaponButtons;

    [Header("테두리 이미지")]
    [SerializeField] private List<Image> selectionFrames;

    [Header("음식 정보")]
    [SerializeField] private Image foodIcon;
    [SerializeField] private TMP_Text foodNameText;
    [SerializeField] private TMP_Text healAmountText;
    [SerializeField] private TMP_Text useCountText;
    public void SetStats(PlayerStats stats)
    {
        playerStats = stats;
        UpdateSubWeaponUI();
        UpdateFoodUI();
    }
    private void Start()
    {
        for (int i = 0; i < subWeaponButtons.Count; i++)
        {
            int index = i;
            subWeaponButtons[i].onClick.AddListener(() =>
            {
                playerStats.subWeapon = (SubWeaponType)index;
                UpdateSubWeaponUI();
            });
        }

        // 초기 테두리 상태 반영
        UpdateSubWeaponUI();
        UpdateFoodUI();
    }

    public void UpdateSubWeaponUI()
    {
        for (int i = 0; i < selectionFrames.Count; i++)
        {
            selectionFrames[i].enabled = (i == (int)playerStats.subWeapon);
        }
    }

    public void UpdateFoodUI()
    {
        var food = playerStats.foodItem;
        if (food == null)
        {
            foodIcon.enabled = false;
            foodNameText.text = "-";
            healAmountText.text = "-";
            useCountText.text = "-";
            return;
        }

        foodIcon.enabled = true;
        foodIcon.sprite = GetFoodSprite(food.foodName); // TODO: 이름으로 스프라이트 찾는 함수 구현
        foodNameText.text = food.foodName;
        healAmountText.text = $"회복량: {food.healAmount}";
        useCountText.text = $"사용횟수: {food.useCount}";
    }
    private Sprite GetFoodSprite(string foodName)
    {
        return Resources.Load<Sprite>($"Food_icon/{foodName}");
    }
    public void SetupButtonListeners()
    {
        for (int i = 0; i < subWeaponButtons.Count; i++)
        {
            int index = i;

            // 기존 리스너 제거 (중복 방지)
            subWeaponButtons[i].onClick.RemoveAllListeners();

            // 새로 연결
            subWeaponButtons[i].onClick.AddListener(() =>
            {
                playerStats.subWeapon = (SubWeaponType)index;
                UpdateSubWeaponUI();
            });
        }
    }

}
