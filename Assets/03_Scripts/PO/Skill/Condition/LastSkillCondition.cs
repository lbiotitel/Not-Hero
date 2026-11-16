using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Condition/LastSkillCondition")]
public class LastSkillCondition : SkillCondition
{
    [SerializeField] EnemyCardData targetSkill;

    public override bool Evaluate(float currentHp, float maxHp, int totalTurn, EnemyCardData lastUsedSkill)
    {
        throw new System.NotImplementedException();
    }
}

