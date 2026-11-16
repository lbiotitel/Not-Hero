using System.Collections.Generic;
using UnityEngine;

public enum EType
{
    Normal,
    Boss
}

[CreateAssetMenu(menuName = "GameData/Unit/EnemyData")]
public class EnemyData : UnitData
{
    [SerializeField] private EType type;

    [SerializeField] private float minHp;
    [SerializeField] private float maxHp;

    [SerializeField] private int minAtk;
    [SerializeField] private int maxAtk;
    [SerializeField] private int baseDef;

    [SerializeField] private SkillPattern pattern;
    [SerializeField] private List<SkillCondition> conditions;

    public Enemy enemyPrefab;

    public EType Type => type;
    public float EMinHp => minHp;
    public float EMaxHp => maxHp;

    public int EMinSTR => minAtk;
    public int EMaxSTR => maxAtk;

    public int EBaseDEF => baseDef;
    public SkillPattern Pattern => pattern;
    public List<SkillCondition> Conditions => conditions;
}