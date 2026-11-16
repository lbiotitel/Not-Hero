using DG.Tweening;
using System.Collections;
using UnityEngine;

// 유닛에 대한 클래스입니다.
public class Unit : MonoBehaviour
{
    [SerializeField] protected GameObject targetObject;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] Animator animator;
    
    public Animator Animator => animator;

    [SerializeField] protected string Name;
    [SerializeField] protected UnitStat unitStat;

    protected UnitData unitData;

    public string UName => Name;

    public UnitStat UnitStat => unitStat;

    protected UnitUI unitUI;
    public UnitUI UnitUI => unitUI;

    [SerializeField] SpriteRenderer sprite;

    // UnitAction
    public event System.Action<IconType, float, float> changeStat;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        unitUI = GetComponentInChildren<UnitUI>();
        sprite = GetComponent<SpriteRenderer>();
    }

    protected void OnEnable()
    {
        changeStat += unitUI.SetUI;
    }

    protected void OnDisable()
    {
        changeStat -= unitUI.SetUI;
    }

    public virtual void SetData(UnitData data, UnitStat stat)
    {
        gameObject.name = data.UName;

        unitData = data;

        unitStat = stat;

        changeStat?.Invoke(IconType.HpBar, stat.Hp, stat.MAXHP);

        sprite.sprite = data.Sprite;

        animator.runtimeAnimatorController = data.AnimControl;
    }

    public virtual void DDamage(float cardDamage, Vector3 direction, Unit target)
    {
        if(target.unitStat.IsDie) return;

        // 사전 계산
        float hp = target.unitStat.Hp;
        bool isDie = hp - cardDamage <= 0 ? true : false;

        target.unitStat.Hp -= cardDamage;

        target.changeStat?.Invoke(IconType.HpBar, target.unitStat.Hp, target.unitStat.MAXHP);

        DamageText damageText = Managers.Pooling.GetObject<DamageText>(damageTextPrefab, target.transform.position);

        if(cardDamage <= 0)
            damageText.Show(cardDamage, EDamageType.Block);
        else
            damageText.Show(cardDamage, EDamageType.Normal);

        Anime(direction);
        target.animator.Play("Hit");

        if(isDie)
        {
            target.unitStat.IsDie = true;
            target.unitStat.Hp = 0;
            StartCoroutine(DieAnime(target));

            return;
        }
    }

    protected virtual void Anime(Vector3 direction)
    {
        transform.DOComplete();
        transform.DOPunchPosition(direction, 0.2f);
    }

    private IEnumerator DieAnime(Unit unit)
    {
        unit.transform.DOKill();
        unit.Animator.SetTrigger("IsDie");

        yield return new WaitForSeconds(0.1f);
        Die(unit);
    }

    public void ShowTargetUI()
    {
        targetObject.SetActive(true);
    }

    public void HideTargetUI()
    {
        targetObject.SetActive(false);
    }

    public void Die(Unit unit)
    {
        unit.unitUI.ShowIcon(false, IconType.HpBar);

        if(unit is Enemy) // 적이 죽었을 때
        {
            unit.unitUI.ShowIcon(false, IconType.Intent);
            Managers.Battle.IsGameWin();
        }
        else if(unit is Hero) // 플레이어가 죽었을 때
        {
            Managers.Battle.IsGameLose();
        }
    }

    // Unit이 죽었을 때 오브젝트 삭제
    protected virtual void Died()
    {
        Die(this);

        this.gameObject.SetActive(false);
    }
}
