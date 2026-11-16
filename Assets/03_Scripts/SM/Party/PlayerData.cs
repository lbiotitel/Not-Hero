using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public CHRClassData classType;

    public float hp;
    public float maxHp;
    public float str;

    public Sprite sprite;

    public void SavePlayerData(Unit unit, PlayerData data)
    {
        /*data.hp = unit.unitState.hp;
        data.maxHp = unit.unitState.maxHp;
        data.str = unit.unitState.str;*/
    }
}
