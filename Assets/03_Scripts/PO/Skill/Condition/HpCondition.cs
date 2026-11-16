using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Condition/HPCondition")]
public class HpCondition : SkillCondition
{
    [SerializeField] float hpPercent;

    public override bool Evaluate(float currentHp, float maxHp, int totalTurn, EnemyCardData  lastUsedSkill)
    {
        bool result = currentHp  <= hpPercent;
        Debug.Log($"[HpCondition] 현재 HP 비율 : {currentHp / maxHp:P0} / 기준 : {hpPercent:P0} / 만족 : {result}");
        return result;
    }
}
