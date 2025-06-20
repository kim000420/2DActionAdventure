using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel; // 말풍선 배경
    [SerializeField] private TextMeshProUGUI D_nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    [Header("Choice UI")]
    
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private TextMeshProUGUI C_nameText;// 🔸 선택지 전체 Panel
    [SerializeField] private TextMeshProUGUI askText;            // 🔸 질문 텍스트
    [SerializeField] private TextMeshProUGUI choiceTextA;        // 🔸 왼쪽 선택지
    [SerializeField] private TextMeshProUGUI choiceTextB;        // 🔸 오른쪽 선택지

    private int selectedIndex = 0;                               // 🔸 현재 선택 중인 항목 (0 또는 1)

    [Header("Speech Bubble")]
    [SerializeField] private GameObject speechBubbleObject;       // 씬에 존재하는 말풍선 오브젝트
    [SerializeField] private TextMeshProUGUI speechBubbleText;

    public System.Action onDialogueEnd;

    private DialogueEntry[] currentLines;
    public int currentIndex;
    private bool isDialogueActive = false;
    private bool isBusy = false;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        // 🔸 선택지 상태 입력 처리
        if (choicePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedIndex = 0;
                UpdateChoiceVisual();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedIndex = 1;
                UpdateChoiceVisual();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                HandleChoiceSelection();
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(DialogueEntry[] lines, Transform _)
    {
        if (isBusy) return;
        isBusy = true;
        currentLines = lines;
        currentIndex = -1;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        var line = currentLines[currentIndex]; 
        currentIndex++;
        if (line.choices != null && line.choices.Length == 2)
        {
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(true);

            selectedIndex = 0;

            C_nameText.text = $"[{line.speaker}]";  // 선택지에도 화자 표시
            askText.text = line.text;
            choiceTextA.text = line.choices[0];
            choiceTextB.text = line.choices[1];

            UpdateChoiceVisual();
        }
        else
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);

            D_nameText.text = $"[{line.speaker}]";
            dialogueText.text = line.text;

            ShowBubbleAt(line.positionTarget, line.text);
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);

        onDialogueEnd?.Invoke();
        onDialogueEnd = null;

        isBusy = false;
    }

    private void ShowBubbleAt(string targetName, string text)
    {
        GameObject target = GameObject.Find(targetName);
        if (target == null)
        {
            Debug.LogWarning($"[DialogueUI] '{targetName}'을 찾을 수 없습니다.");
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

        if (speechBubbleObject != null)
        {
            speechBubbleObject.transform.position = screenPos;
            speechBubbleObject.SetActive(true);

            if (speechBubbleText != null)
                speechBubbleText.text = text;
        }
    }
    private void UpdateChoiceVisual()
    {
        // 글씨 굵기 + 색상 예시 (선택된 항목 강조)
        choiceTextA.fontStyle = selectedIndex == 0 ? FontStyles.Bold : FontStyles.Normal;
        choiceTextB.fontStyle = selectedIndex == 1 ? FontStyles.Bold : FontStyles.Normal;

        choiceTextA.color = selectedIndex == 0 ? Color.white : Color.gray;
        choiceTextB.color = selectedIndex == 1 ? Color.white : Color.gray;
    }
    private void HandleChoiceSelection()
    {
        Debug.Log($"선택한 항목: {selectedIndex} - {currentLines[currentIndex].choices[selectedIndex]}");
        
        var entry = currentLines[currentIndex];

        // 🔸 선택된 이벤트 ID 실행
        if (entry.choiceEvents != null && selectedIndex < entry.choiceEvents.Length)
        {
            string eventId = entry.choiceEvents[selectedIndex];
            DialogueEventManager.Instance?.Trigger(eventId);
        }

        choicePanel.SetActive(false);
        ShowNextLine(); // 다음 대사로 진행
    }

}
