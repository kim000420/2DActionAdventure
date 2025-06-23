[System.Serializable]
public class DialogueData
{
    public DialogueEntry[] lines;
    public string onEndEvent;
}

[System.Serializable]
public class DialogueEntry
{
    public string speaker;
    public string speakerId;
    public string text;
    //말풍선 위치 타겟이름
    public string positionTarget;
    //선택지 관련
    public string[] choices;
    public string[] choiceEvents;
    public string[] branchIds;     // 분기용
    public string branchId;
}
