using UnityEngine;



//UI들의 순서초기화를 하거나 간단한 참조를 위해 모아놓은 곳입니다.
public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private BezierArrow bezierArrow;

    [field: SerializeField] public TopPanelUI TopPanelUI { get; private set; }
    [field: SerializeField] public ShopUI ShopUI { get; private set; }
    [field: SerializeField] public RestUI RestUI { get; private set; }
    [field: SerializeField] public TrapUI TrapUI { get; private set; }
    [field: SerializeField] public AltarUI AltarUI { get; private set; }
    [field: SerializeField] public TreasureUI TreasureUI { get; private set; }
    [field: SerializeField] public MapUI MapUI { get; private set; }
    [field: SerializeField] public CardListViewUI CardListViewUI { get; private set; }
    [field: SerializeField] public TransitionUI TransitionUI { get; private set; }
    [field: SerializeField] public CostUI CostUI { get; private set; }
    [field: SerializeField] public RewardUI RewardUI { get; private set; }
    [field: SerializeField] public GlobalToolTipUI ToolTipUI { get; private set; }
    [field: SerializeField] public TurnUI TurnUI{ get; private set; }
    [field: SerializeField] public Result result { get; private set; }



    //�ؿ� �ִ� ui�� init
    public void Init()
    {
        RestUI.Init();
        TrapUI.Init();
        AltarUI.Init();
        TreasureUI.Init();
        ShopUI.Init();
        CardListViewUI.Init();
        TopPanelUI.Init();
        ShopUI.HideUI();
        CardListViewUI.Hide();
        bezierArrow.gameObject.SetActive(false);
        RewardUI.Init();
        TurnUI.Init();
        result.Init();
    }


    public void ShowBazierArrow(bool active, Vector3 startPos = default) =>
        bezierArrow.ShowArrowActive(active, startPos);

    public void SetArrowColor(Color color) => bezierArrow.SetColor(color);

    public void Clear()
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }

        Destroy(parentTransform.gameObject);
        Destroy(gameObject);
    }
}
