using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button gameExitButton;

    [SerializeField] private string nextScene;

    private Animator animator;

    private void Awake()
    {
        InitButtons();    
        animator = GetComponent<Animator>();
    }


    private void InitButtons()
    {
        gameStartButton.onClick.AddListener(() => animator.SetTrigger("GameStart"));
        gameExitButton.onClick.AddListener(() => Application.Quit());
    }



    public void GameStart() => SceneManager.LoadScene(nextScene);
}
