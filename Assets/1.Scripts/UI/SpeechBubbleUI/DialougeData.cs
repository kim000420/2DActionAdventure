[System.Serializable]
public class DialogueData
{
    public DialogueEntry[] lines;
}

[System.Serializable]
public class DialogueEntry
{
    public string speaker;
    public string speakerId;
    public string text;
    public string positionTarget;
    public string[] choices;
    public string[] choiceEvents;
}
