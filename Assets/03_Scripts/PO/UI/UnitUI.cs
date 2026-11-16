using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유닛의 스탯들의 증가/감소에 의한 분류입니다.
/// </summary>
public enum IconType
{
    HpBar,
    Block,
    Intent
}

public class UnitUI : MonoBehaviour
{
    [Header("Intent")]
    [SerializeField] Image intent;

    [Header("HP")]
    [SerializeField] Slider hpBar;
    [SerializeField] TextMeshProUGUI hpText;

    [Header("Shield")]
    [SerializeField] GameObject blockIcon;
    [SerializeField] TextMeshProUGUI blockText;

    private void Awake()
    {
        hpBar = GetComponentInChildren<Slider>();
        hpText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(float curHP, float maxHP)
    {
        SetHP(curHP, maxHP);
    }

    public void SetUI(IconType type, float amount, float maxamount = 0)
    {
        switch(type)
        {
            case IconType.Block:
                SetBlock(amount);
                break;

            case IconType.HpBar:
                SetHP(amount, maxamount);
                break;

            default:
                break;
        }
    }

    public void SetHP(float curHP, float maxHP)
    {
        if(hpBar != null)
        {
            hpBar.value = curHP / maxHP;
            hpText.text = $"{Mathf.FloorToInt(Mathf.Max(0, curHP))} / {maxHP}";
        }
    }

    public void SetBlock(float amount)
    {
        blockText.text = Mathf.FloorToInt(amount).ToString();
    }

    public void ShowIcon(bool isShow, IconType iconType)
    {
        switch(iconType)
        {
            case IconType.Block:
                blockIcon.SetActive(isShow);
                break;

            case IconType.HpBar:
                if(hpBar != null)
                    hpBar.gameObject.SetActive(isShow);
                break;

            case IconType.Intent:
                if(intent != null)
                    intent.gameObject.SetActive(isShow);
                break;
        }
    }
}
