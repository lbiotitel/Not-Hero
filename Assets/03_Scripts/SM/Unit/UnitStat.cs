using UnityEngine;

[System.Serializable]
public class UnitStat
{
    [SerializeField] float str;
    [SerializeField] float def;
    [SerializeField] float maxHp;
    [SerializeField] private float hp;
    [SerializeField] bool isDie;

    public UnitStat(float str, float def, float maxHp)
    {
        this.str = str;
        this.def = def;
        this.maxHp = maxHp;
        this.hp = maxHp;
        isDie = false;
    }

    public float STR { get => str; set => str = value; }
    public float DEF { get => def; set => def = value; }
    public float MAXHP { get => maxHp; set => maxHp = value; }
    public float Hp { get => hp; set => hp = value; }
    public bool IsDie { get => isDie; set => isDie = value; }

    // 상태이상
    public enum State
    {
        fatigue,
        bleeding,
        poison,
        vulnerable
    }

    public void ModifyHp(float amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHp);
    }
}
