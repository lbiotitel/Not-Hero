using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//모든 플레이어의 자산을 관리 합니다.
public class PlayerStatManager : MonoBehaviour
{
    [SerializeField] private RewardResult rewardResult;
    [SerializeField] private int gold;
    [SerializeField] private List<CardData> currentCardList = new List<CardData>();
    [SerializeField] private List<ItemData> currentItemList = new List<ItemData>();

    public List<CardData> CurrentCardList => currentCardList;
    public List<ItemData> CurrentItemList => currentItemList;

    public void Init()
    { 
        
    }

    public void Clear()
    { 
        currentCardList.Clear();
        currentItemList.Clear();
    }

    public void AddNewCard(string cardName, Hero ownUnit)
    {
        AddNewCard(Managers.Data.GetData<CardData>(cardName), ownUnit);
    }

    public void AddNewCard(CardData cardData, Hero ownUnit)
    { 
        currentCardList.Add(cardData);
        object[] cardDataPair = { cardData, ownUnit };
        Managers.Event.NotifyEvent(EEventType.OnAddNewCard, this, cardDataPair);
    }


    public bool IsEnoughGold(int price)
    {
        if (this.gold < price) return false;
        return true;
    }

    public void SpendGold(int price)
    {
        this.gold -= price;
        Managers.Event.NotifyEvent(EEventType.OnGoldValueChanged, this, gold);
    }

    public void AddGold(int gold)
    {
        this.gold += gold;
        Managers.Event.NotifyEvent(EEventType.OnGoldValueChanged, this, this.gold);
    }

    public void AddNewItem(ItemData item)
    {
        currentItemList.Add(item);
        Managers.Event.NotifyEvent(EEventType.OnAddNewItem, this, item);
    }
}
