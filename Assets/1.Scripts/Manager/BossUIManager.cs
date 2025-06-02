using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    public GameObject bossUIRoot;
    public Image hpBar;
    public Image groggyBar;

    private void Start()
    {
        //bossUIRoot.SetActive(false); // 이미 비활성화 상태이므로 주석처리
    }

    public void ShowUI()
    {
        bossUIRoot.SetActive(true);
        Debug.Log("UI 온");
    }

    public void HideUI()
    {
        bossUIRoot.SetActive(false);
        Debug.Log("UI 오프");
    }

    public void UpdateHP(float ratio)
    {
        hpBar.fillAmount = Mathf.Clamp01(ratio);
    }

    public void UpdateGroggy(float ratio)
    {
        groggyBar.fillAmount = Mathf.Clamp01(ratio);
    }
}
