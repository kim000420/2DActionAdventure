using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    public GameObject bossUIRoot;
    public Image hpBar;
    public Image groggyBar;

    private void Start()
    {
        //bossUIRoot.SetActive(false); // �̹� ��Ȱ��ȭ �����̹Ƿ� �ּ�ó��
    }

    public void ShowUI()
    {
        bossUIRoot.SetActive(true);
        Debug.Log("UI ��");
    }

    public void HideUI()
    {
        bossUIRoot.SetActive(false);
        Debug.Log("UI ����");
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
