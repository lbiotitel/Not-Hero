using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventNodeEntry
{
    public string label;
    public EEventNodeType nodeType;
    public int weight; 
}

public enum EEventNodeType { Trap, Treasure, Altar, Battle }

[CreateAssetMenu(menuName = "GameData/EventNodeTable")]
public class EventNodeSystem : ScriptableObject
{
    [SerializeField] private List<EventNodeEntry> entries = new List<EventNodeEntry>();
    private int totalWeight;
    private EEventNodeType lastPicked = (EEventNodeType)(-1);

    private void OnEnable() => ReCalcWeight();
    private void OnValidate() => ReCalcWeight();

    private void ReCalcWeight()
    {
        totalWeight = 0;
        foreach (var e in entries) { totalWeight += e.weight; }
    }

    public EEventNodeType Pick()
    {
        if (entries.Count == 0 || totalWeight == 0) return EEventNodeType.Treasure;

        int adjustedWeight = 0;
        foreach (var e in entries)
        {
            if (e.nodeType == lastPicked && entries.Count > 1) continue;
            adjustedWeight += e.weight;
        }
        int roll = UnityEngine.Random.Range(0, adjustedWeight);
        int acc  = 0;

        foreach (var e in entries)
        {
            if (e.nodeType == lastPicked && entries.Count > 1) continue;
            
            acc += e.weight;
            if (roll < acc)
            {
                lastPicked = e.nodeType;
                return e.nodeType;
            }
        }
        
        lastPicked = entries[entries.Count - 1].nodeType;
        return lastPicked;
    }
}
