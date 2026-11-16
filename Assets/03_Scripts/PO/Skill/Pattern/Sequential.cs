using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Sequential")]
public class Sequential : SkillPattern
{
    [SerializeField] int fixedIndex = -1; // -1이면 고정 없음
    [SerializeField] List<EnemyCardData> skills;

    public int FixedIndex => fixedIndex;
    public List<EnemyCardData> Skilles => skills;
}
