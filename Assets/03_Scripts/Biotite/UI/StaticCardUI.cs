using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaticCardUI : MonoBehaviour
{

    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardDescText;
    [SerializeField] private TMP_Text cardEnergyCostText;


    protected bool isAnim = false;

    public HeroCardData CardData { get; private set; }



    public virtual void Init(HeroCardData cardData)
    {
        CardData = cardData;
        DrawStaticCardUI();
    }


    protected virtual void DrawStaticCardUI()
    {
        Unit ownUnit = Managers.Battle.GetHeroByClassType(CardData.ClassType);

        cardImage.sprite = CardData.CardIcon;
        cardNameText.text = CardData.CardName;
        cardDescText.text = DescBuilder.BuildDescription(ownUnit, CardData.CardEffects);
        cardEnergyCostText.text = CardData.EnergyCost.ToString();
    }




    public void Show()
    { 
        DrawStaticCardUI();
        gameObject.SetActive(true);
    }
    public void Hide() => gameObject.SetActive(false);


}
