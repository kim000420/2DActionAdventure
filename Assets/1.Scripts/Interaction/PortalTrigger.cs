using UnityEngine;
public class PortalTrigger : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetSpawnPointName;

    public void Interact()
    {
        if (!string.IsNullOrEmpty(targetSceneName) && !string.IsNullOrEmpty(targetSpawnPointName))
        {
            Debug.Log($"[PortalTrigger] �̵�: {targetSceneName}, {targetSpawnPointName}");
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnPointName);
        }
        else
        {
            Debug.LogWarning("[PortalTrigger] �������� ��� �־� �̵��� �� �����ϴ�.");
        }
    }
}
