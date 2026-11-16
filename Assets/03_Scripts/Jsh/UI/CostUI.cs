using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostUI : MonoBehaviour, IEventListener
{
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Transform costImageTransform;
    [SerializeField] private ParticleSystem costRegenParticle;

    private void OnDestroy()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnEnergyCostChanged, this);
            Managers.Event.RemoveListener(EEventType.OnPlayerTurn, this);
            Managers.Event.RemoveListener(EEventType.OnBattleStart, this);
            Managers.Event.RemoveListener(EEventType.OnBattleWin, this);
            Managers.Event.RemoveListener(EEventType.BattleLose, this);
        }
    }

    public void Init()
    {
        Managers.Event.AddListener(EEventType.OnEnergyCostChanged, this);
        Managers.Event.AddListener(EEventType.OnPlayerTurn, this);
        Managers.Event.AddListener(EEventType.OnBattleStart, this);  
        Managers.Event.AddListener(EEventType.OnBattleWin, this);   
        Managers.Event.AddListener(EEventType.BattleLose, this);

        ShowUI();
        RefreshUI();
    }

    public void OnEvent(EEventType eventType, object sender, object param = null)
    {
        switch (eventType)
        {
            case EEventType.OnBattleStart:
                ShowUI();
                RefreshUI();
                break;

            case EEventType.OnBattleWin:
                HideUI();
                break;
            case EEventType.BattleLose:
                HideUI();
                break;

            case EEventType.OnEnergyCostChanged:
                RefreshUI();
                break;
            case EEventType.OnPlayerTurn:
                costRegenParticle.Play();
                costImageTransform.DOComplete();
                costImageTransform.DOPunchScale(Vector3.one * 0.2f, 1f);
                break;
        }
    }

    public void ShowUI()=>gameObject.SetActive(true);   
    public void HideUI()=>gameObject.SetActive(false);

    public void RefreshUI()
    {
        int currentCost = Managers.Battle.PlayerCost;
        int maxCost = Managers.Battle.MaxCost;
        costText.text = $"{currentCost}/{maxCost}";
    }
}
