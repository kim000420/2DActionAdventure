using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEventManager : MonoBehaviour
{
    public static DialogueEventManager Instance;

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 플래그 확인 
    public bool GetFlag(string key)
    {
        return flags.TryGetValue(key, out bool value) && value;
    }

    // 플래그 설정
    public void SetFlag(string key, bool value)
    {
        flags[key] = value;
    }

    //플래그 설정 변경
    public void ToggleFlag(string key)
    {
        SetFlag(key, !GetFlag(key));
    }
    public void Trigger(string eventId)
    {
        Debug.Log($"[DialogueEvent] Triggered: {eventId}");

        switch (eventId)
        {
            case "ExploreStart":
                var followup = Resources.Load<TextAsset>("Dialogue/explore_followup");
                if (followup != null)
                {
                    DialogueData data = JsonUtility.FromJson<DialogueData>(followup.text);
                    var ui = GameManager.Instance.GetComponent<DialogueUIManager>();
                    ui.StartDialogue(data.lines, GameObject.Find("NPC_Nunna").transform);
                }
                break;

            case "ExploreSkip":
                Debug.Log("🔸 탐험을 건너뜀");
                break;

            case "TeleportToCave":
                SceneTransitionManager.Instance.TransitionToScene("Cave", "Start");
                break;

            default:
                Debug.LogWarning($"[DialogueEvent] Unknown ID: {eventId}");
                break;
        }
    }
}
