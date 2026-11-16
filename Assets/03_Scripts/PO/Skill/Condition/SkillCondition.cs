using UnityEngine;


public abstract class SkillCondition : ScriptableObject
{
    public abstract bool Evaluate(float currentHp, float maxHp, int totalTurn, EnemyCardData lastUsedSkill);
}
