using System.Collections.Generic;
using UnityEngine;


//맵 생성 노드
public class MapNode
{
    public Vector2Int mapPos;
    public List<MapNode> nextNodes;
    public EMapNodeType mapNodeType;
}

public enum EMapNodeType
{
    None, //비어있음
    Battle, // 전투
    Event, //이벤트
    Shop, //상점
    Boss, //보스
    Treasure,//보물
    Rest //휴식
}