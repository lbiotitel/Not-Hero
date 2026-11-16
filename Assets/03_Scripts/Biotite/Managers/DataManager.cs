using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    private Dictionary<Type, Dictionary<string, UnityEngine.Object>> dataDict =
        new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();

    [SerializeField] private GradeProbability gradeProbability;
    public GradeProbability GradeProbability => gradeProbability;


    public void Init() => LoadAllResources();

    private void LoadAllResources()
    {
        LoadResource<CardData>(Defines.ResourcePathKey.CardDataPath);
        LoadResource<ItemData>(Defines.ResourcePathKey.ItemPath);
        LoadResource<StageData>(Defines.ResourcePathKey.StagePath);
        LoadResource<AudioClip>(Defines.ResourcePathKey.SoundDataPath);
        LoadResource<UnitData>(Defines.ResourcePathKey.UnitDataPath);
    }

    //리로스를 로드해서 등록합니다.
    public void LoadResource<T>(string path) where T : UnityEngine.Object
    {
        T[] dataList = Resources.LoadAll<T>(path);

        foreach (T data in dataList)
        {
            Type actualType = data.GetType();

            if (!dataDict.ContainsKey(actualType))
                dataDict[actualType] = new Dictionary<string, UnityEngine.Object>();

            string key = data.name;

            if (!dataDict[actualType].ContainsKey(key))
                dataDict[actualType].Add(key, data);
        }
    }


    //type 에 해당되는 name키의 데이터를 가져옵니다.
    public T GetData<T>(string name) where T : UnityEngine.Object
    {
        if (dataDict.TryGetValue(typeof(T), out var dict))
        {
            if (dict.TryGetValue(name, out var obj)) return obj as T;
        }

        foreach (var pair in dataDict)
        {
            if (typeof(T).IsAssignableFrom(pair.Key))
            {
                if (pair.Value.TryGetValue(name, out var obj)) return obj as T;
            }
        }
        return null;
    }


    //Type 에 해당되는 모든 데이터를 가져옵니다.
    public List<T> GetDataList<T>() where T : UnityEngine.Object
    {
        return dataDict
            .Where(pair => typeof(T).IsAssignableFrom(pair.Key))
            .SelectMany(pair => pair.Value.Values)
            .Cast<T>()
            .ToList();
    }

    public void Clear()
    {
        dataDict.Clear();
        Resources.UnloadUnusedAssets();
    }


}