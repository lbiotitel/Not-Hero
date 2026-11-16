using System.Collections.Generic;
using UnityEngine;

public static class CardUtility
{
    private static List<HeroCardData> cachedPartyCards = new List<HeroCardData>();

    public static void GetPartyFilterCards(List<HeroCardData> allCardDataList, List<Hero> currentHeros)
    {
        cachedPartyCards.Clear();

        HashSet<EClassType> activeClasses = new HashSet<EClassType>();

        foreach (var hero in currentHeros)
        {
            if (hero != null)
            {
                activeClasses.Add(hero.classType);
            }
        }

        foreach (HeroCardData card in allCardDataList)
        {


            if (activeClasses.Contains(card.ClassType))
            {
                cachedPartyCards.Add(card);
            }
        }

        Debug.Log($"[Debug] 최종 캐싱된 카드 수: {cachedPartyCards.Count}");

        if (cachedPartyCards.Count > 0)
        {
            string cardNames = "";
            foreach (var card in cachedPartyCards)
            {
                cardNames += $"[{card.CardName}({card.CardGrade})] ";
            }
            Debug.Log($"[Debug] 캐싱된 카드 목록: {cardNames}");
        }
        else
        {
            Debug.LogWarning("[Debug] 캐싱된 카드가 하나도 없습니다! 필터 조건(직업)을 확인하세요.");
        }
    }

    public static List<HeroCardData> GetCachedPartyCards() => cachedPartyCards;


}
