

//��� �������� ���� �մϴ�

//이벤트
public enum EEventType
{
    //Game State Event
    OnGameStart,
    OnGameOver, 
    OnGameWin,
    //Game State Event

    //Asset Changed Event
    OnAddNewCard, //새로운 카드 추가
    OnAddNewItem, //새로운 아이템 추가
    OnGoldValueChanged, // 골드 재화 변경
    OnEnergyCostChanged, //에너지 코스트 변경
    //Asset ChangedEvent

    //Map Event
    OnMapNodeChanged, //노드가 바뀌었을 때
    OnEnteredBattleNode, //전투 노드 진입시
    OnEnteredEventNode, //이벤트 진입시
    OnEnteredShopNode,//상점노드 진입시
    OnEnteredBossNode,//보스노드 진입시
    OnEnteredTreasureNode,//보물 노드 진입시
    OnNodeCompleted,//노드를 클리어 할시
    //Map Event
    
    //Battle Manager Event
    OnBattleStart,   //전투 시작
    OnPlayerTurn,   // 플레이어 턴
    OnEnemyTurn,    // 적의 턴
    OnBattleWin,    // 승리시
    BattleLose,     // 패배시
    //Battle Manager Event    
}


