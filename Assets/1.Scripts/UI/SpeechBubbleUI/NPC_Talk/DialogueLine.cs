[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;

    public DialogueLine(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}
