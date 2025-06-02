using UnityEngine;

public class PlayerInteractStateController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerStateController stateController;

    private void Awake()
    {
        stateController = GetComponent<PlayerStateController>();
    }

    private void Update()
    {
        if (!stateController.IsControllable()) return;
        if (currentPortal != null && Input.GetKeyDown(KeyCode.D))
        {
            stateController.RequestStateChange(PlayerState.Interacting);
            currentPortal.Interact();
            stateController.RequestStateChange(PlayerState.Idle);
        }
    }
    private PortalTrigger currentPortal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PortalTrigger portal))
        {
            currentPortal = portal;
            Debug.Log("[Portal] 접근 가능");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PortalTrigger portal) && portal == currentPortal)
        {
            currentPortal = null;
            Debug.Log("[Portal] 벗어남");
        }
    }

}
