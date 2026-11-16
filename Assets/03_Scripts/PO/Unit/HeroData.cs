using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Unit/Hero")]
public class HeroData : UnitData
{
    [SerializeField] CHRClassData classData;
    [SerializeField] private int baseDef;

    [SerializeField] List<string> startCardName;


    public CHRClassData ClassData => classData;
    public int BaseDEF => baseDef;

    public List<string> StartCardName => startCardName;
}