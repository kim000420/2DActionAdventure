using UnityEngine;
using System.Collections;

public class AutoBGMStartTrigger : MonoBehaviour
{
    [SerializeField] private SoundID bgmToPlay;

    private void Start()
    {
        StartCoroutine(DelayedBGMStart());
    }

    private IEnumerator DelayedBGMStart()
    {
        yield return new WaitForSeconds(0.1f); // 1프레임 딜레이
        if (SoundManager.Instance == null) yield break;

        if (SoundManager.Instance.CurrentBGM == bgmToPlay)
            yield break;

        SoundManager.Instance.PlayBGM(bgmToPlay);
    }
}
