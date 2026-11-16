using System.Collections.Generic;
using UnityEngine;


//스킬 설명이나 여러 문자열을 만드는 클래스 입니다
public class DescBuilder : MonoBehaviour
{

    public enum EDescKey
    {
        Damage,
        AtkCount,
        Armor,
        DrawCount,
        Heal,
        Cost
    }

    public static readonly Dictionary<EDescKey, string> descriptionKeys = new Dictionary<EDescKey, string>()
    {
        { EDescKey.Damage ,"{Damage}" }, //데미지
        { EDescKey.AtkCount,"{AtkCount}" }, //공격 횟수
        { EDescKey.Armor, "{Armor}" }, //방어도
        { EDescKey.DrawCount, "{DrawCount}" }, //드로우 수
        { EDescKey.Heal, "{Heal}" }, //드로우 수
        { EDescKey.Cost, "{Cost}" }, //코스트 
    };

    public static readonly Dictionary<EDescKey, string> descriptionColor = new Dictionary<EDescKey, string>()
    {
        { EDescKey.Damage , "red" }, //데미지 빨간색
        { EDescKey.AtkCount, "yellow" }, //공격 횟수 노란색
        { EDescKey.Armor, "#00FFFF"}, //방어도 파란색
        { EDescKey.DrawCount, "#800080"}, //드로우 수 보라색
        { EDescKey.Heal, "green"}, //힐량 녹색
        { EDescKey.Cost, "orange"}, //힐량 녹색
    };


    public static string BuildDescription(string description, EDescKey[] keys, object[] param)
    {
        if (description == string.Empty) return description;

        string res = description;

        int minIndex = Mathf.Min(keys.Length, param.Length);

        for (int i = 0; i < minIndex; i++)
        {
            string descKey = descriptionKeys[keys[i]];
            res = res.Replace(descKey,
                $"<color={descriptionColor[keys[i]]}>" + param[i].ToString() + "</color>");
        }

        return res;
    }

    public static string BuildDescription(Unit unit, List<CardEffect> cardEffectList)
    {
        string res = string.Empty;

        foreach (var cardEffect in cardEffectList)
        {
            res += cardEffect.BuildDescription(unit, string.Empty);
        }

        return res;
    }
}
