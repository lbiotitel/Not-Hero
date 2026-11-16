using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RewardUI : MonoBehaviour, IEventListener
{
    [SerializeField] private RewardResult rewardResult;
    [SerializeField] private TooltipSystem tooltipSystem;
    [SerializeField] private TooltipUI tooltipUI;

    [SerializeField] private RewardSystem rewardSystem;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject rewardMainPrefab;
    [SerializeField] private GameObject button;
    [SerializeField] private Image DOTween_Img;
    [SerializeField] private Image rewardItemIcon;
    [SerializeField] private Image rewardCardIcon;
    [SerializeField] private TMP_Text showGold;
    [SerializeField] private TMP_Text showItemName;
    [SerializeField] private TMP_Text showCardName;
    [SerializeField] private Button exitButton;
    [SerializeField] private VerticalLayoutGroup vertical;

    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private RectTransform panel;

    private Vector2 originalPos;
    private Vector2 originalScale;


    public Image RewardItemIcon => rewardItemIcon;
    public Image RewardCardIcon => rewardCardIcon;

    private void OnDisable()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnBattleWin, this);
            Managers.Event.RemoveListener(EEventType.OnGameWin, this);
        }
    }
    public void Init()
    {
        originalPos = panel.anchoredPosition;
        Managers.Event.AddListener(EEventType.OnBattleWin, this);
        Managers.Event.AddListener(EEventType.OnGameWin, this);
        HideUI();
        exitButton.onClick.AddListener(OnClickExitButton);
        goldPrefab.GetComponent<Button>().onClick.AddListener(OnClickGoldReward);
        itemPrefab.GetComponent<Button>().onClick.AddListener(OnClickItemReward);
        cardPrefab.GetComponent<Button>().onClick.AddListener(OnClickCardReward);
        rewardSystem = new RewardSystem();
        rewardResult = new RewardResult();

        originalScale = goldPrefab.transform.localScale;
    }

    public void ShowUI()
    {
        panel.anchoredPosition = originalPos + new Vector2(0, -800);
        panel.DOAnchorPos(originalPos, 0.5f).SetEase(Ease.OutCubic);
        DOTween_Img.DOFade(0.99f, 0.7f);

        rewardMainPrefab.SetActive(true);
        goldPrefab.SetActive(true);

        if (rewardResult.item != null)
            itemPrefab.SetActive(true);
        else itemPrefab.SetActive(false);

        if (rewardResult.card != null)
            cardPrefab.SetActive(true);
        else cardPrefab.SetActive(false);

        button.SetActive(true);
    }

    public void HideUI()
    {
        rewardMainPrefab.SetActive(false);
        goldPrefab.SetActive(false);
        itemPrefab.SetActive(false);
        cardPrefab.SetActive(false);
        button.SetActive(false);
        //itemTooltip.SetActive(false);
        //cardTooltip.SetActive(false);

        DOTween_Img.DOKill();
        DOTween_Img.color = new Color(0, 0, 0, 0);
    }

    public void OnEvent(EEventType eventType, object sender, object param)
    {

        if (eventType == EEventType.OnGameWin)
        {
            HideUI();
        }

        if (eventType == EEventType.OnBattleWin)
        {
            rewardPrefab.SetActive(true);
            /*string floorName = (string)param;
            int floor;

            switch (floorName)
            {
                case "F0" :
                    floor = 1;
                    break;

                case "F1":
                    floor = 2;
                    break;

                case "F2":
                    floor = 3;
                    break;

                case "F3": 
                    floor = 4;
                    break;

                default:
                     floor = -1;
                    break;
            }
*/
            ShowReward();

        }
    }

    private int Stage(string floorName)
    {
        int floor;

        switch (floorName)
        {
            case "F0":
                floor = 1;
                break;

            case "F1":
                floor = 2;
                break;

            case "F2":
                floor = 3;
                break;

            case "F3":
                floor = 4;
                break;

            default:
                floor = -1;
                break;
        }

        return floor;
    }

    private void ShowReward()
    {
        string stageName = Managers.Stage.StageData.name;

        int floor = Stage(stageName);
        Debug.Log(stageName);
        Debug.Log(floor == -1 ? "��������" : "");

        rewardResult = rewardSystem.GenerateReward(floor);

        showGold.text = rewardResult.gold.ToString();

        if (rewardResult.item != null)
        {
            showItemName.text = rewardResult.item.itemName;
            rewardItemIcon.sprite = rewardResult.item.icon;

            itemPrefab.GetComponent<TooltipUI>().SetItem(rewardResult.item);
        }
        else
        {
            itemPrefab.SetActive(false);
        }

        if (rewardResult.card != null)
        {
            showCardName.text = rewardResult.card.CardName;
            rewardCardIcon.sprite = rewardResult.card.CardIcon;

            cardPrefab.GetComponent<TooltipUI>().SetCard(rewardResult.card);

        }
        else
        {
            cardPrefab.SetActive(false);
        }

        ShowUI();
    }

    public void OnClickGoldReward()
    {
        Managers.PlayerStat.AddGold(rewardResult.gold);
        Managers.Sound.PlaySound(Defines.SoundKey.GOLD_SFX, false, ESoundType.Sfx);

        goldPrefab.SetActive(false);

        goldPrefab.transform.localScale = originalScale;

        ClearReward();
    }

    public void OnClickItemReward()
    {
        if (rewardResult.item == null) return;

        Managers.PlayerStat.AddNewItem(rewardResult.item);

        itemPrefab.SetActive(false);

        TooltipSystem.Instance.Hide();

        itemPrefab.transform.localScale = originalScale;

        Managers.Sound.PlaySound(Defines.SoundKey.GET_EQUIPMENT_SFX, false, ESoundType.Sfx);

        ClearReward();
    }

    public void OnClickCardReward()
    {
        if (rewardResult.card == null) return;

        EClassType type = rewardResult.card.ClassType;

        Hero addCardHero = Managers.Battle.GetHeroByClassType(type);

        Managers.PlayerStat.AddNewCard(rewardResult.card, addCardHero);
        Managers.Sound.PlaySound(Defines.SoundKey.CARD_MOVE_SFX, false, ESoundType.Sfx);

        cardPrefab.SetActive(false);

        TooltipSystem.Instance.Hide();

        cardPrefab.transform.localScale = originalScale;

        ClearReward();
    }

    public void OnClickExitButton()
    {
        rewardResult = null;

        HideUI();

        if (Managers.Stage.CurrentMapNode.mapNodeType == EMapNodeType.Boss)
        {
            EScene nextScene = Managers.Stage.GetNextStageScene();
            LoadManager.Instance.LoadScene(nextScene, () =>
            {
                Managers.Stage.StartStage();
                Managers.UI.MapUI.Show(true);
            });
        }
        else Managers.UI.MapUI.Show();
    }

    public void ClearReward()
    {
        foreach (Transform child in vertical.transform)
        {
            if (child.gameObject.activeSelf)
            {
                return;
            }
        }

        HideUI();

        if (Managers.Stage.CurrentMapNode.mapNodeType == EMapNodeType.Boss)
        {
            EScene nextScene = Managers.Stage.GetNextStageScene();
            LoadManager.Instance.LoadScene(nextScene, () =>
            {
                Managers.Stage.StartStage();
                Managers.UI.MapUI.Show(true);
            });
        }
        else Managers.UI.MapUI.Show();

    }

    public void Clear()
    {
        rewardResult = null;

        HideUI();

        tooltipUI?.SetItem(null);
        tooltipUI?.SetCard(null);
        TooltipSystem.Instance?.Hide();

        DOTween_Img?.DOKill();
        DOTween_Img.color = new Color(0, 0, 0, 0);

        rewardSystem = new RewardSystem();

        goldPrefab.transform.localScale = originalScale;
        itemPrefab.transform.localScale = originalScale;
        cardPrefab.transform.localScale = originalScale;

        goldPrefab.SetActive(false);
        itemPrefab.SetActive(false);
        cardPrefab.SetActive(false);
        rewardMainPrefab.SetActive(false);
        button.SetActive(false);
    }
}
