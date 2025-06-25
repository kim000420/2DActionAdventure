using UnityEngine;

/// <summary>
/// 보스가 등장하면 BossManager에 자동 등록됩니다.
/// </summary>
public class BossComponent : MonoBehaviour
{
    [SerializeField] private string bossId = "Jo";

    public string GetBossId() => bossId;

    private void OnEnable()
    {
        BossManager.Instance?.RegisterBoss(bossId, this.gameObject);
    }

    private void OnDisable()
    {
        BossManager.Instance?.UnregisterBoss(bossId);
    }
}
