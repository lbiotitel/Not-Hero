using System;
using UnityEngine;


[Serializable]
public class DrawEffect : CardEffect
{
    [SerializeField] private int drawCount;

    public override void Excute(Unit caster, Unit target, GameObject skillEffect = null, string soundSfx = null)
    {
        Managers.Battle.CardSystem.Draw(drawCount, true);
    }

    public override string BuildDescription(Unit caster, string description)
    {
        return DescBuilder.BuildDescription(effectDescription, 
            new DescBuilder.EDescKey[] { DescBuilder.EDescKey.DrawCount },
            new object[] { drawCount });
    }


}
