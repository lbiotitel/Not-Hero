using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSystem : MonoBehaviour, IEventListener
{
    [Header("Card Data")]
    [SerializeField] private CardUI cardUIPrefab;

    [Header("Card Lists")]
    private List<CardUI> cardList = new List<CardUI>();
    private List<CardUI> deckCardList = new List<CardUI>();
    private List<CardUI> handCardList = new List<CardUI>();
    private List<CardUI> discardCardList = new List<CardUI>();

    [Header("Transforms")]
    [SerializeField] private Transform deckTransform;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform discardTransform;

    [Header("Settings")]
    [SerializeField] private float maxCardSpacing;
    [SerializeField] private float handWidth;
    [SerializeField] private int drawCount;
    [SerializeField] private float cardDrawInterval;
    [SerializeField] private float animDuration;
    [SerializeField] private float cardSortInterval;

    [Header("Text UI")]
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private TMP_Text discardCountText;

    [Header("UI")]
    [SerializeField] private Button deckButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private GameObject cardSystemUI;

    private Dictionary<EEventType, Action> eventBinder = new Dictionary<EEventType, Action>();

    //이벤트
    public event Action OnDrawAnimStarted;
    public event Action OnDrawAnimFinished;

    private Coroutine sortCoroutine;
    private Coroutine drawQueueCoroutine;
    private Queue<(int count, bool extraDraw)> drawRequests = new Queue<(int, bool)>();
    public bool IsDraw { get; private set; }

    private void OnDestroy()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnAddNewCard, this);
            Managers.Event.RemoveListener(EEventType.OnEnemyTurn, this);
            Managers.Event.RemoveListener(EEventType.OnBattleWin, this);
            Managers.Event.RemoveListener(EEventType.OnPlayerTurn, this);
            Managers.Event.RemoveListener(EEventType.OnBattleStart, this);

            cardList.Clear();
            deckCardList.Clear();
            discardCardList.Clear();
            handCardList.Clear();

            eventBinder.Clear();
        }
    }

    public void Init()
    {
        Managers.Event.AddListener(EEventType.OnAddNewCard, this);
        Managers.Event.AddListener(EEventType.OnPlayerTurn, this);
        Managers.Event.AddListener(EEventType.OnEnemyTurn, this);
        Managers.Event.AddListener(EEventType.OnBattleWin, this);
        Managers.Event.AddListener(EEventType.OnBattleStart, this);

        eventBinder.Add(EEventType.OnPlayerTurn, () => Draw());
        eventBinder.Add(EEventType.OnBattleWin, () => { ResetAllCard(); cardSystemUI.gameObject.SetActive(false); });
        eventBinder.Add(EEventType.OnEnemyTurn, () => StartCoroutine(CoAlHandlCardToPile()));
        eventBinder.Add(EEventType.OnBattleStart, () => cardSystemUI.gameObject.SetActive(true));
        

        deckButton.onClick.AddListener(() => ShowCardListView(deckCardList));
        discardButton.onClick.AddListener(() => ShowCardListView(discardCardList));
    }

    private List<HeroCardData> GetCardListFromCardUI(List<CardUI> cardUIList)
    {
        List<HeroCardData> cardDataList = new List<HeroCardData>();
        foreach (var cardUI in cardUIList)
        {
            cardDataList.Add(cardUI.HeroCardData);
        }
        return cardDataList;
    }

    //덱 또는 묘지 리스트 보기
    private void ShowCardListView(List<CardUI> cardDataList)
    {
        if (IsDraw) return;
        Managers.UI.CardListViewUI.Show(GetCardListFromCardUI(cardDataList));
    }

    //카드 생성
    private void CreateCard(CardData cardData, Hero ownUnit)
    {
        CardUI cardUI = Instantiate(cardUIPrefab, deckTransform);
        Card card = new Card();
        card.Init(cardData, ownUnit);
        cardUI.Init(card, ownUnit);
        cardUI.gameObject.SetActive(false);
        cardUI.UseCardEvent += UseCard;

        cardList.Add(cardUI);
        deckCardList.Add(cardUI);
        UpdateCardCountUI();
    }

    //드로우
    public void Draw(int count = 0, bool extraDraw = false)
    {
        int finalCount = (count == 0) ? drawCount : count;
        drawRequests.Enqueue((finalCount, extraDraw));

        if (drawQueueCoroutine == null)
        {
            drawQueueCoroutine = StartCoroutine(CoProcessDrawQueue());
        }
    }

    private IEnumerator CoProcessDrawQueue()
    {
        while (drawRequests.Count > 0)
        {
            var request = drawRequests.Dequeue();

            IsDraw = true;
            OnDrawAnimStarted?.Invoke();

            yield return StartCoroutine(CoDraw(request.count, request.extraDraw));

            IsDraw = false;
            OnDrawAnimFinished?.Invoke();
        }

        drawQueueCoroutine = null;
    }

    //드로우
    private IEnumerator CoDraw(int count, bool extraDraw)
    {
        if (!extraDraw && handCardList.Count > 0)
        {
            yield return StartCoroutine(CoAlHandlCardToPile());
            yield return new WaitForSeconds(0.2f);
        }

        if (deckCardList.Count < count && discardCardList.Count > 0)
        {
            yield return StartCoroutine(CoPileToDeck());
            yield return new WaitForSeconds(0.3f);
        }

        yield return StartCoroutine(CoDrawCard(count));
    }

    //드로우 코루틴 로직
    private IEnumerator CoDrawCard(int count)
    {
        int countToDraw = Math.Min(count, deckCardList.Count);

        for (int i = 0; i < countToDraw; i++)
        {
            CardUI cardUI = GetRandomCardInDeck();
            if (cardUI == null) continue;

            handCardList.Add(cardUI);
            cardUI.IsAnchored = false;
            cardUI.transform.SetParent(handTransform);
            cardUI.transform.SetAsLastSibling();
            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.rotation = Quaternion.identity;
            cardUI.gameObject.SetActive(true);

            UpdateCardCountUI();
            ReorderHandCards();

            yield return new WaitForSeconds(cardDrawInterval);
        }

        ReorderHandCards();
    }

    //패모두 버리기
    private IEnumerator CoAlHandlCardToPile()
    {
        for (int i = handCardList.Count - 1; i >= 0; i--)
        {
            CardUI cardUI = handCardList[i];
            handCardList.RemoveAt(i);
            discardCardList.Add(cardUI);
            MoveRotateAnimation(cardUI, discardTransform.position, animDuration, true);
        }
        UpdateCardCountUI();
        yield return new WaitForSeconds(animDuration);
    }

    //카드 사용시 처리
    private void UseCard(CardUI cardUI)
    {
        CancelHandSorting();

        if (!handCardList.Contains(cardUI))
            return;

        handCardList.Remove(cardUI);
        discardCardList.Add(cardUI);

        Managers.Sound.PlaySound(Defines.SoundKey.CARD_MOVE_SFX, false, ESoundType.Sfx);
        UseCardAnimation(cardUI, discardTransform.position, animDuration * 1.2f, true);

        UpdateCardCountUI();
        ReorderHandCards();
    }

    //재정렬
    private void ReorderHandCards()
    {
        List<Vector3> targetPositions = CalcCardPos(handCardList.Count);

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < handCardList.Count; i++)
        {
            var handCard = handCardList[i];
            if (handCard == null) continue;

            handCard.transform.DOKill();
            handCard.gameObject.SetActive(true);
            handCard.transform.SetSiblingIndex(i);
            handCard.SetOriginLayout(targetPositions[i]);

            if (handCard.CurrentState == handCard.SelectState)
                continue;

            seq.Join(
                handCard.transform.DOMove(targetPositions[i], animDuration / 1.5f).
                SetEase(Ease.OutBack)
                );
        }

        seq.OnComplete(() =>
        {
            for (int i = 0; i < handCardList.Count; i++)
            {
                if (handCardList[i] != null)
                    handCardList[i].IsAnchored = true;
            }
        });

     
        if (!IsDraw) OnDrawAnimFinished?.Invoke();
    }

    //묘지에서 덱으로 카드 옮기기
    private IEnumerator CoPileToDeck()
    {
        var t = new WaitForSeconds(0.05f);

        List<CardUI> cardsToMove = new List<CardUI>(discardCardList);
        discardCardList.Clear();

        for (int i = cardsToMove.Count - 1; i >= 0; i--)
        {
            CardUI card = cardsToMove[i];
            deckCardList.Add(card);

            card.IsAnchored = false;
            card.gameObject.SetActive(true);
            card.transform.localScale = Vector3.one * 0.2f;
            card.transform.Rotate(0, 0, 90);
            card.transform.DOMove(deckTransform.position, animDuration)
                .OnComplete(() =>
                {
                    card.gameObject.SetActive(false);
                    card.transform.localScale = Vector3.one;
                    card.transform.localRotation = Quaternion.identity;
                });

            UpdateCardCountUI();
            yield return t;
        }
        yield return new WaitForSeconds(animDuration);
    }


    //손 정렬 캔슬
    private void CancelHandSorting()
    {
        if (sortCoroutine != null)
        {
            StopCoroutine(sortCoroutine);
            sortCoroutine = null;
        }

        foreach (var card in cardList)
        {
            if (card == null) continue;
            card.transform.DOComplete();
        }
    }

    #region Card Animation 
    //카드 사용 애니메이션
    private void UseCardAnimation(CardUI cardUI, Vector3 targetPos, float duration, bool deactivateOnComplete)
    {
        cardUI.IsAnchored = false;
        Vector3 startPos = cardUI.transform.position;
        Vector3 midPoint = (startPos + targetPos) / 2f + Vector3.up * 300f;

        Vector3[] path = new Vector3[] { startPos, midPoint, targetPos };

        cardUI.transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.InQuad);

        cardUI.transform.DORotate(new Vector3(0, 0, 180), duration, RotateMode.FastBeyond360);

        cardUI.transform.DOScale(Vector3.zero, duration * 1.3f).OnComplete(() =>
        {
            cardUI.transform.SetParent(deactivateOnComplete ? discardTransform : deckTransform);
            if (deactivateOnComplete) cardUI.gameObject.SetActive(false);

            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.localRotation = Quaternion.identity;
        });
    }

    //이동 애니메이션
    private void MoveRotateAnimation(CardUI cardUI, Vector3 targetPos, float duration, bool deactivateOnComplete)
    {
        cardUI.IsAnchored = false;
        cardUI.transform.DOMove(targetPos, duration).SetEase(Ease.InQuad);
        cardUI.transform.DOScale(Vector3.zero, duration).OnComplete(() =>
        {
            cardUI.transform.SetParent(deactivateOnComplete ? discardTransform : deckTransform);
            if (deactivateOnComplete) cardUI.gameObject.SetActive(false);
            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.localRotation = Quaternion.identity;
        });
    }
    #endregion

    //정렬 위치 계산
    private List<Vector3> CalcCardPos(int count)
    {
        List<Vector3> res = new List<Vector3>();
        if (count <= 0) return res;

        float currentSpacing = maxCardSpacing;
        if (count > 1)
        {
            float requiredWidth = maxCardSpacing * (count - 1);
            if (requiredWidth > handWidth)
            {
                currentSpacing = handWidth / (count - 1);
            }
        }

        float totalContentWidth = currentSpacing * (count - 1);
        float startX = handTransform.position.x - (totalContentWidth / 2f);

        for (int i = 0; i < count; i++)
        {
            float xPos = startX + (i * currentSpacing);
            res.Add(new Vector3(xPos, handTransform.position.y, 0));
        }

        return res;
    }

    //랜덤한 카드 뽑기
    private CardUI GetRandomCardInDeck()
    {
        int randIndex = UnityEngine.Random.Range(0, deckCardList.Count);
        CardUI cardUI = deckCardList[randIndex];
        deckCardList.RemoveAt(randIndex);
        return cardUI;
    }

    //덱과 묘지의 카드수 업데이트
    private void UpdateCardCountUI()
    {
        deckCountText.text = deckCardList.Count.ToString();
        discardCountText.text = discardCardList.Count.ToString();
    }

    //모든 카드들을 리셋 시킵니다.
    private void ResetAllCard()
    {
        StopAllCoroutines();
        drawRequests.Clear();
        drawQueueCoroutine = null;

        IsDraw = false;
        sortCoroutine = null;

        handCardList.Clear();
        discardCardList.Clear();
        deckCardList.Clear();

        foreach (var cardUI in cardList)
        {
            cardUI.transform.DOKill();
            cardUI.IsAnchored = false;
            cardUI.gameObject.SetActive(false);
            cardUI.transform.SetParent(deckTransform);
            cardUI.transform.position = deckTransform.position;
            cardUI.transform.localRotation = Quaternion.identity;
            cardUI.transform.localScale = Vector3.one;

            deckCardList.Add(cardUI);
        }

        UpdateCardCountUI();
    }



    //이벤트 제어
    public void OnEvent(EEventType eventType, object sender, object param = null)
    {
        if (eventBinder.ContainsKey(eventType))
        {
            eventBinder[eventType]?.Invoke();
        }

        if (eventType == EEventType.OnAddNewCard)
        {
            if (param is object[] p)
            {
                var cardData = p[0] as CardData;
                var ownUnit = p[1] as Hero;
                if (cardData != null && ownUnit != null) CreateCard(cardData, ownUnit);
            }
        }
    }
}