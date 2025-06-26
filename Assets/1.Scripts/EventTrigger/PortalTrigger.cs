using UnityEngine;
public enum PortalType
{
    AutoEnter,    // �浹 ��� �̵�
    InteractEnter // DŰ �� ��ȣ�ۿ� �� �̵�
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
            Debug.Log($"[PortalTrigger] �̵�: {targetSceneName}, {targetSpawnPointName}");
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnPointName);
        }
        else
        {
            Debug.LogWarning("[PortalTrigger] �������� ��� �־� �̵��� �� �����ϴ�.");
        }

        controller.RequestStateChange(PlayerState.Idle); // �ٷ� ����
    }
    private void TryTeleport()
    {
        Debug.Log("[PortalTrigger] �ڵ� ��Ż �ߵ���");
        SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnPointName);
    }
}
