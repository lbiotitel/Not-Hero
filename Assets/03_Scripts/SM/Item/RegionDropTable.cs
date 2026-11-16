using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/RegionDrop")]
[Serializable]
public class RegionDropTable : ScriptableObject
{
    public List<DropItem> dropItems;

    public RegionName regionName;

    public enum RegionName
    {
        None,
        B1,
        B2,
        B3
    }
}
