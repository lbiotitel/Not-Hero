using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour, IEventListener
{
    private BattleManager battleManager;

    [SerializeField] private TMP_Text state;
    [SerializeField] private TMP_Text turn;
    [SerializeField] private Image BG;

    private bool isPlaying = false;

    private Vector2 origenPos;

    private void OnDisable()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnPlayerTurn, this);
            Managers.Event.RemoveListener(EEventType.OnEnemyTurn, this);
        }
    }

    public void Init()
    {
        battleManager = Managers.Battle;
        Managers.Event.AddListener(EEventType.OnPlayerTurn, this);
        Managers.Event.AddListener(EEventType.OnEnemyTurn, this);
        state.gameObject.SetActive(false);
        turn.gameObject.SetActive(false);
        BG.gameObject.SetActive(false);

        origenPos = state.transform.position;
    }

    private void Show()
    {
        state.gameObject.SetActive(true);
        BG.gameObject.SetActive(true);
    }
    private void Hide()
    {
        state.gameObject.SetActive(false);
        turn.gameObject.SetActive(false);
        BG.gameObject.SetActive(false);
    }

    public void OnEvent(EEventType type, object sender, object param)
    {
        if (isPlaying) return;

        isPlaying = true;

        if (type == EEventType.OnPlayerTurn)
        {
            Managers.Sound.PlaySound(Defines.SoundKey.TURN_SFX, false, ESoundType.Sfx);
            state.text = "나의 턴";
            turn.text = battleManager.Turn.ToString() + "턴 째";
            state.gameObject.SetActive(true);
            turn.gameObject.SetActive(true);
            BG.gameObject.SetActive(true);
            state.transform.position = origenPos;
        }
        if (type == EEventType.OnEnemyTurn)
        {
            Managers.Sound.PlaySound(Defines.SoundKey.TURN_SFX, false, ESoundType.Sfx);
            state.text = "적의 턴";
            state.gameObject.SetActive(true);
            BG.gameObject.SetActive(true);
            state.transform.position = origenPos + new Vector2(0, -50);
        }

        DOTweenMotion(type);
    }

    public void DOTweenMotion(EEventType eEvent)
    {
        // 초기 상태
        state.alpha = 0;
        turn.alpha = 0;

        if (eEvent == EEventType.OnEnemyTurn)
        {
            state.gameObject.SetActive(true);
            turn.gameObject.SetActive(false);
            BG.gameObject.SetActive(true);
        }
        else
        {
            state.gameObject.SetActive(true);
            turn.gameObject.SetActive(true);
            BG.gameObject.SetActive(true);
        }

        // 색 설정
        if (eEvent == EEventType.OnEnemyTurn)
            state.color = Color.red;
        else
            state.color = Color.green;

        Sequence seq = DOTween.Sequence();

        // 등장 (Fade In)
        seq.Append(state.DOFade(1f, 0.5f));
        seq.Join(turn.DOFade(1f, 0.5f));

        // 유지
        seq.AppendInterval(0.3f);

        // 사라짐 (Fade Out)
        seq.Append(state.DOFade(0f, 0.5f));
        seq.Join(turn.DOFade(0f, 0.5f));

        // 끝나고 비활성화
        seq.OnComplete(() =>
        {
            Hide();
            isPlaying = false;
        });
    }

    public void Clear()
    {
        StopAllCoroutines();

        DOTween.Kill(state);
        DOTween.Kill(turn);

        state.gameObject.SetActive(false);
        turn.gameObject.SetActive(false);
        BG.gameObject.SetActive(false);
        state.alpha = 1f;
        turn.alpha = 1f;
        state.transform.position = origenPos;

        isPlaying = false;
    }
}
