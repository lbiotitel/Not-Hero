using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem
{
    private RewardResult rewardResult;
    RewardConfig rewardConfig = new RewardConfig();

    private bool Roll(float dropChance)
    {
        return Random.value < dropChance;
    }

    public RewardResult GenerateReward(int floor)
    {
        RewardResult result = new RewardResult();

        result.gold = GenerateGold(floor);

        if (Roll(rewardConfig.itemChance))
        {
            result.item = GetItemGradeList(floor);
        }

        if (Roll(rewardConfig.cardChance))
        {
            result.card = GetCardGradeList();
        }

        return result;
    }

    public int GenerateGold(int floor)
    {
        if (floor == -1) return -1;

        int minGold = rewardConfig.minGold;
        int maxGold = rewardConfig.maxGold; 

        int getGold = Random.Range((int)(minGold + floor * 1.5), (int)(maxGold + floor * 1.5) + 1);

        return getGold;
    }
    public ItemData GetItemGradeList(int floor)
    {
       List<ItemData> rewardItemList = new List<ItemData>();

        EGrade selectGrade = Managers.Data.GradeProbability.GetGrade();

        List<ItemData> allItemList = Managers.Data.GetDataList<ItemData>();

        foreach (ItemData item in allItemList)
        {
            if (item.ItemGrade == selectGrade)
            {
                rewardItemList.Add(item);
            }
        }

        return SelectItemReward(rewardItemList, floor);
    }

    private ItemData SelectItemReward(List<ItemData> rewardItemList, int floor)
    {
        if (rewardItemList.Count == 0)
        {
            Debug.Log("등급에 맞는 아이템리스트 안넘어옴");
            return null;
        }

        var selectItem = rewardItemList.FindAll(i => (int)i.floorType == floor);

        return selectItem[Random.Range(0, selectItem.Count)];
    }

    public HeroCardData GetCardGradeList()
    {
        List<HeroCardData> rewardCardList = CardUtility.GetCachedPartyCards();

        List<HeroCardData> finalCards = Managers.Data.GradeProbability.GetRandomCardList(rewardCardList, 1);

        if(finalCards == null) Debug.Log("등급에 맞는 카드리스트 안넘어옴");

        return (finalCards != null && finalCards.Count > 0) ? finalCards[0] : null;
    }
    public void Clear()
    {
        
    }
}