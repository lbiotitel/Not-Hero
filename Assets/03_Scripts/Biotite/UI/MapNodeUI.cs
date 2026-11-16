using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{

    [Serializable]
    public struct MapNodeSprite
    {
        public EMapNodeType nodeType;
        public Sprite sprite;
    }

    [SerializeField] private float animScale = 0.4f;
    [SerializeField] private float animDuration = 0.2f;
    [SerializeField] private List<MapNodeSprite> nodeSprites = new List<MapNodeSprite>();

    public event Action<MapNodeUI> OnClickedAction;
    public MapNode MapNode { get; private set; }
    
    private Image nodeImage;
    private bool isSelect = false;
    private bool isHover = false;


    public void Init(MapNode mapNode)
    {
        this.MapNode = mapNode;
        nodeImage = GetComponent<Image>();
        nodeImage.enabled = true;
        nodeImage.sprite = GetSpriteByNodeType(mapNode.mapNodeType);
        Resize(mapNode);
    }


    private void Resize(MapNode mapNode)
    {
        if (mapNode.mapNodeType == EMapNodeType.Boss)
        {
            nodeImage.transform.localScale = Vector3.one * 2f;
        }
        else
        {
            nodeImage.transform.localScale = Vector3.one;
        }
    }


    private Sprite GetSpriteByNodeType(EMapNodeType nodeType)
    {
        for (int i = 0; i < nodeSprites.Count; i++)
        {
            if (nodeType == nodeSprites[i].nodeType)
                return nodeSprites[i].sprite;
        }
        nodeImage.enabled = false;
        return null;
    }


    public void CanGoNodeAnimation(bool flag)
    {
        if (flag)
        {
            transform.DOScale(1.2f, 0.6f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine) 
                .SetLink(gameObject);
        }
        else
        {
            transform.DOKill();
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (Managers.Stage.CheckCanGoNode(MapNode))
        {
            nodeImage.DOFade(animScale, animDuration)
               .OnComplete(() => SelectNode());
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHover || isSelect) return;
        if (!Managers.Stage.CheckCanGoNode(MapNode)) return;

        isHover = true;
        transform.DOPunchScale(Vector3.one * animScale, animDuration)
            .OnComplete(() => isHover = false);
    }

    private void SelectNode()
    {
        OnClickedAction?.Invoke(this);
        transform.DOKill();
        isSelect = true;
    }
}
