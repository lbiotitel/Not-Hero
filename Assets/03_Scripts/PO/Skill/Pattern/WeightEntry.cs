using UnityEngine;

[System.Serializable]
public class WeightEntry
{
    [SerializeField] float weight;
    [SerializeField] EnemyCardData skill;

    public float Weight => weight;
    public EnemyCardData Skill => skill;
}