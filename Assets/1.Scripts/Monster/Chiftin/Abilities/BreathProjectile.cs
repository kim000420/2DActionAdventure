using UnityEngine;

public class BreathProjectile : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float decelerationTime = 1.2f;
    public float slowSpeedThreshold = 0.05f;

    public Vector3 maxScale = new Vector3(2f, 2f, 1f);
    public float scaleDuration = 0.3f;

    public float riseAmount = 1f;
    public float riseDuration = 0.3f;

    private Vector2 direction;
    private float currentSpeed;
    private float slowStartTime;
    private bool slowingDown = false;

    private Vector3 initialScale;
    private float scaleElapsed = 0f;

    private bool rising = false;
    private float riseStartTime;
    private Vector3 riseStartPos;
    private Vector3 riseTargetPos;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        currentSpeed = moveSpeed;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // 크기 확장
        if (scaleElapsed < scaleDuration)
        {
            scaleElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(scaleElapsed / scaleDuration);
            transform.localScale = Vector3.Lerp(initialScale, maxScale, t);
        }

        // 상승
        if (rising)
        {
            float t = (Time.time - riseStartTime) / riseDuration;
            transform.position = Vector3.Lerp(riseStartPos, riseTargetPos, t);
            if (t >= 1f) rising = false;
        }

        // 감속 이동
        if (slowingDown)
        {
            float t = (Time.time - slowStartTime) / decelerationTime;
            currentSpeed = Mathf.SmoothStep(moveSpeed, 0f, t);

            if (t >= 1f)
            {
                currentSpeed = 0f;
                slowingDown = false;
            }
        }

        if (currentSpeed > slowSpeedThreshold)
            transform.Translate(direction * currentSpeed * Time.deltaTime);
    }

    public void StartSlowDown()
    {
        slowingDown = true;
        slowStartTime = Time.time;

        // 부드럽게 상승
        rising = true;
        riseStartTime = Time.time;
        riseStartPos = transform.position;
        riseTargetPos = riseStartPos + new Vector3(0f, riseAmount, 0f);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
