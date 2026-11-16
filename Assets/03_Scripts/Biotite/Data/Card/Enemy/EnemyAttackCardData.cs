using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameData/SkillData/EnemyAttackCardData")]
public class EnemyAttackCardData : EnemyCardData
{
    [SerializeField] private EAtkRange atkRange;

    public EAtkRange AtkRange => atkRange;
}
