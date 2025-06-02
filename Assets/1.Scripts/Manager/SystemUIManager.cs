using UnityEngine;

public class SystemUIManager : MonoBehaviour
{
    [SerializeField] private GameObject systemUI;

    [Header("탭 패널")]
    [SerializeField] private GameObject panelStats;
    [SerializeField] private GameObject panelSkills;
    [SerializeField] private GameObject panelInventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isOpen = systemUI.activeSelf;

            if (!isOpen)
            {
                OpenSystemUI();
            }
            else
            {
                CloseSystemUI();
            }
        }
    }

    public void OpenSystemUI()
    {
        systemUI.SetActive(true);
        ShowStatsPanel(); // 디폴트로 Stats 보여줌
    }

    public void CloseSystemUI()
    {
        systemUI.SetActive(false);
    }

    public void ShowStatsPanel()
    {
        panelStats.SetActive(true);
        panelSkills.SetActive(false);
        panelInventory.SetActive(false);
    }

    public void ShowSkillsPanel()
    {
        panelStats.SetActive(false);
        panelSkills.SetActive(true);
        panelInventory.SetActive(false);
    }

    public void ShowInventoryPanel()
    {
        panelStats.SetActive(false);
        panelSkills.SetActive(false);
        panelInventory.SetActive(true);
    }
}
