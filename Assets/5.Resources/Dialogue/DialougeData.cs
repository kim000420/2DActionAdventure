[System.Serializable]
public class DialogueData
{
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueEntry
{
    public string speaker;
    public string speakerId;
    public string text;
    public string positionTarget;
}
