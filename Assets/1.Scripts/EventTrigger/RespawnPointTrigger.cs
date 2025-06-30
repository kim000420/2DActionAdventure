using UnityEngine;

public class RespawnPointTrigger : MonoBehaviour
{
    [Header("������ ������ ����")]
    [SerializeField] private string RespawnSceneName;
    [SerializeField] private string RespawnId;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneTransitionManager.Instance.SetRespawnLocation(RespawnSceneName, RespawnId);
    }
}
