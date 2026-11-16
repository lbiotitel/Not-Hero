using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//카드 기능 
public class Card
{
    private Unit ownUnit;
    private CardData cardData;
    public CardData CardData => cardData;
    public Unit OwnUnit => ownUnit;

    public virtual void Init(CardData cardData, Unit ownUnit = null)
    {
        this.ownUnit = ownUnit;
        this.cardData = cardData.Clone() as CardData;
    }

    public virtual void DoCardLogic(List<Unit> targetList)
    {
        GameObject skillEffect = cardData.SkillEffect;
        List<CardEffect> cardEffects = cardData.CardEffects;
        string soundSFX = cardData.SkillSfx;

        if (targetList == null) return;

        foreach(var target in targetList)
        {
            foreach(var cardEffect in cardEffects)
            {
                if (cardEffect == null) continue;
                cardEffect.Excute(this.ownUnit, target, skillEffect,soundSFX);
            }
        }
    }


    public virtual string BuildDescription()
    {
        string res = string.Empty;
        List<CardEffect> cardEffects = cardData.CardEffects;

        foreach (var effect in cardEffects)
        {
            if (effect == null) continue;
            res += $"{effect.BuildDescription(this.ownUnit, cardData.CardDesc)}\n";
        }

        return res;
    }
}