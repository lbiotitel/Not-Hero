using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : Unit, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] EnemyData data;
    [SerializeField] IntentUI intentUI;
    [SerializeField] private TargetLine[] targetLines;

    [SerializeField] SkillPatternEntry skillPatternEntry;
    Queue<EnemyAction> actions;

    private Card pendingSkill;
    private List<Unit> currentTargetList;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetData(UnitData data, UnitStat stat)
    {
        base.SetData(data, stat);
        this.data = (EnemyData)data;
        skillPatternEntry = new SkillPatternEntry(this.data.Pattern);
    }
    

    public void SetData(ItemData item)
    {
        unitStat.MAXHP += item.itemMaxHp;
        unitUI.SetHP(unitStat.Hp, unitStat.MAXHP);
        unitStat.STR += item.itemStr;
    }

    // ── 스킬 로직 ────────────────────────────────────────────────────────────

    public void Idle()
    {
        CheckConditions();
        SkillAdd();
    }

    public void SkillAdd()
    {
        if(data.Pattern == null || unitStat.IsDie)
        {
            return;
        }


        pendingSkill = skillPatternEntry.NextSkill(this);

        intentUI.SetIcon(pendingSkill.CardData.CardIcon, 1);

        currentTargetList = GetPotentialTargets();
        Debug.Log($"{gameObject.name}[SkillAdd] 다음 스킬 준비 : {pendingSkill.CardData.name}");
    }

    public void Skill()
    {
        if(pendingSkill == null || unitStat.IsDie) return;

        pendingSkill.DoCardLogic(currentTargetList);

        intentUI.SetIcon(null, 0);
        Animator.Play("Attack");
    }

    private void CheckConditions()
    {
        if(data.Conditions == null || data.Conditions.Count == 0)
        {
            return;
        }

        foreach(var condition in data.Conditions)
        {
            bool result = condition.Evaluate(unitStat.Hp, unitStat.MAXHP, 0, null);
            Debug.Log($"[Condition: {condition.name}] 결과: {result}");
        }
    }

    public List<Unit> GetPotentialTargets()
    {
        List<Unit> targets = new List<Unit>();

        ECardType cardType = pendingSkill.CardData.CardType;

        if(cardType == ECardType.Skill)
        {
            //플레이어 전체 가져옴
            EnemySkillCardData skillCardData = pendingSkill.CardData as EnemySkillCardData;
            ESkillRange skillRange = skillCardData.SkillRange;

            if(skillRange == ESkillRange.Self)
            {
                targets.Add(this);

            }
            else if(skillRange == ESkillRange.Party)
            {
                targets.AddRange(GetAliveEnemies());
            }

            this.Animator.Play("Skill");
        }
        else if(cardType == ECardType.Attack)
        {
            EnemyAttackCardData atkCardData = pendingSkill.CardData as EnemyAttackCardData;
            EAtkRange atkRange = atkCardData.AtkRange;

            if(atkRange == EAtkRange.Range)
            {
                //타겟은 여러명
                targets.AddRange(GetAliveHeroes());
            }
            else if(atkRange == EAtkRange.Single)
            {
                Unit hero = RandomTargeting();
                if(hero != null)
                {
                    targets.Add(hero);
                }
            }

        }
        return targets;

    }
    public List<Hero> GetAliveHeroes()
    {
        return Managers.Battle.Heroes.Where(u => u != null && !u.UnitStat.IsDie).ToList();
    }

    public List<Enemy> GetAliveEnemies()
    {
        return Managers.Battle.Enemies.Where(u => u != null && !u.unitStat.IsDie).ToList();
    }

    // Random Targeting
    private Unit RandomTargeting()
    {
        List<Unit> targets = new List<Unit>();
        targets.AddRange(GetAliveHeroes());

        if(targets == null || targets.Count == 0)
            return null;

        Unit target = targets[Random.Range(0, targets.Count)];

        return target; 
    }

    protected override void Died()
    {
        base.Died();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DrawTargetLines();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var targetLine in targetLines) 
            targetLine.Hide();
    }
    private void DrawTargetLines()
    {
        if (UnitStat.IsDie) return;

        int lineIndex = 0;
        foreach (var unit in currentTargetList)
        {
            if (lineIndex >= targetLines.Length) break;
            targetLines[lineIndex++].Show(transform, unit.transform);
        }
    }
}