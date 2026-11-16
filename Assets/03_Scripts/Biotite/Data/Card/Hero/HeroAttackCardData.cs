using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "GameData/SkillData/HeroAttackCardData")]
public class HeroAttackCardData : HeroCardData
{
    [Header("Attack Card Properties")]
    [SerializeField] private EAtkRange atkRange;

    public EAtkRange AtkRange => atkRange;


    private void OnEnable()
    {
        cardType = ECardType.Attack;
    }
}

public enum EAtkRange
{
    Single,
    Range
}