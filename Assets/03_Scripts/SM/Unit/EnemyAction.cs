
using System.Collections.Generic;

class EnemyAction
{
    public EnemyCardData SkillData { get; }
    public List<Unit> hero{ get; }

    public EnemyAction(EnemyCardData skillData, List<Unit> hero)
    {
        this.SkillData = skillData;
        this.hero = hero;
    }
}