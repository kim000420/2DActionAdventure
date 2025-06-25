using UnityEngine;

/// <summary>
/// ������ �����ϸ� BossManager�� �ڵ� ��ϵ˴ϴ�.
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
