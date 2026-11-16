using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum EDamageType
{ 
    Normal,
    Block,
}

public class DamageText : MonoBehaviour,IPoolingObject
{
    public GameObject OriginPrefab { get; set; }

    private TMP_Text damageText;
    [SerializeField] private Vector3 movePos;
    [SerializeField] private float animMoveDuration;
    [SerializeField] private float animFadeOutDuration;


    private void Awake()
    {
        damageText = GetComponent<TMP_Text>();
    }


    public void Show(float damage, EDamageType damageType = EDamageType.Normal)
    {
        ResetObject();
        damageText.color = GetTextColor(damageType);

        if (damageType == EDamageType.Block)
            damageText.text = "Blocked";
        else
            damageText.text = damage.ToString();

        DoAnimation();
    }


    private Color GetTextColor(EDamageType damageType)
    {
        switch (damageType)
        {
            case EDamageType.Normal:
                return Color.white;
            case EDamageType.Block:
                return Color.blue;
        }

        return Color.white;
    }


    private void DoAnimation()
    {
        float randomX = Random.Range(-movePos.x, movePos.x);
        Vector3 jumpTarget = transform.position + new Vector3(randomX, movePos.y, movePos.z);

        transform.DOJump(jumpTarget, 0.5f, 1, animMoveDuration);

        damageText.DOFade(0, animFadeOutDuration)
            .OnComplete(() => {
                ReturnObject();
            });
    }

    public void ResetObject()
    {
        transform.DOKill();
        damageText.transform.DOKill();
        damageText.color = Color.white;
    }

    public void ReturnObject()
    {
        Managers.Pooling.ReturnObject(gameObject);
    }
}
