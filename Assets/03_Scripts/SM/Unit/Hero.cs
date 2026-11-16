using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    public ItemList list;
    HeroData data;

    public PlayerItem playerItem;
    public ItemManager itemManager;
    public EClassType classType;

    [SerializeField] SpriteRenderer spriteRender;
    [SerializeField] Enemy testTarget;

    protected override void Awake()
    {
        base.Awake();
        itemManager = FindObjectOfType<ItemManager>();
    }


    public override void SetData(UnitData data, UnitStat stat)
    {
        base.SetData(data, stat);
        this.data = (HeroData)data;

        classType = this.data.ClassData.CHRClass;
    }

    public void ChangeStateToItem(ItemData item)
    {
        Debug.Log("얻은 아이템 : " + item);

        unitStat.STR += item.itemStr;
        unitStat.MAXHP += item.itemMaxHp;
        unitStat.Hp += item.itemMaxHp;

        UnitUI unitUI = this.unitUI;
        unitUI.SetHP(unitStat.Hp, unitStat.MAXHP);

        Debug.Log(item.itemStr + " " + item.itemMaxHp);
        Debug.Log("STR + " + unitStat.STR + " Hp + " + unitStat.Hp);
    }

    public void ChangeItemStat(ItemData oldItem, ItemData newItem)
    {
        float finalStr = unitStat.STR;
        float finalMaxHp = unitStat.Hp;
        float finalHp = unitStat.MAXHP;

        if (oldItem != null)
        {
            finalStr -= oldItem.itemStr;
            finalMaxHp -= oldItem.itemMaxHp;
            finalHp -= oldItem.itemMaxHp;
        }

        finalStr += newItem.itemStr;
        finalMaxHp += newItem.itemMaxHp;
        finalHp += newItem.itemMaxHp;

        Debug.Log($"STR : {finalStr}  HP : {finalMaxHp}");

        unitStat.STR = finalStr;
        unitStat.MAXHP = finalMaxHp;
    }

    public void ClickButton(List<ItemData> item)
    {
        System.Random rand = new System.Random();
        int num = rand.Next(0, item.Count);

        Debug.Log("랜덤 index : " + num);

        Debug.Log("아이템 데이터 : " + item[num]);

        Debug.Log("아이템 이름 : " + item[num].itemName);

        ItemData newItem = item[num];
        playerItem.EquipItem(newItem);
    }

    
    protected override void Died()
    {
        base.Died();
    }

    public Sprite GetSprite()
    {
        if (data != null && data.Sprite != null)
            return data.Sprite;

        return spriteRender != null ? spriteRender.sprite : null;
    }
}

