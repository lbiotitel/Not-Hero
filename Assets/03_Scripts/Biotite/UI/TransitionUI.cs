using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float delay = 1f;


    public void ShowUI(UnityAction action = null)
    {
        StartCoroutine(CoShowUISequence(action));
    }

    private IEnumerator CoShowUISequence(UnityAction action)
    {
        FadeIn();

        yield return new WaitForSeconds(delay + (duration / 2f));

        action?.Invoke();

        FadeOut();
    }

    public void FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, duration);
    }

    public void FadeOut()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, duration);
    }
}
