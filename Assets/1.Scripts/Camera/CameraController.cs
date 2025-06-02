using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("플레이어 추적 설정")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = new Vector2(0f, 2f);
    [SerializeField] private float yFollowThreshold = 1.0f;   // Y축 이동 시작 기준 거리
    [SerializeField] private float minFollowSpeed = 1.0f;     // 가까울 때 보간 속도
    [SerializeField] private float maxFollowSpeed = 5.0f;     // 멀 때 보간 속도
    [SerializeField] private float stopThreshold = 0.05f;     // 거의 같을 때는 움직임 생략

    [Header("스폰 포인트")]
    [SerializeField] private Transform spawnPoint;

    private bool isFollowing = true;
    private bool isWaitingForPlayer = false;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void LateUpdate()
    {
        if (isFollowing && target != null)
        {
            FollowTarget();
        }
        else if (target == null && !isWaitingForPlayer)
        {
            StartCoroutine(WaitForPlayer());
        }
    }

    private void FollowTarget()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position + (Vector3)offset;
        targetPos.z = currentPos.z;

        // X축은 즉시 추적
        float finalX = targetPos.x;

        // Y축은 부드럽게
        float currentY = currentPos.y;
        float targetY = targetPos.y;
        float distanceY = Mathf.Abs(targetY - currentY);

        float finalY = currentY;

        if (distanceY > stopThreshold)
        {
            float t = Mathf.InverseLerp(stopThreshold, yFollowThreshold, distanceY);
            float speed = Mathf.Lerp(minFollowSpeed, maxFollowSpeed, t);
            finalY = Mathf.Lerp(currentY, targetY, speed * Time.deltaTime);
        }

        Vector3 finalPos = new Vector3(finalX, finalY, targetPos.z);

        // 흔들림 적용
        if (CameraEffectManager.Instance != null)
        {
            finalPos += CameraEffectManager.Instance.ShakeOffset;
        }

        transform.position = finalPos;
    }

    private System.Collections.IEnumerator WaitForPlayer()
    {
        isWaitingForPlayer = true;

        // 먼저 스폰 위치로 천천히 이동
        while (target == null)
        {
            Vector3 desiredPosition = spawnPoint.position + (Vector3)offset;
            desiredPosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 2f);

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                SetTarget(playerObj.transform);
                FollowOn();
                break;
            }

            yield return null;
        }

        isWaitingForPlayer = false;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void FollowOff()
    {
        isFollowing = false;
    }

    public void FollowOn()
    {
        isFollowing = true;
    }

}
