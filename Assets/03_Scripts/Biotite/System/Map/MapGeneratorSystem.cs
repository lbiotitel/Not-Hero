using System;
using System.Collections.Generic;
using UnityEngine;





// 맵 랜덤생성 스크립트
public class MapGeneratorSystem : MonoBehaviour
{
    [Serializable]
    struct MapNodeWeight
    {
        public EMapNodeType nodeType;
        public int weight;
    }

    //가중치 랜덤
    [Header("Node Weight")]
    [SerializeField] private List<MapNodeWeight> mapNodeWeights = new List<MapNodeWeight>();
    private int totalWeight; //가중치 합

    [Header("System Properties")]
    [SerializeField] private int mapHeight;
    [SerializeField] private int mapWidth;
    [SerializeField] private int startNodeCount;

    private MapNode[,] map;
    private List<MapNode> startNodeList = new List<MapNode>(); //맨 처음 배틀 노드
    public MapNode StartNode { get; private set; } = null; //시작 노드




    //맵 생성로직을 순차로 실행
    public MapNode[,] GenerateMap()
    {
        map = new MapNode[mapHeight, mapWidth];
        startNodeList.Clear();
        StartNode = null;
        
        CalcTotalWeight();
        InitAllMapNode();
        SelectStartNode();
        GeneratePath();
        InitBossMapNode();
        InitRestNode();
        InitTreasureNode();
        InitAllMapNodeType();

        return map;
    }

    //가중치 계산
    private void CalcTotalWeight()
    {
        totalWeight = 0;
        foreach (var node in mapNodeWeights)
            totalWeight += node.weight;
    }


    //길 연결
    private void GeneratePath()
    {
        foreach (var node in startNodeList)
        {
            int currentX = node.mapPos.x;
            for (int y = mapHeight - 1; y >= 2; y--)
            {
                currentX = SetUpConnection(y, currentX);
            }
        }
    }


    //연결
    private int SetUpConnection(int y, int x)
    {
        MapNode nextRoom = null;
        MapNode currentRoom = map[y, x];

        while (nextRoom == null || IsCrossExistingPath(y, x, nextRoom))
        {
            int randomX = Mathf.Clamp(UnityEngine.Random.Range(x - 1, x + 2), 0, mapWidth - 1);
            nextRoom = map[y - 1, randomX];
        }

        currentRoom.nextNodes.Add(nextRoom);

        return nextRoom.mapPos.x;
    }

    //교차 노드 체크
    private bool IsCrossExistingPath(int y, int x, MapNode room)
    {
        if (room == null)
            return false;

        MapNode leftRoom = null;
        MapNode rightRoom = null;
        
        //범위 체크
        if (x > 0) leftRoom = map[y, x - 1];
        if (x < mapWidth - 1) rightRoom = map[y, x + 1];

        if (rightRoom != null && room.mapPos.x > x)
        {
            foreach (var nextRoom in rightRoom.nextNodes)
            {
                if (nextRoom.mapPos.x < room.mapPos.x)
                    return true;
            }
        }

        if (leftRoom != null && room.mapPos.x < x)
        {
            foreach (var nextRoom in leftRoom.nextNodes)
            {
                if (nextRoom.mapPos.x > room.mapPos.x)
                    return true;
            }
        }


        return false;
    }


    //모든 노드 초기화
    private void InitAllMapNode()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[y, x] = new MapNode();
                map[y, x].mapPos = new Vector2Int(x, y);
                map[y, x].mapNodeType = EMapNodeType.None;
                map[y, x].nextNodes = new List<MapNode>();
            }
        }
    }


    //처음 시작 부분 랜덤으로 선택
    private void SelectStartNode()
    {
        HashSet<int> sel = new HashSet<int>();

        while (sel.Count != startNodeCount)
        {
            int randomStartNode = UnityEngine.Random.Range(0, mapWidth);
            sel.Add(randomStartNode);
        }

        StartNode = new MapNode();
        StartNode.nextNodes = new List<MapNode>();

        foreach (var s in sel)
        {
            map[mapHeight - 1, s].mapNodeType = EMapNodeType.Battle;
            startNodeList.Add(map[mapHeight - 1, s]);
            StartNode.nextNodes.Add(map[mapHeight - 1, s]);
        }
    }


    //보스룸 설정
    private void InitBossMapNode()
    {
        int mid = mapWidth / 2;
        int lastH = 2;
        map[0, mid].mapNodeType = EMapNodeType.Boss;

        for (int x = 0; x < mapWidth; x++)
        {
            if (map[lastH, x].nextNodes.Count == 0) continue;
            map[lastH, x].nextNodes.Clear();
            map[lastH, x].nextNodes.Add(map[0, mid]);
        }
    }

    //중간에 보물방 생성
    private void InitTreasureNode()
    {
        int mid = mapHeight / 2;

        for (int x = 0; x < mapWidth; x++)
        {
            if (map[mid, x].nextNodes.Count > 0)
            {
                map[mid, x].mapNodeType = EMapNodeType.Treasure;
            }
        }
    }


    private void InitRestNode()
    {
        int targetY = 2;
        for (int x = 0; x < mapWidth; x++)
        {
            if (map[targetY, x].nextNodes.Count == 0) continue;
            map[targetY, x].mapNodeType = EMapNodeType.Rest;
        }
    }



    //비어 있는 노드들의 타입 설정
    private void InitAllMapNodeType()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map[y, x].mapNodeType == EMapNodeType.None &&
                    map[y, x].nextNodes.Count > 0)
                {
                    map[y, x].mapNodeType = GetRandomMapNodeTypeByWeight();
                }
            }
        }
    }


    //가중치 랜덤으로 타입가져오기
    private EMapNodeType GetRandomMapNodeTypeByWeight()
    {
        int randomType = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < mapNodeWeights.Count; i++)
        {
            if (randomType < mapNodeWeights[i].weight)
            {
                return mapNodeWeights[i].nodeType;
            }
            randomType -= mapNodeWeights[i].weight;
        }

        return EMapNodeType.None;
    }

}
