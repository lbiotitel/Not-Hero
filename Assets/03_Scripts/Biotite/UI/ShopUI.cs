using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Card")]
    [SerializeField] private CardShopUI cardShopUIPrefab;
    [SerializeField] private Transform cardShopUILayout;
    [SerializeField] private int generateCardCount;

    [Header("Equipment")]
    [SerializeField] private EquipmentShopUI equipmentShopUIPrefab;
    [SerializeField] private int generateEquipmentCount;
    [SerializeField] private Transform shopEquipmentUILayout;

    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text shopOwnerDialogueText;
    [SerializeField] private List<string> shopOwnerDialogue = new List<string>();

    private List<CardShopUI> cardShopUIList = new List<CardShopUI>();
    private List<EquipmentShopUI> equipmentShopUIList = new List<EquipmentShopUI>();

    private List<ItemData> itemDataList;

    public void Init()
    {
        exitButton.onClick.AddListener(HideUI);

        itemDataList = Managers.Data.GetDataList<ItemData>(); //미리 캐싱

        //상점 카드 생성
        InitCardShopUI(generateCardCount);
        InitEquipmentShopUI(generateEquipmentCount);
    }

    private void InitCardShopUI(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CardShopUI cardShopUI = Instantiate(cardShopUIPrefab, cardShopUILayout);
            cardShopUI.OnCardClicked += SellCard;
            cardShopUIList.Add(cardShopUI);
        }
    }

    private void InitEquipmentShopUI(int count)
    {
        for (int i = 0; i < count; i++)
        {
            EquipmentShopUI equipmentShopUI = Instantiate(equipmentShopUIPrefab, shopEquipmentUILayout);
            equipmentShopUI.OnClickedAction += SellEquipment;
            equipmentShopUIList.Add(equipmentShopUI);
        }
    }

    private void GenerateShopCardsByPrbability()
    {
        List<HeroCardData> finalCards = Managers.Data.GradeProbability.GetRandomCardList(
            CardUtility.GetCachedPartyCards(),
            cardShopUIList.Count);

        for (int i = 0; i < finalCards.Count; i++)
        {
            cardShopUIList[i].Init(finalCards[i]);
        }
    }

    private void GenerateShopEquipmentsByProbability()
    {
        for (int i = 0; i < equipmentShopUIList.Count; i++)
        {
            EGrade selectedGrade = Managers.Data.GradeProbability.GetGrade();

            List<ItemData> candidates = new List<ItemData>();

            foreach (ItemData item in itemDataList)
            {
                if (item.ItemGrade == selectedGrade)
                {
                    candidates.Add(item);
                }
            }

            if (candidates.Count > 0)
            {
                int randIndex = Random.Range(0, candidates.Count);
                equipmentShopUIList[i].Init(candidates[randIndex]);
            }
            else
            {
                int fallbackIndex = Random.Range(0, itemDataList.Count);
                equipmentShopUIList[i].Init(itemDataList[fallbackIndex]);
            }
        }
    }

    private void ShopOwnerRandomDialogue()
    {
        int randIndex = Random.Range(0, shopOwnerDialogue.Count);
        string dialogue = shopOwnerDialogue[randIndex];

        shopOwnerDialogueText.text = dialogue;
    }

    private void SellEquipment(EquipmentShopUI equipmentShopUI)
    {
        //골드가 충분한지 확인 후 구매
        ItemData itemData = equipmentShopUI.ItemData;
        int price = itemData.price;
        if (Managers.PlayerStat.IsEnoughGold(price))
        {
            Managers.PlayerStat.SpendGold(price);
            Managers .PlayerStat.AddNewItem(itemData);
            Managers.Sound.PlaySound(Defines.SoundKey.SHOP_SELL_SFX, false, ESoundType.Sfx);
            equipmentShopUI.Sell();
        }
        //구매 후 상점 주인 대사 변경
        ShopOwnerRandomDialogue();
    }

    private void SellCard(CardShopUI cardShopUI)
    {
        HeroCardData cardData = cardShopUI.CardData;
        int price = cardData.Price;
        if (Managers.PlayerStat.IsEnoughGold(price))
        {
            Managers.PlayerStat.SpendGold(price);
            Hero hero = Managers.Battle.GetHeroByClassType(cardData.ClassType);
            Managers.PlayerStat.AddNewCard(cardData, hero);
            cardShopUI.Sell();

            Managers.Sound.PlaySound(Defines.SoundKey.SHOP_SELL_SFX, false, ESoundType.Sfx);
        }
    
        ShopOwnerRandomDialogue();
    }

    public void ShowUI()
    {
        ShopOwnerRandomDialogue();
        GenerateShopCardsByPrbability();
        GenerateShopEquipmentsByProbability();
        gameObject.SetActive(true);
    }


    public void HideUI()
    {
        Managers.Event.NotifyEvent(EEventType.OnNodeCompleted, this, false);
        gameObject.SetActive(false);
    }

}
