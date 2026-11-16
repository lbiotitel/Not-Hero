using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData item;
    private HeroCardData card;
    private GameObject goldPrefab;

    private Vector2 originScale;
    private float changeSale = 1.2f;


    public void Awake()
    {
        originScale = transform.localScale;
    }

    public void SetItem(ItemData itemData)
    {
        item = itemData;
        card = null;
    }

    public void SetCard(HeroCardData cardData)
    {
        card = cardData;
        item = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            TooltipSystem.Instance.ShowItem(item);
        else if (card != null)
            TooltipSystem.Instance.ShowCard(card);

        transform.localScale = changeSale * originScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
        transform.localScale = originScale;
    }
    public void Clear()
    {
        item = null;
        card = null;
        transform.localScale = originScale;

        if (TooltipSystem.Instance != null)
            TooltipSystem.Instance.Hide();
    }
}