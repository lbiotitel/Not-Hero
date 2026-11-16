using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour, IEventListener
{
    [SerializeField] private Image BG;
    [SerializeField] private TMP_Text result;

    [SerializeField] private TMP_Text playTimeText;

    [SerializeField] private Transform heroListParent;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private Transform cardListParent;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button exitButton;


    private void OnDisable()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnGameOver, this);
            Managers.Event.RemoveListener(EEventType.OnGameWin, this);
        }
    }

    public void Init()
    {
        Managers.Event.AddListener(EEventType.OnGameOver, this);
        Managers.Event.AddListener(EEventType.OnGameWin, this);
        exitButton.onClick.AddListener(ExitButton);
        Hide();
    }

    private void Show()
    {
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void OnEvent(EEventType eventType, object sender, object param)
    {
        if (eventType == EEventType.OnGameOver || eventType == EEventType.OnGameWin)
        {
            ResultData data = (ResultData)param;

            Managers.Instance.StartCoroutine(SetTimeSacle(data));
        }
    }

    private IEnumerator SetTimeSacle(ResultData data)
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;

        ShowResult(data);
    }

    private void ShowResult(ResultData data)
    {
        gameObject.SetActive(true);

        result.text = data.result == ResultData.EResult.Victory ? "승리!" : "죽었습니다..";
        playTimeText.text = $" 플레이 시간 : {data.playTime:F1}초";

        SetHeroList(data.heroList);
        SetItemList(data.itemList);
        SetCardList(data.cardList);

        LayoutRebuilder.ForceRebuildLayoutImmediate(itemListParent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardListParent.GetComponent<RectTransform>());
    }
    private void SetHeroList(List<Hero> heroes)
    {
        foreach (Transform x in heroListParent)
            Destroy(x.gameObject);

        foreach (var hero in heroes)
        {
            GameObject obj = Instantiate(heroPrefab, heroListParent);

            Image img = obj.GetComponent<Image>();

            if (img == null)
            {
                return;
            }

            img.sprite = hero.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void SetItemList(List<ItemData> items)
    {
        if (items == null)
        {
            return;
        }

        Debug.Log(items.Count);
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (var x in items)
        {
            GameObject obj = Instantiate(itemPrefab, itemListParent);

            Image img = obj.GetComponent<Image>();

            if (img == null)
            {
                return;
            }
            img.sprite = x.icon;
        }
    }

    private void SetCardList(List<CardData> card)
    {
        if (card == null)
        {
            return;
        }

        Debug.Log(card.Count);
        foreach (Transform child in cardListParent)
            Destroy(child.gameObject);

        foreach (var x in card)
        {
            GameObject obj = Instantiate(cardPrefab, cardListParent);

            Image img = obj.GetComponent<Image>();

            if (img == null)
            {
                return;
            }
            img.sprite = x.CardIcon;

        }
    }

    private void ExitButton()
    {
        Clear();

        Managers.Instance.Clear();

        LoadManager.Instance.LoadScene(EScene.Title);
    }

    private void Clear()
    {
        result.text = "";
        playTimeText.text = "";

        foreach (Transform child in heroListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in cardListParent)
        {
            Destroy(child.gameObject);
        }

        Hide();
    }
}
