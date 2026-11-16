using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DropSystem : MonoBehaviour
{
    [SerializeField] List<RegionDropTable> regions;
    private int level = 0; // 게임매니저에서 가져올 예정

    private void Start()
    {
        for(int i = 0; i < 3; i++)
            GetDrop();
    }

    public DropItem GetDrop()
    {
        RegionDropTable table = regions[level];

        float totalWeight = 0f;

        foreach(var item in table.dropItems)
        {
            totalWeight += item.chance;
        }

        float randPoint = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var item in table.dropItems)
        {
            currentWeight += item.chance;

            if (randPoint != 0 && randPoint <= currentWeight)
            {
                item.Print();
                return item;
            }
        }

        return null;
    }
}