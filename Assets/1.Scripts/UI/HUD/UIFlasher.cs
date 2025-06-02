using UnityEngine;
using System.Collections;

public class UIFlasher : MonoBehaviour
{
    [SerializeField] private GameObject normalBackground;
    [SerializeField] private GameObject warningBackground;
    [SerializeField] private float blinkInterval = 0.5f;

    private Coroutine blinkCoroutine;

    public void SetBlinking(bool isBlinking)
    {
        if (isBlinking && blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(Blink());
        }
        else if (!isBlinking && blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            SetBackground(true);
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            SetBackground(true);
            yield return new WaitForSeconds(blinkInterval);
            SetBackground(false);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void SetBackground(bool isNormal)
    {
        normalBackground.SetActive(isNormal);
        warningBackground.SetActive(!isNormal);
    }
}
