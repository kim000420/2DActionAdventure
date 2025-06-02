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
            Debug.Log($"[PortalTrigger] 이동: {targetSceneName}, {targetSpawnPointName}");
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnPointName);
        }
        else
        {
            Debug.LogWarning("[PortalTrigger] 설정값이 비어 있어 이동할 수 없습니다.");
        }
    }
}
