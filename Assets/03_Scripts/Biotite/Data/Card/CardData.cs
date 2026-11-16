using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardData : ScriptableObject, ICloneable
{
    [Header("Card Data Properties")]
    [SerializeField] protected string cardName;
    [SerializeField][TextArea] private string cardDesc;
    [SerializeField] protected Sprite cardIcon;
    [SerializeField] protected ECardType cardType;
    [SerializeField] protected ESkillType skillType;

    [Header("Effect")]
    [SerializeField] protected GameObject skillEffect;
    [SerializeField] protected string skillSfx;

    [Header("Card Effect Data")]
    [SerializeReference, SubclassSelector] List<CardEffect> cardEffects = new List<CardEffect>();

    public string CardName => cardName;
    public string CardDesc => cardDesc;
    public Sprite CardIcon => cardIcon;
    public ECardType CardType => cardType;
    public ESkillType SkillType => skillType;

    public List<CardEffect> CardEffects => cardEffects;
    public GameObject SkillEffect => skillEffect;
    public string SkillSfx => skillSfx;
    public object Clone() => Instantiate(this);


}

public enum ECardType
{
    Attack,
    Skill
}

public enum ESkillType
{
    SkillAttack,
    Buff
}