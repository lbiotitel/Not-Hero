using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Condition/TurnCondition")]
public class TurnCondition : SkillCondition
{
    [SerializeField] int requiredTurn;

    public override bool Evaluate(float currentHp, float maxHp, int totalTurn, EnemyCardData lastUsedSkill)
    {
        throw new System.NotImplementedException();
    }
}
