using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class TreasureUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventListener
{
    private Light globalLight;
    private float originalLightIntensity;

    [Header("Chest Objects")]
    [SerializeField] private GameObject closeChest;
    [SerializeField] private GameObject gold;
    [SerializeField] private GameObject openChest;
    [SerializeField] private GameObject coins;

    bool isOpened = false;

    private Outline outline;
    private Vector3 originRot;
    private Vector3 coinOriginPos;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        originRot = closeChest.transform.localEulerAngles;
        if(coins != null) coinOriginPos = coins.transform.localPosition;
    }

    public void Init()
    {
        Managers.Event.AddListener(EEventType.OnMapNodeChanged, this);
    }

    public void ResetChest()
    {
        isOpened = false;

        if (outline != null) outline.enabled = true;

        closeChest.SetActive(true);
        gold.SetActive(true);
        openChest.SetActive(false);

        closeChest.transform.localEulerAngles = originRot;

        coins.transform.localPosition = coinOriginPos;
        coins.SetActive(true);
    }

    private void OpenChest()
    {
        isOpened = true;

        if (outline != null) outline.enabled = false;

        closeChest.SetActive(false);
        openChest.SetActive(true);

        if (coins != null)
        {

            coins.transform.DOLocalMoveY(coinOriginPos.y + 10.0f, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    coins.SetActive(false);
                });
        }

        openChest.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);

        Managers.Sound.PlaySound(Defines.SoundKey.OPEN_TREASURE_SFX, false, ESoundType.Sfx);
        Managers.Event.NotifyEvent(EEventType.OnBattleWin, this, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOpened) return;
       
        OpenChest();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        closeChest.transform.DOLocalRotate(new Vector3(5f, 0, 0), 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        closeChest.transform.DOLocalRotate(originRot, 0.3f);
    }

    public void ShowUI()
    {
        globalLight = GameObject.Find("Directional Light")?.GetComponent<Light>();

        if (globalLight != null)
        {
            originalLightIntensity = globalLight.intensity;
            globalLight.intensity = 0.6f;
        }

        gameObject.SetActive(true);
        ResetChest();
    }

    public void HideUI()
    {
        if (globalLight != null)
        {
            globalLight.intensity = originalLightIntensity;
            globalLight = null;
        }

        gameObject.SetActive(false);
    }

    public void OnEvent(EEventType eventType, object sender, object param = null)
    {
        if(eventType == EEventType.OnMapNodeChanged)
        {
            HideUI();
        }
    }

}
