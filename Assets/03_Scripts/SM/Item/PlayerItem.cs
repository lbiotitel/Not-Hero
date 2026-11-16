using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static ItemData;

public class PlayerItem : MonoBehaviour
{
    Hero player;

    //기본아이템
    public ItemData idleWeapon;
    public ItemData idleArmor;


    // 0 = Weapon
    // 1 = Armor
    public List<ItemData> equippedItems;

    void Awake()
    {
        player = GetComponent<Hero>();

        equippedItems = new List<ItemData>()
        {
            idleWeapon, idleArmor
        };
    }

    public void EquipItem(ItemData newItem)
    {
        if (newItem.equipPart == EquipPart.Weapon)
        {
            equippedItems[0] = newItem;
        }
        else if (newItem.equipPart == EquipPart.Armor)
        {
            equippedItems[1] = newItem;
        }
        else return;
    }
}