using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu]
public class GradeProbability : ScriptableObject
{
    [System.Serializable]
    private struct GradeProbabilityData
    {
        public EGrade Grade;
        public int weight;
    }


    [SerializeField] private List<GradeProbabilityData> gradeProbabilityDataList = new List<GradeProbabilityData>();

    private int totalWeight;

    private void OnValidate() => CalcWeight();
    private void OnEnable() => CalcWeight();

    private void CalcWeight()
    {
        totalWeight = 0;
        foreach (var data in gradeProbabilityDataList)
        {
            totalWeight += data.weight;
        }
    }

    public EGrade GetGrade()
    {
        if (totalWeight <= 0)
        {
            CalcWeight();
            if (totalWeight <= 0) return EGrade.Common;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var data in gradeProbabilityDataList)
        {
            currentWeight += data.weight;
            if (randomValue < currentWeight)
            {
                return data.Grade;
            }
        }

        return EGrade.Common;
    }

    public List<HeroCardData> GetRandomCardList(List<HeroCardData> filteredCards, int count)
    {
        List<HeroCardData> result = new List<HeroCardData>();

        if (filteredCards == null || filteredCards.Count == 0) return result;

        for (int i = 0; i < count; i++)
        {
            HeroCardData pickedCard = null;

            for (int retry = 0; retry < 10; retry++)
            {
                EGrade selectedGrade = GetGrade();
                List<HeroCardData> gradeMatched = filteredCards.FindAll(c => c.CardGrade == selectedGrade);

                if (gradeMatched.Count > 0)
                {
                    pickedCard = gradeMatched[Random.Range(0, gradeMatched.Count)];
                    break; 
                }

            }

            if (pickedCard == null)
            {

                pickedCard = filteredCards[Random.Range(0, filteredCards.Count)];
            }

            result.Add(pickedCard);
        }

        for (int i = 0; i < result.Count; i++)
        {
            int randomIndex = Random.Range(i, result.Count);
            HeroCardData temp = result[i];
            result[i] = result[randomIndex];
            result[randomIndex] = temp;
        }

        return result;
    }


}
