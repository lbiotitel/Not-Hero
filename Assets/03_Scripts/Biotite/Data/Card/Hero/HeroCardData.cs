using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCardData : CardData
{
    [Header("Hero Card Properties")]
    [SerializeField] private EGrade cardGrade;
    [SerializeField] private EClassType classType;
    [SerializeField] private int energyCost;
    [SerializeField] private int price;

    public EGrade CardGrade => cardGrade;
    public int EnergyCost => energyCost;
    public int Price => price;
    public EClassType ClassType => classType;
}
