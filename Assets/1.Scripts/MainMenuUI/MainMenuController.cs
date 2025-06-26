using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("[MainMenu] ���� ���� ��ư ���õ�");
        SceneTransitionManager.Instance.TransitionToScene("Home", "StartHome");
    }

    public void ExitGame()
    {
        Debug.Log("[MainMenu] ���� ����");
        Application.Quit();
    }
}
