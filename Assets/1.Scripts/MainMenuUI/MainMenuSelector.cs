using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuSelector : MonoBehaviour
{
    public RectTransform selectorArrow; // 화살표 이미지
    public RectTransform[] buttons;     // 버튼들 (NewGame, LoadGame, ...)

    private int currentIndex = 0;

    void Start()
    {
        UpdateArrowPosition();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            UpdateArrowPosition();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            UpdateArrowPosition();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ExecuteEvents.Execute(buttons[currentIndex].gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    void UpdateArrowPosition()
    {
        RectTransform target = buttons[currentIndex];

        // 버튼의 월드 좌표 기준: 좌측 끝 = 위치 - (가로길이 * pivot.x) 만큼 왼쪽으로
        Vector3 btnLeftWorldPos = target.position;
        float buttonWidth = target.rect.width;
        float pivotX = target.pivot.x;

        // 좌측 끝 x좌표 계산
        float leftX = btnLeftWorldPos.x - (buttonWidth * pivotX * target.lossyScale.x);

        // 화살표를 버튼 왼쪽으로 약간 띄워 배치 (예: 30px 왼쪽)
        selectorArrow.position = new Vector3(leftX - 50f, btnLeftWorldPos.y, btnLeftWorldPos.z);
    }


    // 마우스 올렸을 때 호출할 함수
    public void HoverSelect(int index)
    {
        currentIndex = index;
        UpdateArrowPosition();
    }
}
