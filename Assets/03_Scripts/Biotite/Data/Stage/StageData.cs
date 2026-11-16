using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "GameData/StageData")]

public class StageData : ScriptableObject
{
    [Serializable]
    public struct EnemyDataList
    {
        public List<string> enemyNameList;
    }

    [SerializeField] private string stageName;
    [SerializeField] private GameObject envObject;
    [SerializeField] private string bossName;
    [SerializeField] private List<EnemyDataList> stageEnemyList;



    public string StageName => stageName;
    public GameObject EnvObject => envObject;
    public string BossName => bossName; 
    public List<EnemyDataList> StageEnemyList => stageEnemyList;
}
