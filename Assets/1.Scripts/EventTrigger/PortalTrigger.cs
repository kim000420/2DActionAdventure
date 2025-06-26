using UnityEngine;
public enum PortalType
{
    AutoEnter,    // 충돌 즉시 이동
    InteractEnter // D키 등 상호작용 시 이동
}
public class PortalTrigger : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetSpawnPointName;


    [Header("Portal Type")]
    [SerializeField] private PortalType portalType = PortalType.InteractEnter;

    private bool isPlayerInside = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (portalType == PortalType.AutoEnter && other.CompareTag("Player"))
        {
            TryTeleport();
        }
        else if (portalType == PortalType.InteractEnter && other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (portalType == PortalType.InteractEnter && other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
    public void Interact(PlayerStateController controller)
    {
        controller.RequestStateChange(PlayerState.Interacting);

        if (!string.IsNullOrEmpty(targetSceneName) && !string.IsNullOrEmpty(targetSpawnPointName))
        {
            Debug.Log($"[PortalTrigger] 이동: {targetSceneName}, {targetSpawnPointName}");
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnPointName);
        }
        else
        {
            Debug.LogWarning("[PortalTrigger] 설정값이 비어 있어 이동할 수 없습니다.");
        }

        controller.RequestStateChange(PlayerState.Idle); // 바로 복귀
    }
    private void TryTeleport()
    {
        Debug.Log("[PortalTrigger] 자동 포탈 발동됨");
        SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnPointName);
    }
}
