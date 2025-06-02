using System.Collections;
using UnityEngine;
using Monster.States;
public class ChiftinAnimatorEvents : MonoBehaviour
{
    public ChiftinAI chiftin;
    
    [Header("히트박스")]
    public GameObject hitbox_Attack1;
    public GameObject hitbox_Strong;
    public GameObject hitbox_Sliding;

    public void EnableHitbox_Attack1() => hitbox_Attack1.SetActive(true);
    public void DisableHitbox_Attack1() => hitbox_Attack1.SetActive(false);

    public void EnableHitbox_Strong() => hitbox_Strong.SetActive(true);
    public void DisableHitbox_Strong() => hitbox_Strong.SetActive(false);

    public void EnableHitbox_Sliding() => hitbox_Sliding.SetActive(true);
    public void DisableHitbox_Sliding() => hitbox_Sliding.SetActive(false);
    
    public void OnAttackEnd()
    {
        if (!chiftin.isDead && !chiftin.isGroggy)
        {
            chiftin.StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        chiftin.isAttackCooldown = true;
        chiftin.ChangeState(new IdleState(chiftin)); // 즉시 Idle로 전환
        yield return new WaitForSeconds(0.8f);
        chiftin.isAttackCooldown = false;
    }
    public void OnGroggyAnimationEnd()
    {
        chiftin.GetComponent<MonsterStats>()?.RecoverGroggy(); // 게이지 초기화
        chiftin.OnGroggyEnd(); // FSM 전환
    }

    public void OnDie()
    {
        chiftin.OnDeath();
    }
}
