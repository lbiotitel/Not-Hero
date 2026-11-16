using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour, IEventListener
{
    [Header("Map Properties")]
    [SerializeField] private float mapWidth;
    [SerializeField] private float mapHeight;

    [Header("UI Properties")]
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private MapNodeUI mapNodeUIPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float mapNodeRandomScale = 0.7f;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Button mapExitButton;
    [SerializeField] private GameObject clickBlocker;



    private Dictionary<MapNode, MapNodeUI> mapNodeToUIs = new Dictionary<MapNode, MapNodeUI>();
    private List<GameObject> lineNodes = new List<GameObject>();
    private MapNode[,] mapData;

    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        mapExitButton.onClick.AddListener(() => Hide());
    }


    public void Init()
    {
        BindEvent();
    }

    private void BindEvent()
    {
        Managers.Event.AddListener(EEventType.OnNodeCompleted, this);
    }

    private void OnDestroy()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnNodeCompleted, this);
        }
    }

    public void DrawMapUI(MapNode[,] mapData, string stageName)
    {
        this.mapData = mapData;
        stageNameText.text = stageName;

        ClearMapNodes();
        DrawMapUI();
        Show();
        DrawConnectedLine();
        Hide();
    }



    //연결된 연결 라인 그리기
    private void DrawConnectedLine()
    {
        int mapDataHeight = mapData.GetLength(0);
        int mapDataWidth = mapData.GetLength(1);

        for (int y = 0; y < mapDataHeight; y++)
        {
            for (int x = 0; x < mapDataWidth; x++)
            {
                MapNode curMapNode = mapData[y, x];
                if (mapNodeToUIs.TryGetValue(curMapNode, out MapNodeUI curMapNodeUI))
                {
                    RectTransform curMapNodeRect = curMapNodeUI.GetComponent<RectTransform>();
                    //연결된 노드 다 라인 그리기
                    foreach (var nextNode in curMapNode.nextNodes)
                    {

                        if (mapNodeToUIs.TryGetValue(nextNode, out MapNodeUI nextMapNodeUI))
                        {
                            RectTransform nextMapNodeRect = nextMapNodeUI.GetComponent<RectTransform>();
                            ConnectLine(curMapNodeRect.transform.position, nextMapNodeRect.transform.position);
                        }
                    }
                }
            }
        }
    }

    //두사이에 라인을 그림
    private void ConnectLine(Vector2 startPos, Vector2 endPos)
    {
        GameObject lineObj = Instantiate(linePrefab, contentTransform);
        lineNodes.Add(lineObj);

        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        Vector3 targetPos = startPos + (endPos - startPos) / 2f;
        float dis = (endPos - startPos).magnitude;
        float margin = 0.2f;
        float marginSize = dis * margin;
        float angle = Mathf.Atan2((endPos - startPos).y, (endPos - startPos).x) * Mathf.Rad2Deg;

        dis -= marginSize * 2f;
        lineRect.position = targetPos;
        lineRect.sizeDelta = new Vector2(dis, lineRect.sizeDelta.y);
        lineRect.Rotate(0, 0, angle);
    }


    //데이터 대로 맵 그리기
    private void DrawMapUI()
    {
        int mapDataHeight = mapData.GetLength(0);
        int mapDataWidth = mapData.GetLength(1);

        float xOffset = mapWidth / mapDataWidth;
        float yOffset = mapHeight / mapDataHeight;

        float startX = -(mapWidth / 2f) + (xOffset / 2f);
        float startY = -(mapHeight / 2f) + (yOffset / 2f);

        for (int y = mapDataHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapDataWidth; x++)
            {
                if (mapData[y, x].mapNodeType != EMapNodeType.None)
                {
                    MapNodeUI mapNode = Instantiate(mapNodeUIPrefab);

                    RectTransform nodeRect = mapNode.GetComponent<RectTransform>();
                    nodeRect.SetParent(contentTransform.transform);

                    float posX = startX + (x * xOffset);
                    float posY = startY + (y * yOffset);

                    nodeRect.anchoredPosition = new Vector2(posX, posY) + Random.insideUnitCircle * mapNodeRandomScale;

                    nodeRect.localScale = Vector3.one;

                    mapNode.Init(mapData[y, x]);
                    mapNode.OnClickedAction += OnClikedMapNodeUIHandler;

                    mapNodeToUIs[mapData[y, x]] = mapNode;
                }
            }
        }
    }

    //모든 노드들 삭제
    private void ClearMapNodes()
    {
        foreach (var node in mapNodeToUIs)
        {
            if (node.Value != null)
            {
                node.Value.OnClickedAction -= OnClikedMapNodeUIHandler;
                Destroy(node.Value.gameObject);
            }
        }
        mapNodeToUIs.Clear();

        foreach (var line in lineNodes)
        {
            if (line != null)
            {
                Destroy(line);
            }
        }
        lineNodes.Clear();
    }

    private void CanGoNodeAnimation(MapNode mapNode, bool flag)
    {
        foreach (var nextNode in mapNode.nextNodes)
        {
            if (mapNodeToUIs.TryGetValue(nextNode, out MapNodeUI nextMapNodeUI))
            {
                nextMapNodeUI.CanGoNodeAnimation(flag);
            }
        }
    }


    //특정 노드 클릭 핸들러 
    private void OnClikedMapNodeUIHandler(MapNodeUI mapNodeUI)
    {
        MapNode mapNode = mapNodeUI.MapNode;
        MapNode beforeNode = Managers.Stage.CurrentMapNode;
        CanGoNodeAnimation(beforeNode, false);
        Managers.Stage.GoSelectedNode(mapNode);
    }

    //애니메이션 끝나면 제어권을 플레이어에게 다시넘김
    public void MapAnimationFinishTrigger()
    {
        if (animator != null)
            animator.enabled = false;
        clickBlocker.SetActive(false);
    }

    public void Show(bool firstShow = false)
    {
        Managers.Sound.PlaySound(Defines.SoundKey.MAP_OPEN_SFX, false, ESoundType.Sfx);
        MapNode currentMapNode = Managers.Stage.CurrentMapNode;
        CanGoNodeAnimation(currentMapNode, true);

        if (gameObject.activeSelf) return;

        if (animator != null)
            animator.enabled = true;

        clickBlocker.SetActive(true);
        gameObject.SetActive(true);

        if (firstShow) animator.SetTrigger("Intro");
        else animator.SetTrigger("Show");
    }

    public void Hide() => gameObject.SetActive(false);



    public void OnEvent(EEventType eventType, object sender, object param = null)
    {

        //노드 클리어시 나타냄
        if (eventType == EEventType.OnNodeCompleted)
        {
            Show();
        }
    }
}
