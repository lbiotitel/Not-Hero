using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Item")]
public class ItemData :ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public float  itemMaxHp;
    public int itemStr;
    public int price;

    public Sprite icon;

    public EquipPart equipPart;
    //public ERank rank;  //03-11 EGrade로 통합했습니다
    [SerializeField] private EGrade itemGrade;
    public FloorType floorType;
    public SourceType sourceType;
    public ItemType itemType;

    public EGrade ItemGrade => itemGrade;

    public enum EquipPart
    {
        Weapon,
        Armor
    }

    //public enum ERank
    //{
    //    Common,
    //    Rare,
    //    Epic,
    //    Boss,
    //    Idle
    //}
    public enum FloorType
    {
        None,
        B1,
        B2,
        B3
    }
    
    public enum SourceType
    {
        Chest,
        Shop,
        Floor
    }
    public enum ItemType 
    {
        Gold,
        Card,
        Equipment
    }
}