using UnityEngine;

[System.Serializable]
public class SkillPatternEntry
{
    [SerializeField] SkillPattern skillPattern;
    [SerializeField] int index;

    public SkillPatternEntry(SkillPattern skillPattern)
    {
        this.skillPattern = skillPattern;
    }

    public int Index { get => index; set => index = value; }
    public SkillPattern SkillPattern => skillPattern;

    public Card NextSkill(Unit caster)
    {
        if (skillPattern is Sequential seq) // ÆÐÅÏ
        {
            EnemyCardData skill = seq.Skilles[index];
            if (seq.FixedIndex != -1)
                index = (index + 1) % seq.Skilles.Count;
            Card card = new Card();
            card.Init(skill, caster);
            return card;
        }
        else if (skillPattern is Weighted weighted) // ·£´ý
        {
            int totalWeight = 0;
            foreach (var skill in weighted.Skilles)
                totalWeight += (int)skill.Weight;

            int value = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var skill in weighted.Skilles)
            {
                currentWeight += (int)skill.Weight;
                if (value < currentWeight)
                {
                    Card card = new Card();
                    card.Init(skill.Skill, caster);

                    return card;
                }
            }
        }

        return null;
    }

    public virtual void Reset()
    {
        index = 0;
    }
}
