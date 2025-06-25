using UnityEngine;
using System.Collections.Generic;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    private Dictionary<string, GameObject> bossDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterBoss(string id, GameObject boss)
    {
        if (!bossDict.ContainsKey(id))
        {
            bossDict[id] = boss;
            Debug.Log($"[BossManager] 보스 '{id}' 등록됨.");
        }
    }

    public void UnregisterBoss(string id)
    {
        if (bossDict.ContainsKey(id))
        {
            bossDict.Remove(id);
            Debug.Log($"[BossManager] 보스 '{id}' 해제됨.");
        }
    }

    public void SetBossActive(string id, bool active)
    {
        if (bossDict.TryGetValue(id, out GameObject boss) && boss != null)
        {
            boss.SetActive(active);
            Debug.Log($"[BossManager] 보스 '{id}' {(active ? "활성화" : "비활성화")}됨.");
        }
        else
        {
            Debug.LogWarning($"[BossManager] '{id}' 보스를 찾지 못했습니다.");
        }
    }

    public void ClearBosses()
    {
        bossDict.Clear();
        Debug.Log("[BossManager] 보스 목록 초기화됨.");
    }
}
