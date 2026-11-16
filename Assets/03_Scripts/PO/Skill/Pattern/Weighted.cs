using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Weighted")]
public class Weighted : SkillPattern
{
    [SerializeField] List<WeightEntry> skilles;
    public List<WeightEntry> Skilles => skilles;
}
