using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//카드 기능의 부모입니다.
[Serializable]
public abstract class CardEffect
{
    [SerializeField,TextArea] protected string effectDescription;
    public abstract void Excute(Unit caster, Unit target,GameObject skillEffect = null, string soundSfx = null);
    public abstract string BuildDescription(Unit caster, string description);
}
