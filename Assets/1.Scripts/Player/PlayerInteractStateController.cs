using UnityEngine;

public class PlayerInteractStateController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerStateController stateController;
    private IInteractable currentInteractable;

    private void Awake()
    {
        stateController = GetComponent<PlayerStateController>();
    }

    private void Update()
    {
        if (!stateController.IsControllable()) return;

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.D))
        {
            currentInteractable.Interact(stateController);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
            Debug.Log("[Interact] 접근 가능");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("[Interact] 벗어남");
        }
    }
}
