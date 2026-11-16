using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameData/SkillData/EnemySkillCardData")]
public class EnemySkillCardData : EnemyCardData
{
    [SerializeField] private ESkillRange skillRange;
    
    public ESkillRange SkillRange => skillRange;
    
}
