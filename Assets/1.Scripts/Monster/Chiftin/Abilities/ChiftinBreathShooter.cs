// Scripts/Monster/Abilities/ChiftinBreathShooter.cs
using UnityEngine;

public class ChiftinBreathShooter : MonoBehaviour
{
    public GameObject breathPrefab;
    public Transform firePoint; // 입 위치 (Transform)

    [Header("타겟 지정 및 시야 설정")]
    public Transform target; // 플레이어 Transform
    public float angleLimit = 60f; // 시야각 (기즈모로 표시)
    public float breathRange = 8f; // 최대 사거리

    [Header("기본 발사 각도 설정")]
    [Tooltip("기본 발사 방향 (도 단위, 왼쪽이 0도)")]
    public float defaultFireAngle = 45f;

    public void FireBreath()
    {
        if (breathPrefab == null || firePoint == null || target == null) return;

        Vector2 dirToTarget = (target.position - firePoint.position);
        float distToTarget = dirToTarget.magnitude;
        Vector2 dirToTargetNorm = dirToTarget.normalized;

        // 👈 치프틴은 기본적으로 왼쪽을 보므로 -transform.right 기준
        Vector2 baseDir = -transform.right * Mathf.Sign(transform.localScale.x);

        float angle = Vector2.Angle(baseDir, dirToTargetNorm);
        bool isWithinAngle = angle <= angleLimit;
        bool isWithinRange = distToTarget <= breathRange;

        Vector2 shootDir = (isWithinAngle && isWithinRange)
            ? dirToTargetNorm
            : Quaternion.Euler(0, 0, defaultFireAngle * Mathf.Sign(transform.localScale.x)) * baseDir;

        var go = Instantiate(breathPrefab, firePoint.position, Quaternion.identity);
        go.GetComponent<BreathProjectile>()?.Init(shootDir);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.red;
        float direction = Mathf.Sign(transform.localScale.x);
        Vector3 forward = -transform.right * direction; // 👈 왼쪽 기준

        // 시야각 시각화
        Gizmos.DrawRay(firePoint.position, Quaternion.Euler(0, 0, angleLimit) * forward * breathRange);
        Gizmos.DrawRay(firePoint.position, Quaternion.Euler(0, 0, -angleLimit) * forward * breathRange);

        // 기본 발사 각도 시각화
        Gizmos.color = Color.cyan;
        Vector3 fallbackDir = Quaternion.Euler(0, 0, defaultFireAngle * direction) * forward;
        Gizmos.DrawRay(firePoint.position, fallbackDir * breathRange);

        // 범위 원 그리기
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.2f);
        Gizmos.DrawWireSphere(firePoint.position, breathRange);
    }
}
