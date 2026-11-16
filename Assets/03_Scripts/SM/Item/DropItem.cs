using System;
using UnityEngine;

[Serializable]
public class DropItem
{
    public ItemData item;
    public float chance;

    public void Print()
    {
        Debug.Log("이름: " + item.itemName);
        Debug.Log("설명: " + item);
        Debug.Log("최대체력: " + item.itemMaxHp);
        Debug.Log("힘: " + item.itemStr);
        Debug.Log("가격: " + item.price);
    }
}
