using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "GameData/SkillData/HeroSkillCardData")]
public class HeroSkillCardData : HeroCardData
{
    [SerializeField] private ESkillRange skillRange;

    public ESkillRange SkillRange => skillRange;

    private void OnEnable()
    {
        cardType = ECardType.Skill;
    }
}
public enum ESkillRange
{
    Self,
    Party
}