using DG.Tweening;
using Nobi.UiRoundedCorners;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;


public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler , IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private Card card;
    private Hero ownHero;

    [SerializeField] private CanvasGroup canvasGroup;

    private ImageWithRoundedCorners[] roundedCornerImages;
    private ImageWithIndependentRoundedCorners[] indepRoundedImages;

    public CardData CardData {  get { return card.CardData; } }
    public HeroCardData HeroCardData {  get { return card?.CardData as HeroCardData; } }

    public CardIdleState IdleState { get; private set; }
    public CardHoverState HoverState { get; private set; }
    public CardSelectState SelectState { get; private set; }
    public CardTargetingState TargetState { get; private set; }
    public CardUsedState UsedState { get; private set; }

    private ICardState currentState;

    public ICardState CurrentState => currentState;

    public List<Unit> targetList = new List<Unit>();

    [Header("UI Elements")]
    [SerializeField] private GameObject cardBorderEffect;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardDescText;
    [SerializeField] private TMP_Text cardTypeText;
    [SerializeField] private TMP_Text cardCostText;

    public event Action<CardUI> UseCardEvent;

    [Header("Settings")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    private Vector3 originalScale;
    private float originalHeight;
    private int originalSiblingIndex;
    private Vector3 originalPosition;
    [SerializeField] private string cardState;

    public Vector3 HoverScale => hoverScale;
    public Vector3 OriginalScale => originalScale;
   
    public float OriginalHeight => originalHeight;
    public int OriginalSiblingIndex => originalSiblingIndex;
    public Vector3 OriginalPosition => originalPosition;

    [SerializeField] private float playThresholdY = 200f;
    [SerializeField] float additionalYOffset = 50f;

    public float AdditionalYOffset => additionalYOffset;    
    public float TriggerHeight => originalHeight + playThresholdY;

    private bool isAnchored = false;

    private bool isOwnerDeadCached = false;
    private bool isBorderEffectActiveCached = false;

    public bool IsAnchored {
        get => isAnchored;
        set {
            isAnchored = value;
            if (isAnchored)
            {
                if (currentState != IdleState && currentState != HoverState)
                {
                    ChangeState(IdleState); 
                }

                transform.DOKill();

                RefreshDescription();
            }
        }
    }

    public void SetOriginLayout(Vector3 position)
    {
        originalSiblingIndex = transform.GetSiblingIndex();
        originalPosition = position;
        originalHeight = transform.position.y;
    }
    public void SetOriginLayout()
    {
        originalSiblingIndex = transform.GetSiblingIndex();
        originalPosition = transform.position;
        originalHeight = transform.position.y;
    }
    public static CardUI ActiveCard { get; private set; } = null;

    public static void SetActiveCard(CardUI card) { ActiveCard = card; }
    public static void ClearActiveCard() { ActiveCard = null; }

    private void Awake()
    {
        IdleState = new CardIdleState(this);
        HoverState = new CardHoverState(this);
        SelectState = new CardSelectState(this);
        TargetState = new CardTargetingState(this);
        UsedState = new CardUsedState(this);

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        roundedCornerImages = GetComponentsInChildren<ImageWithRoundedCorners>(true);
        indepRoundedImages = GetComponentsInChildren<ImageWithIndependentRoundedCorners>(true);
    }
    private void Start()
    {
        originalScale = transform.localScale;
        ChangeState(IdleState);
    }

    public Card Init(Card card, Hero hero = null)
    {
        this.card = card;
        this.ownHero = hero;

        if (card != null)
        {
            cardNameText.text = card.CardData.CardName;
            cardImage.sprite = card.CardData.CardIcon;
            cardDescText.text = card.BuildDescription();
            cardTypeText.text = card.CardData.CardType switch
            {
                ECardType.Attack => "공격",
                ECardType.Skill  => "스킬",
                _                => "null"
            };
            cardCostText.text = HeroCardData.EnergyCost.ToString();
        }

        return card;
    }
    public void ChangeState(ICardState newstate)
    {
        if (currentState == newstate) return;

        currentState?.Exit();
        currentState = newstate;
        cardState = currentState.GetType().Name;
        currentState?.Enter();
    }

    private void Update()
    {
        bool ownerIsDead = ownHero != null && ownHero.UnitStat.IsDie;

        if (ownerIsDead != isOwnerDeadCached)
        {
            isOwnerDeadCached = ownerIsDead;
            SetDeadCardVisual(isOwnerDeadCached);
        }

        if (isOwnerDeadCached) return;

        currentState?.Update();

        if (isAnchored && currentState != UsedState)
        {
            bool CanUsed = Managers.Battle.PlayerCost >= HeroCardData.EnergyCost;

            if (isBorderEffectActiveCached != CanUsed)
            {
                isBorderEffectActiveCached = CanUsed;
                cardBorderEffect.SetActive(CanUsed);
            }
        }
        else
        {
            if (isBorderEffectActiveCached)
            {
                isBorderEffectActiveCached = false;
                cardBorderEffect.SetActive(false);
            }
        }
    }

    private void SetDeadCardVisual(bool isDead)
    {
        canvasGroup.blocksRaycasts = !isDead;
        canvasGroup.interactable = !isDead;

        cardImage.color = isDead ? Color.gray : Color.white;

        if (roundedCornerImages != null)
        {
            foreach (var rc in roundedCornerImages)
            {
                if(rc != null) rc.enabled = !isDead;
            }
        }

        if (indepRoundedImages != null)
        {
            foreach (var irc in indepRoundedImages)
            {
                if(irc != null) irc.enabled = !isDead;
            }
        }

        if (isDead)
        {
            if (IsAnchored) IsAnchored = false;
            if (cardBorderEffect != null) cardBorderEffect.SetActive(false);
            isBorderEffectActiveCached = false; 
        }
    }

    private void OnEnable()
    {
        isBorderEffectActiveCached = false;

        if(cardBorderEffect != null) cardBorderEffect.SetActive(false);
    }

    private void OnDisable()
    {
        if (currentState is CardTargetingState)
        {
            Cursor.visible = true;
            Managers.UI.ShowBazierArrow(false);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus) Cursor.visible = true;
    }

    #region [Pointer Events]

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardUI.ActiveCard != null) return;

        if (currentState == IdleState && IsAnchored)
        {
            ChangeState(HoverState);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == HoverState)
            ChangeState(IdleState);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (currentState == HoverState && Managers.Battle.PlayerCost >= HeroCardData.EnergyCost)
            {
                Managers.Sound.PlaySound(Defines.SoundKey.CARD_MOVE_SFX, false, ESoundType.Sfx);
                ChangeState(SelectState);
            }
            else if (currentState == HoverState)
            {
                PlayRejectAnimation();
            }
        }
    }

    #endregion

    #region [Drag Events]

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isAnchored) return;

        if (currentState == HoverState && Managers.Battle.PlayerCost >= HeroCardData.EnergyCost)
        {
            Managers.Sound.PlaySound(Defines.SoundKey.CARD_MOVE_SFX, false, ESoundType.Sfx);
            ChangeState(SelectState);
        }
        else if(currentState == HoverState)
        {
            PlayRejectAnimation();
        }
    }

    public void OnDrag(PointerEventData eventData) { }


    public void OnEndDrag(PointerEventData eventData) { }

    #endregion

    #region [Util]

    public void GetPotentialTargets()
    {
        ClearTargetData();

        ECardType cardType = CardData.CardType;

        if (cardType == ECardType.Skill)
        {
            HeroSkillCardData skillCardData = CardData as HeroSkillCardData;

            if (skillCardData.SkillRange == ESkillRange.Self)
            {
                if (ownHero != null && !ownHero.UnitStat.IsDie)
                {
                    targetList.Add(ownHero);
                }
            }
            else if (skillCardData.SkillRange == ESkillRange.Party)
            {
                foreach (Hero h in Managers.Battle.Heroes)
                {
                    if(h != null && !h.UnitStat.IsDie)
                    {
                        targetList.Add(h);
                    }
                }
            }
        }
        else if (cardType == ECardType.Attack)
        {
            HeroAttackCardData atkCardData = CardData as HeroAttackCardData;

            if (atkCardData.AtkRange == EAtkRange.Range)
            {
                foreach (Enemy e in Managers.Battle.Enemies)
                {
                    if(e != null && !e.UnitStat.IsDie)
                    {
                        targetList.Add(e);
                    }
                }
            }
            else if (atkCardData.AtkRange == EAtkRange.Single)
            {
                Unit mouseEnemy = GetUnitUnderMouse();
                if (mouseEnemy != null && !mouseEnemy.UnitStat.IsDie)
                {
                    targetList.Add(mouseEnemy);
                }
            }
        }
    }

    private void ClearTargetData()
    {
        targetList.Clear();
    }


    public void ClearTargetUI()
    {
        if (targetList != null)
        {
            foreach (var t in targetList)
            {
                if (t == null) continue;

                t.HideTargetUI();
            }
            ClearTargetData();
        }

    }

    public void UpdateTargetsHighlight()
    {
        foreach (var t in targetList)
        {
            if (t == null || t.UnitStat.IsDie) continue;

            t.ShowTargetUI();         
        }
    }

    public Unit GetUnitUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<Unit>();
        }
        return null;
    }

    public void PlayRejectAnimation()
    {
        transform.DOKill();

        transform.DOShakePosition(0.2f, new Vector3(15f, 0, 0), 30, 0, false, true);
    }

    public void UseCard(List<Unit> target)
    {

        if (card == null) return;
        int cost = HeroCardData.EnergyCost;

        if (!Managers.Battle.UseCost(cost)) return;

        UseCardEvent?.Invoke(this);
        Managers.Battle.EnqueuePlayerAction(card, target);
    }

    public void RefreshDescription()
    {
        if (card == null) return;
        cardDescText.text = card.BuildDescription();
    }

    #endregion
}
