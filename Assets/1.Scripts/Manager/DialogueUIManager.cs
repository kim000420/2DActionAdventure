using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public System.Action onDialogueEnd;

    private DialogueLine[] currentLines;
    private int currentIndex = 0;

    private bool isDialogueActive = false;

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.D))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        currentLines = lines;
        currentIndex = 0;
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

        nameText.text = currentLines[currentIndex].speaker;
        dialogueText.text = currentLines[currentIndex].text;
        currentIndex++;
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);


        // 대화 종료 후 콜백 호출
        onDialogueEnd?.Invoke();
        onDialogueEnd = null; // 호출 후 초기화 (메모리 안전성)
    }
}
