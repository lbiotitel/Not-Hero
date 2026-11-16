using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;



public interface ICardState
{
    void Enter();
    void Update();
    void Exit();
}

public class CardIdleState : ICardState
{
    private CardUI cardUI;
    public CardIdleState(CardUI cardUI) { this.cardUI = cardUI; }

    public void Enter()
    {
        if (cardUI.IsAnchored)
        {

            cardUI.transform.DOKill();
            cardUI.transform.SetSiblingIndex(cardUI.OriginalSiblingIndex);

            cardUI.transform.DOScale(cardUI.OriginalScale, 0.15f);
            cardUI.transform.DOMove(cardUI.OriginalPosition, 0.15f).SetEase(Ease.OutQuad);
            
        }
    }

    public void Update()
    {

    }

    public void Exit()
    {

    }
}

public class CardHoverState : ICardState
{
    private CardUI cardUI;
    public CardHoverState(CardUI cardUI) { this.cardUI = cardUI; }

    public void Enter()
    {
        if (CardUI.ActiveCard != null && CardUI.ActiveCard != cardUI) return;

        cardUI.transform.SetAsLastSibling();
        cardUI.transform.DOScale(cardUI.HoverScale, 0.2f).SetEase(Ease.OutCubic);
        float scaleYOffset = cardUI.GetComponent<RectTransform>().rect.height * (cardUI.HoverScale.y - 1f) * 0.5f;

        cardUI.transform.DOMoveY(cardUI.OriginalHeight + scaleYOffset + cardUI.AdditionalYOffset, 0.2f);
    }

    public void Update()
    {

    }

    public void Exit()
    {
           
    }
}

public class CardSelectState : ICardState
{
    private CardUI cardUI;
    public CardSelectState(CardUI cardUI) { this.cardUI = cardUI; }

    public void Enter()
    {
        CardUI.SetActiveCard(cardUI);

        cardUI.transform.SetAsLastSibling();
    }                

    public void Update()
    {
        cardUI.transform.position = Input.mousePosition;

        if (Input.GetMouseButtonUp(1))
        {
            cardUI.ChangeState(cardUI.IdleState);
            cardUI.ClearTargetUI();
            return;
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1)) return;

        bool isAboveThreshold = Input.mousePosition.y > cardUI.TriggerHeight;

        if (isAboveThreshold)
        {
            if (cardUI.CardData is HeroAttackCardData atkData && atkData.AtkRange == EAtkRange.Single)
            {
                cardUI.ChangeState(cardUI.TargetState);
                return;
            }
            else
            {
                cardUI.GetPotentialTargets();
                cardUI.UpdateTargetsHighlight();
            }
        }
        else
        {
            cardUI.ClearTargetUI();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isAboveThreshold && !(cardUI.CardData is HeroAttackCardData atkData && atkData.AtkRange == EAtkRange.Single))
            {
                cardUI.GetPotentialTargets();
                cardUI.ChangeState(cardUI.UsedState);
            }
            else
            {
                cardUI.ClearTargetUI();
            }
        }
    }

    public void Exit()
    {
        CardUI.ClearActiveCard();
    }
}

public class CardTargetingState : ICardState
{
    private CardUI cardUI;
    public CardTargetingState(CardUI cardUI) { this.cardUI = cardUI; }

    private Unit currentEnemy;

    public void Enter()
    {
        CardUI.SetActiveCard(cardUI);
        cardUI.transform.SetAsLastSibling();

        Cursor.visible = false;

        float bottomY = cardUI.GetComponent<RectTransform>().rect.height * cardUI.transform.localScale.y * 0.5f;
        Vector3 cardPos = new Vector3(Screen.width * 0.5f, bottomY, cardUI.transform.position.z);

        cardUI.transform.DOMove(cardPos, 0.2f).OnComplete(() => {
            if (cardUI.CurrentState is CardTargetingState)
            {
                Managers.UI.ShowBazierArrow(true, cardUI.transform.position);
            }
        });
    }

    public void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            cardUI.ChangeState(cardUI.IdleState);
            cardUI.ClearTargetUI();
            return;
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1)) return;

        if (Input.mousePosition.y < 30f)
        {
            cardUI.ChangeState(cardUI.IdleState);
            return;
        }

        Unit hitUnit = cardUI.GetUnitUnderMouse();

        if (hitUnit != null && hitUnit.UnitStat.IsDie) hitUnit = null;

        if (hitUnit != currentEnemy)
        {
            cardUI.ClearTargetUI();
            currentEnemy = hitUnit;

            if (currentEnemy != null)
            {
                cardUI.GetPotentialTargets();
                cardUI.UpdateTargetsHighlight();
            }
        }

        Managers.UI.ShowBazierArrow(true, cardUI.transform.position);
        Managers.UI.SetArrowColor(currentEnemy != null ? Color.red : Color.white);

        if (Input.GetMouseButtonUp(0))
        {
            if (currentEnemy != null && !currentEnemy.UnitStat.IsDie)
            {
                Managers.UI.ShowBazierArrow(false);
                cardUI.ChangeState(cardUI.UsedState);
            }
            else
            {
                cardUI.ChangeState(cardUI.IdleState);
            }
        }


    }

    public void Exit()
    {
        CardUI.ClearActiveCard();
        Managers.UI.ShowBazierArrow(false);
        currentEnemy = null;

        Cursor.visible = true;
    }
}

public class CardUsedState : ICardState
{
    private CardUI cardUI;
    public CardUsedState(CardUI cardUI) { this.cardUI = cardUI; }

    public void Enter()
    {
        cardUI.UseCard(cardUI.targetList);
        cardUI.ClearTargetUI();
    }

    public void Update()
    {

    }

    public void Exit()
    {

    }
}
