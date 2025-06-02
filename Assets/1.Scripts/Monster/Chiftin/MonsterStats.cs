using UnityEngine;
namespace Monster.States
{
    public class MonsterStats : MonoBehaviour
    {
        public BossUIManager bossUI;

        [Header("체력")]
        public int maxHP = 100;
        public int currentHP = 100;

        [Header("그로기 게이지")]
        public int maxGroggy = 100;
        public int currentGroggy = 0;

        public bool isDead = false;

        private ChiftinAI chiftinAI;

        private void Awake()
        {
            currentHP = maxHP;
            currentGroggy = maxGroggy;

            chiftinAI = GetComponent<ChiftinAI>();
        }
        private void Start()
        {
            Debug.Log("MonsterStats Start 실행됨");
            bossUI?.ShowUI(); // 전투 시작 시 표시
            UpdateUI();
        }
        private void Update()
        {

        }
        public void ApplyHit(int damage, int groggy, float knockback, Vector2 attackerPos)
        {
            TakeDamage(damage);
            AddGroggy(groggy);
        }

        public void TakeDamage(int amount)
        {
            if (isDead) return;

            currentHP = Mathf.Max(currentHP - amount, 0);
            UpdateUI();
            Debug.Log($"몬스터 피격! 현재 체력: {currentHP}");

            if (currentHP == 0)
            {
                isDead = true;
                chiftinAI.OnDeath();
                bossUI.HideUI();
            }
        }

        public void AddGroggy(int amount)
        {
            UpdateUI();
            if (isDead || chiftinAI.isGroggy) return;

            currentGroggy -= amount;
            currentGroggy = Mathf.Max(currentGroggy, 0);

            if (currentGroggy == 0)
            {
                chiftinAI.isGroggy = true;
                chiftinAI.ChangeState(new GroggyState(chiftinAI));
            }
        }

        public void RecoverGroggy()
        {
            currentGroggy = maxGroggy;
            UpdateUI();
        }

        private void UpdateUI()
        {
            bossUI?.UpdateHP((float)currentHP / maxHP);
            bossUI?.UpdateGroggy((float)currentGroggy / maxGroggy);
        }
    }
}