using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] dialogue;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.GetComponentInChildren<DialogueUIManager>().StartDialogue(dialogue);
        }
    }
}
