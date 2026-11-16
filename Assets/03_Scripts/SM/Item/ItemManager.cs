using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemList itemDB;

    private void Start()
    {
        //ItemData item = GetItemData("æ‡≈ª¿⁄¿« ≥∞¿∫ ∞À");
        //itemUI.SetupUI(item);
    }

    public ItemData GetItemData(string itemName)
    {
        return itemDB.FindItem(itemName);
    }
}