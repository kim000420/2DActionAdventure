using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuSelector : MonoBehaviour
{
    public RectTransform selectorArrow; // ȭ��ǥ �̹���
    public RectTransform[] buttons;     // ��ư�� (NewGame, LoadGame, ...)

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

        // ��ư�� ���� ��ǥ ����: ���� �� = ��ġ - (���α��� * pivot.x) ��ŭ ��������
        Vector3 btnLeftWorldPos = target.position;
        float buttonWidth = target.rect.width;
        float pivotX = target.pivot.x;

        // ���� �� x��ǥ ���
        float leftX = btnLeftWorldPos.x - (buttonWidth * pivotX * target.lossyScale.x);

        // ȭ��ǥ�� ��ư �������� �ణ ��� ��ġ (��: 30px ����)
        selectorArrow.position = new Vector3(leftX - 50f, btnLeftWorldPos.y, btnLeftWorldPos.z);
    }


    // ���콺 �÷��� �� ȣ���� �Լ�
    public void HoverSelect(int index)
    {
        currentIndex = index;
        UpdateArrowPosition();
    }
}
