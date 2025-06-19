using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel; // 말풍선 배경
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private GameObject choiceGroup; // 선택지 영역 (활성/비활성)
    [SerializeField] private TextMeshProUGUI choiceText1;
    [SerializeField] private TextMeshProUGUI choiceText2;
    private int selectedIndex = 0;
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
        if (isDialogueActive && Input.GetKeyDown(KeyCode.D))
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

        var line = currentLines[currentIndex++];
        nameText.text = $"[{line.speaker}]";
        dialogueText.text = line.text;

        ShowBubbleAt(line.positionTarget, line.text);
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
}
