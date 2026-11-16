using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//덱의 카드 정보들이나 묘지의 카드 정보들을 보여주는 UI 입니다.
public class CardListViewUI : MonoBehaviour
{
    [SerializeField] private StaticCardUI cardListViewNodeUIPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private int poolingCount;
    [SerializeField] private Button ExitButton;

    private List<StaticCardUI> cardListViewNodeUIList = new List<StaticCardUI>();
    private List<HeroCardData> currentCardList;

    public void Init()
    {
        ExitButton.onClick.AddListener(() => Hide());
        InitCardListViewNode();
    }


    private void InitCardListViewNode()
    {
        for (int i = 0; i < poolingCount; i++)
        {
            StaticCardUI cardListViewNodeUI = Instantiate(cardListViewNodeUIPrefab, contentTransform);
            cardListViewNodeUIList.Add(cardListViewNodeUI);
        }
    }


    public void Show(List<HeroCardData> cardDataList)
    {
        gameObject.SetActive(true);
        currentCardList = cardDataList;
        DrawCardList();
    }

    private void DrawCardList()
    {
        HideAllCardNode();
        for (int i = 0; i < currentCardList.Count; i++)
        {
            cardListViewNodeUIList[i].Init(currentCardList[i]);
            cardListViewNodeUIList[i].Show();
        }
    }


    private void HideAllCardNode()
    {
        for (int i = 0; i < cardListViewNodeUIList.Count; i++)
        {
            cardListViewNodeUIList[i].Hide();
        }
    }


    public void Hide() => gameObject.SetActive(false);

}
