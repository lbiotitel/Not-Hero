using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/ItemDatabase")]
public class ItemList : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();

    public ItemData FindItem(string itemName)
    {
        foreach (ItemData item in items)
        {

            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }
}