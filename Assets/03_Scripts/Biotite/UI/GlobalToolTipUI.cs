using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalToolTipUI : MonoBehaviour
{
    [SerializeField] private Transform bgTransform;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescText;


    public void Show(ItemData itemData)
    {
        gameObject.SetActive(true);

        itemImage.sprite = itemData.icon;
        itemNameText.text = itemData.itemName;
        itemDescText.text = GetDesc(itemData);

        CalcPosition();
    }

    private void CalcPosition()
    {
        Canvas.ForceUpdateCanvases();

        Vector2 mousePos = Input.mousePosition;
        RectTransform rectTransform = bgTransform as RectTransform;
        Vector2 tooltipSize = rectTransform.sizeDelta;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mousePos.x + tooltipSize.x > screenWidth)
        {
            mousePos.x -= tooltipSize.x;
        }

        if (mousePos.y + tooltipSize.y > screenHeight)
        {
            mousePos.y -= tooltipSize.y;
        }

        else if (mousePos.y - tooltipSize.y < 0)
        {
            mousePos.y += tooltipSize.y;
        }

        bgTransform.position = mousePos;
    }

    private string GetDesc(ItemData itemData)
    {
        return $"+{itemData.itemMaxHp}MaxHp\n+{itemData.itemStr}Str\n";
    }


    public void Hide() => gameObject.SetActive(false);
}
