using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour, IEventListener
{
    private MapGeneratorSystem mapGeneratorSystem;
    private StageData stageData;
    private GameObject envObject;

    public MapNode CurrentMapNode { get; private set; }
    public StageData StageData => stageData;

    private int currentFloor = 0;
    private bool canSelectNode = true;
    private int currentStageIndex = -1;

    [SerializeField] private EventNodeSystem eventNodeSystem;


    private Dictionary<EMapNodeType, Action> nodeLogicDict;

    public void Init()
    {
        mapGeneratorSystem = GetComponent<MapGeneratorSystem>();

        // 노드 로직 등록
        InitNodeLogics();

        Managers.Event.AddListener(EEventType.OnBattleWin, this);
        Managers.Event.AddListener(EEventType.OnNodeCompleted, this);
        Managers.UI.MapUI.Init();
    }

    private void InitNodeLogics()
    {
        nodeLogicDict = new Dictionary<EMapNodeType, Action>
        {
            { EMapNodeType.Rest,() => Managers.UI.RestUI.ShowUI() },
            { EMapNodeType.Shop,() =>
            ShowTransition(() => {
                Managers.Event.NotifyEvent(EEventType.OnEnteredShopNode, this);
                Managers.UI.ShopUI.ShowUI();
            }) },
            { EMapNodeType.Treasure,() => ShowTransition(() => Managers.UI.TreasureUI.ShowUI()) },
            { EMapNodeType.Battle,() => EnterBattleNode(false) },
            { EMapNodeType.Boss,() => EnterBattleNode(true) },
            { EMapNodeType.Event,() => DoEventNodeLogic() }
        };
    }


    private void ShowTransition(Action onComplete)
    {
        Managers.UI.TransitionUI.ShowUI(() =>
        {
            onComplete?.Invoke();
            Managers.UI.MapUI.Hide();
        });
    }

    private void EnterBattleNode(bool isBoss)
    {
        ShowTransition(() =>
        {
            List<string> enemies;
            if (isBoss)
            {
                Managers.Event.NotifyEvent(EEventType.OnEnteredBossNode, this);
                enemies = new List<string> { stageData.BossName };
            }
            else
            {
                int randIndex = Random.Range(0, stageData.StageEnemyList.Count);
                enemies = stageData.StageEnemyList[randIndex].enemyNameList;
            }
            Managers.Event.NotifyEvent(EEventType.OnEnteredBattleNode, this, enemies);
        });
    }

    private void DoEventNodeLogic()
    {
        Managers.Event.NotifyEvent(EEventType.OnEnteredEventNode, this);
        EEventNodeType eventType = eventNodeSystem.Pick();

        switch (eventType)
        {
            case EEventNodeType.Trap: Managers.UI.TrapUI.ShowUI(); break;
            case EEventNodeType.Altar: Managers.UI.AltarUI.ShowUI(); break;
            case EEventNodeType.Treasure: ShowTransition(() => Managers.UI.TreasureUI.ShowUI()); break;
            case EEventNodeType.Battle: EnterBattleNode(false); break;
        }
    }

    public void GoSelectedNode(MapNode nextNode)
    {
        if (!canSelectNode || !CheckCanGoNode(nextNode)) return;

        Managers.Event.NotifyEvent(EEventType.OnMapNodeChanged, this, ++currentFloor);
        canSelectNode = false;
        CurrentMapNode = nextNode;

        if (nodeLogicDict.TryGetValue(CurrentMapNode.mapNodeType, out var logic))
        {
            logic.Invoke();
        }
    }

    public bool CheckCanGoNode(MapNode nextNode)
    {
        if (CurrentMapNode == null) return true;
        foreach (var node in CurrentMapNode.nextNodes)
        {
            if (node == nextNode) return true;
        }
        return false;
    }

    public void StartStage()
    {
        EScene nextStageName = GetNextStageScene();
        currentStageIndex++;
        currentFloor = 0;
        canSelectNode = true;

        MapNode[,] mapData = mapGeneratorSystem.GenerateMap();
        CurrentMapNode = mapGeneratorSystem.StartNode;
        stageData = Managers.Data.GetData<StageData>(nextStageName.ToString());

        Managers.UI.MapUI.DrawMapUI(mapData, stageData.StageName);
        GenerateEnvObject();
    }

    public EScene GetNextStageScene()
    {
        int nextIndex = Mathf.Clamp(currentStageIndex + 1, 0, Defines.MapOrder.Length - 1);
        return Defines.MapOrder[nextIndex];
    }

    private void GenerateEnvObject()
    {
        if (envObject != null) Destroy(envObject);
        envObject = Instantiate(stageData.EnvObject, Vector3.zero, Quaternion.identity);
    }

    public void OnEvent(EEventType eventType, object sender, object param = null)
    {
        if (eventType == EEventType.OnBattleWin)
        {
            if (CurrentMapNode.mapNodeType == EMapNodeType.Boss && GetNextStageScene() == EScene.Last)
            {
                Managers.Game.GameWin(Managers.Battle.Heroes);
            }
        }

        if (eventType == EEventType.OnBattleWin || eventType == EEventType.OnNodeCompleted)
        {
            canSelectNode = true;
        }
    }

    private void OnDestroy()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnBattleWin, this);
            Managers.Event.RemoveListener(EEventType.OnNodeCompleted, this);
        }
    }

    public void Clear() { }
}