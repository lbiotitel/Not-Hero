using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardShopUI : StaticCardUI, IPointerClickHandler, IPointerEnterHandler
{

    [SerializeField] private TMP_Text cardShopCostText;
    [SerializeField] protected float animScale;

    private CanvasGroup canvasGroup;
    private bool isSell = false;
    public event Action<CardShopUI> OnCardClicked;


    public override void Init(HeroCardData cardData)
    {
        base.Init(cardData);
        isSell = false;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
    }

    protected override void DrawStaticCardUI()
    {
        base.DrawStaticCardUI();
        cardShopCostText.text = CardData.Price.ToString();
    }


    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (isSell) return;
        OnCardClicked?.Invoke(this);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSell) return;
        if (isAnim) return;
        isAnim = true;
        transform.DOPunchScale(Vector3.one * animScale, 0.1f).
            OnComplete(() => isAnim = false);
    }

    public void Sell()
    {
        canvasGroup.alpha = 0.5f;
        isSell = true;
    }

}
