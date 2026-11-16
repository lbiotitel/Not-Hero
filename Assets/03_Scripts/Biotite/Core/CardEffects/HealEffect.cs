using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class HealEffect : CardEffect
{
    public enum EHealTarget
    { 
        Caster,
        Target
    }

    [SerializeField] private float value;
    [SerializeField] private EHealTarget healTarget;

    public override void Excute(Unit caster, Unit target, GameObject skillEffect = null, string soundSfx = null)
    {
        if (healTarget == EHealTarget.Caster) Heal(caster);
        else Heal(target);

        Managers.Sound.PlaySound(soundSfx, false, ESoundType.Sfx);

        if (skillEffect != null)
        {
            Managers.Pooling.GetObject(skillEffect, target.transform.position);
        }
    }

    private void Heal(Unit target)
    {
        float maxHp = target.UnitStat.MAXHP;
        float healAmount = Mathf.Min(maxHp, target.UnitStat.Hp + value);
        target.UnitStat.Hp = healAmount;
        target.UnitUI.SetHP(target.UnitStat.Hp, target.UnitStat.MAXHP);
    }

    public override string BuildDescription(Unit caster, string description)
    {
        string res = DescBuilder.BuildDescription(effectDescription, 
            new DescBuilder.EDescKey[] { DescBuilder.EDescKey.Heal }, 
            new object[] { value });

        return res;
    }

}
