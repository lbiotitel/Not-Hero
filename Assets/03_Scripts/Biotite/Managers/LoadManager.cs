using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EScene
{ 
    Title,
    BootStrap,
    CharacterSpawn,
    F0,
    F1,
    Last
}

public class LoadManager : Singleton<LoadManager>
{
    [SerializeField] private CanvasGroup loadingUI;
    [SerializeField] private TMP_Text loadingProgressText;
    [SerializeField] private float animDuration;

    public void LoadScene(EScene scene, Action action = null)
    {
        StartCoroutine(CoLoadScene(scene.ToString(), action));
    }

    private IEnumerator CoLoadScene(string sceneName, Action action = null)
    {
        loadingUI.alpha = 1f;
        loadingUI.gameObject.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            loadingProgressText.text = $"로드 중 {op.progress * 100f:F0}%";
            yield return null;
        }

        loadingProgressText.text = "로드 완료! 100%";

        yield return new WaitForSeconds(1f);

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        action?.Invoke();
        FadeOut(); 
    }


    private void FadeOut()
    {
        loadingUI.alpha = 1;
        loadingUI.DOFade(0f, animDuration / 2f)
            .OnComplete(() => { loadingUI.gameObject.SetActive(false); });
    }

}
