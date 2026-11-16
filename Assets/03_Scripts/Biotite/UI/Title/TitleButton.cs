using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private float animScale;
    [SerializeField] private float animDuration;
    private bool isHover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHover) return;
        isHover = true;
        transform.DOPunchScale(Vector3.one * animScale, animDuration)
            .OnComplete(() => isHover = false);
    }

  
}
