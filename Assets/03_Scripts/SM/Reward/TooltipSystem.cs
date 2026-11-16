using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Item Tooltip")]
    [SerializeField] private GameObject itemTooltipRoot;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDesc;

    [Header("Card Tooltip")]
    [SerializeField] private GameObject cardTooltipPrefab;

    private GameObject currentCardTooltip;

    public void ShowItem(ItemData item)
    {
        if (item == null) return;

        itemTooltipRoot.SetActive(true);

        itemTooltipRoot.transform.position = Input.mousePosition;

        itemIcon.sprite = item.icon;
        itemName.text = item.name;
        itemDesc.text = "공격력 + " + item.itemStr + "     체력 + " + item.itemMaxHp;

        if (currentCardTooltip != null)
            currentCardTooltip.SetActive(false);
    }

    public void ShowCard(HeroCardData card)
    {
        if (card == null) return;

        if (currentCardTooltip == null)
        {
            currentCardTooltip = Instantiate(cardTooltipPrefab);
        }

        currentCardTooltip.SetActive(true);
        currentCardTooltip.transform.position = Input.mousePosition;

        StaticCardUI ui = currentCardTooltip.GetComponentInChildren<StaticCardUI>();
        ui.Init(card);
    }

    public void Hide()
    {
        itemTooltipRoot.SetActive(false);

        if (currentCardTooltip != null)
            currentCardTooltip.SetActive(false);
    }

    public void Clear()
    {
        if (itemTooltipRoot != null)
            itemTooltipRoot.SetActive(false);

        if (currentCardTooltip != null)
        {
            currentCardTooltip.SetActive(false);

            Destroy(currentCardTooltip);
            currentCardTooltip = null;
        }
    }
}