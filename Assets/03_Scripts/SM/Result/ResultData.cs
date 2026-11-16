using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData
{
    public List<Hero> heroList;
    public List<CardData> cardList;
    public List<ItemData> itemList;
    public int playTime;

    public EResult result;

    public enum EResult
    {
        Victory,
        Defeat
    }

    
    
}
