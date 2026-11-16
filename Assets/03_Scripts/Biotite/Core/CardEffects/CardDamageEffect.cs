using DG.Tweening;
using System;
using UnityEngine;


[Serializable]
public class CardDamageEffect : CardEffect
{
    [SerializeField] private int skillDamage;
    [SerializeField] private int atkCount = 1;

    public override void Excute(Unit caster, Unit target, GameObject skillEffect = null, string soundSfx = null)
    {
        for (int i = 0; i < atkCount; i++)
        {
            DOVirtual.DelayedCall(Defines.TimeKey.CARD_DAMAGE_EFFECT_DELAY * i,
                () =>
                {
                    DoDamage(caster, target, skillEffect, soundSfx);
                    Managers.Sound.PlaySound(soundSfx, false, ESoundType.Sfx);
                });
        }
    }

    private void DoDamage(Unit caster, Unit target, GameObject skillEffect, string soundSfx)
    {
        if (target == null || target.UnitStat.IsDie) return;

        float damage = (caster.UnitStat.STR + skillDamage);

        float currentArmor = target.UnitStat.DEF;
        float armorDamage = Mathf.Min(damage, currentArmor);
        target.UnitStat.DEF -= armorDamage;

        if (target.UnitStat.DEF <= 0)
        {
            target.UnitStat.DEF = 0;
            target.UnitUI.ShowIcon(false, IconType.Block);
        }
        else
        {
            target.UnitUI.SetBlock(target.UnitStat.DEF);
        }

        float finalDamage = Mathf.FloorToInt(damage) - currentArmor;

        if (finalDamage > 0)
        {
            caster.DDamage(finalDamage, Vector3.right, target);
        }

        if (skillEffect != null)
        {
            Managers.Pooling.GetObject(skillEffect, target.transform.position);
        }
    }


    public override string BuildDescription(Unit caster, string description)
    {
        if (caster == null)
        {
            Debug.LogError("제대로 된 cater 가 아닙니다");
            return string.Empty;
        }

        float damage = (caster.UnitStat.STR + skillDamage);
        int finalDamage = Mathf.FloorToInt(damage);

        string res = DescBuilder.BuildDescription(effectDescription, new DescBuilder.EDescKey[]
        { DescBuilder.EDescKey.Damage, DescBuilder.EDescKey.AtkCount },
        new object[] { finalDamage, atkCount });


        return res;
    }
}
