using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentShopUI : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private float animScale;
    [SerializeField] private TMP_Text equipmentNameText;
    [SerializeField] private Image equipmentImage;
    [SerializeField] private TMP_Text priceText;

    public event Action<EquipmentShopUI> OnClickedAction;

    private CanvasGroup canvasGroup;
    private bool isSell = false;
    private bool isAnim = false;
    public ItemData ItemData { get; private set; }


    public void Init(ItemData itemData)
    {
        isSell = false;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        ItemData = itemData;
        DrawShopEquipementUI();
    }

    private void DrawShopEquipementUI()
    {
        equipmentImage.sprite = ItemData.icon;
        equipmentNameText.text = ItemData.itemName;
        priceText.text = ItemData.price.ToString();
    }


    public void Sell()
    {
        isSell = true;
        canvasGroup.alpha = 0.2f;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSell) return;
        OnClickedAction?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSell) return;
        if (isAnim) return;
        isAnim = true;
        Managers.UI.ToolTipUI.Show(ItemData);
        transform.DOPunchScale(Vector3.one * animScale, 0.1f).
            OnComplete(() => isAnim = false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.ToolTipUI.Hide();
    }
}
