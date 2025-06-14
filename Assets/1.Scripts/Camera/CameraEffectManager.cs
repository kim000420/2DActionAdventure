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
        // ��鸲 ���
        if (shakeDuration > 0)
        {
            shakeOffset = (Vector3)(Random.insideUnitCircle * shakeIntensity);
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
        // �� ����
        if (zoomDuration > 0)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
            zoomDuration -= Time.deltaTime;
        }
    }

    public void Shake(ShakeStrength strength)    //ī�޶� ��鸲 ȣ���Լ�
    {
        switch (strength)
        {
            case ShakeStrength.Weak:
                shakeIntensity = 0.1f;
                shakeDuration = 0.1f;
                break;
            case ShakeStrength.Medium:
                shakeIntensity = 0.3f;
                shakeDuration = 0.2f;
                break;
            case ShakeStrength.Strong:
                shakeIntensity = 0.6f;
                shakeDuration = 0.3f;
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

}
