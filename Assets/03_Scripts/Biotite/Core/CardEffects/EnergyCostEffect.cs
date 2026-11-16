using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class EnergyCostEffect : CardEffect
{
    [SerializeField] private int value;

    public override void Excute(Unit caster, Unit target, GameObject skillEffect = null, string soundSfx = null)
    {

        Managers.Battle.PlayerCost += value;
        Managers.Sound.PlaySound(soundSfx, false, ESoundType.Sfx);
        if (skillEffect != null)
        {
            Managers.Pooling.GetObject(skillEffect, target.transform.position);
        }
    }

    public override string BuildDescription(Unit caster, string description)
    {
        return DescBuilder.BuildDescription(effectDescription,
         new DescBuilder.EDescKey[] { DescBuilder.EDescKey.Cost },
         new object[] { value });
    }
}
