using System.Collections;
using UnityEngine;
public enum ShakeStrength
{
    Weak,
    Medium,
    Strong
}
public class CameraEffectManager : MonoBehaviour
{
    public static CameraEffectManager Instance { get; private set; }

    private Transform camTransform;
    private Vector3 originalPos;

    private float shakeDuration = 0f;
    private float shakeIntensity = 0.1f;

    private Vector3 shakeOffset = Vector3.zero;
    public Vector3 ShakeOffset => shakeOffset;

    private float targetZoom = 6f;
    private float zoomDuration = 0f;
    private float zoomSpeed = 0f;

    [SerializeField] private CanvasGroup flashGroup; // 흰색 전체 화면 UI 이미지

    public float ZoomSize => targetZoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        camTransform = Camera.main.transform;
        originalPos = camTransform.localPosition;
    }

    private void LateUpdate()
    {
        // 흔들림 계산
        if (shakeDuration > 0)
        {
            shakeOffset = (Vector3)(Random.insideUnitCircle * shakeIntensity);
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
        // 줌 보간
        if (zoomDuration > 0)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
            zoomDuration -= Time.deltaTime;
        }
    }

    public void Shake(ShakeStrength strength)    //카메라 흔들림 호출함수
    {
        switch (strength)
        {
            case ShakeStrength.Weak:
                shakeIntensity = 0.06f;
                shakeDuration = 0.05f;
                break;
            case ShakeStrength.Medium:
                shakeIntensity = 0.12f;
                shakeDuration = 0.1f;
                break;
            case ShakeStrength.Strong:
                shakeIntensity = 0.3f;
                shakeDuration = 0.2f;
                break;
        }
    }
    public void ZoomTo(float newZoom, float duration)
    {
        targetZoom = newZoom;
        zoomDuration = duration;
        zoomSpeed = Mathf.Abs(Camera.main.orthographicSize - targetZoom) / duration;
    }

    private IEnumerator ZoomCoroutine(float targetSize, float duration)
    {
        float startSize = Camera.main.orthographicSize;
        float time = 0f;
        while (time < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        Camera.main.orthographicSize = targetSize;
    }
    public void FlashWhite(float duration = 0.1f)
    {
        StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        flashGroup.gameObject.SetActive(true);
        flashGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            flashGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        flashGroup.alpha = 0f;
        flashGroup.gameObject.SetActive(false); // 꺼서 성능 최적화
    }

}
