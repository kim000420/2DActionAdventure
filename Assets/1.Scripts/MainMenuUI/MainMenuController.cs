using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("[MainMenu] 게임 시작 버튼 선택됨");
        SceneTransitionManager.Instance.TransitionToScene("Home", "StartHome");
    }

    public void ExitGame()
    {
        Debug.Log("[MainMenu] 게임 종료");
        Application.Quit();
    }
}
