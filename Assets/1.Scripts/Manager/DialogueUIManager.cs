using UnityEngine;
using TMPro;
using UnityEditor.IMGUI.Controls;
using System.Collections;

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
    public string onEndEventId; // 대화 종료 후 실행할 트리거 ID
    public string activeBranchId = null; // 현재 진행중인 분기 ID

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

        onEndEventId = null;
        activeBranchId = null;

        GameManager.Instance.SetDialogueState(false);
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        currentIndex++;

        Debug.Log($"[Dialogue] ShowNextLine → Index: {currentIndex}, ActiveBranch: {activeBranchId}");

        //  분기 조건에 맞는 다음 라인 찾기
        while (currentIndex < currentLines.Length)
        {
            var line = currentLines[currentIndex];

            Debug.Log($"[Dialogue] Checking line {currentIndex} (branchId: {line.branchId})");

            bool isBranchMatch =
                    string.IsNullOrEmpty(line.branchId) && activeBranchId == null || // 일반 대사
                    (activeBranchId != null && line.branchId == activeBranchId); // 선택지 분기 대사

            if (isBranchMatch)
                break;

            currentIndex++;
        }

        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }


        var entry = currentLines[currentIndex];

        if (entry.choices != null && entry.choices.Length == 2)
        {
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(true);

            selectedIndex = 0;

            C_nameText.text = $"{entry.speaker}";  // 선택지에도 화자 표시
            askText.text = entry.text;
            choiceTextA.text = entry.choices[0];
            choiceTextB.text = entry.choices[1];

            UpdateChoiceVisual();
        }
        else
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);

            D_nameText.text = $"{entry.speaker}";
            dialogueText.text = entry.text;

            ShowBubbleAt(entry.positionTarget, entry.text);
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

        if (!string.IsNullOrEmpty(onEndEventId))
        {
            StartCoroutine(DelayedTrigger(onEndEventId));
        }
        else
        {
            // 이벤트 없이 종료된 경우 여기서 대화 상태 해제
            GameManager.Instance.SetDialogueState(false);
        }
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
        if (currentLines == null || currentIndex >= currentLines.Length)
        {
            Debug.LogWarning("[Dialogue] 잘못된 선택지 처리 시도");
            return;
        }

        var entry = currentLines[currentIndex];

        Debug.Log($"선택한 항목: {selectedIndex} - {entry.choices[selectedIndex]}");

        // 트리거 실행
        if (entry.choiceEvents != null && selectedIndex < entry.choiceEvents.Length)
        {
            string eventId = entry.choiceEvents[selectedIndex];
            DialogueEventManager.Instance?.Trigger(eventId);
        }

        // 분기 지정
        if (entry.branchIds != null && selectedIndex < entry.branchIds.Length)
        {
            activeBranchId = entry.branchIds[selectedIndex];
        }

        choicePanel.SetActive(false);
        StartCoroutine(ContinueAfterChoiceDelay());
    }
    public void SetOnEndEvent(string eventId)
    {
        onEndEventId = eventId;
    }

    private IEnumerator ContinueAfterChoiceDelay()
    {
        yield return new WaitForSeconds(0.25f); // 0.2~0.5s 추천
        ShowNextLine();
    }
    private IEnumerator DelayedTrigger(string eventId)
    {
        yield return null; // 한 프레임 대기
        DialogueEventManager.Instance?.Trigger(eventId);
        GameManager.Instance.SetDialogueState(false);
    }
}
