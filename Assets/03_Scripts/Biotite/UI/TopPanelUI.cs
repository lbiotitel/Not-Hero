using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



//화면 위의 골드나 턴 맵 등 위에 있는 모든 UI를 공통으로 관리합니다.
public class TopPanelUI : MonoBehaviour, IEventListener
{
    [Header("Buttons")]
    [SerializeField] private Button mapButton;
    [SerializeField] private Button optionButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text partyNameText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text currentFloorText;

    [Header("Equipment UI")]
    [SerializeField] private GameObject equipmentNodeUIPrefab;
    [SerializeField] private Transform equipmentUITransform;

    public void Init()
    {
        Managers.Event.AddListener(EEventType.OnGoldValueChanged, this);
        Managers.Event.AddListener(EEventType.OnMapNodeChanged, this);
        Managers.Event.AddListener(EEventType.OnAddNewItem, this);
        Managers.Game.OnTimeTick += DrawPlayTimeText;
        mapButton.onClick.AddListener(() => Managers.UI.MapUI.Show());
    }


    private void OnDestroy()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnGoldValueChanged, this);
            Managers.Event.RemoveListener(EEventType.OnMapNodeChanged, this);
            Managers.Event.RemoveListener(EEventType.OnAddNewItem, this);
            Managers.Game.OnTimeTick -= DrawPlayTimeText;
        }
    }

    public void OnEvent(EEventType eventType, object sender, object param = null)
    {
        if (eventType == EEventType.OnGoldValueChanged)
        {
            DrawGoldText((int)param);
        }
        else if (eventType == EEventType.OnMapNodeChanged)
        {
            DrawCurrentFloorText((int)param);
        }
        else if (eventType == EEventType.OnAddNewItem)
        {
            DrawEquipmentUI((ItemData)param);
        }
    }

    private void DrawPlayTimeText(int playTime)
    {
        playTimeText.text =
            string.Format("{0:00}:{1:00}", (int)(playTime / 60), (int)(playTime % 60));
    }

    private void DrawCurrentFloorText(int floor)
    {
        currentFloorText.text = $"{floor}F";
    }

    private void DrawGoldText(int gold)
    {
        goldText.text = $"{gold}G";
    }

    private void DrawEquipmentUI(ItemData itemData)
    { 
        GameObject equipmentNodeUI = Instantiate(equipmentNodeUIPrefab, equipmentUITransform);
        equipmentNodeUI.GetComponent<Image>().sprite = itemData.icon;

        EventTrigger trigger = equipmentNodeUI.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            Managers.UI.ToolTipUI.Show(itemData); 
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            Managers.UI.ToolTipUI.Hide();
        });
        trigger.triggers.Add(entryExit);
    }


}
