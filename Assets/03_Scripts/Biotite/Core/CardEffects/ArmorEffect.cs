using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArmorEffect : CardEffect
{
    public enum EArmorRange
    {
        caster,
        target
    }

    [SerializeField] private int value;
    [SerializeField] private EArmorRange skillRange;
    public override void Excute(Unit caster, Unit target, GameObject skillEffect = null, string soundSfx = null)
    {
        if (skillRange == EArmorRange.caster) GiveArmor(caster);
        else GiveArmor(target);
        Managers.Sound.PlaySound(soundSfx, false, ESoundType.Sfx);

        caster.Animator.Play("Skill");

        if (skillEffect != null)
        {
            Managers.Pooling.GetObject(skillEffect, target.transform.position);
        }
    }


    private void GiveArmor(Unit target)
    {
        target.UnitUI.ShowIcon(true, IconType.Block);
        target.UnitStat.DEF += value;
        target.UnitUI.SetBlock(target.UnitStat.DEF);
    }

    public override string BuildDescription(Unit caster, string description)
    {
        string res = DescBuilder.BuildDescription(effectDescription, new DescBuilder.EDescKey[]
        { DescBuilder.EDescKey.Armor
        }, new object[] { value });

        return res;
    }
}
