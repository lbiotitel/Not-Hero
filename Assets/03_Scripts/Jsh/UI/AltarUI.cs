using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AltarUI : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private Button exitbutton;
    [SerializeField] private Button scrollButton;
    [SerializeField] private Button priestButton;


    [Header("Revive Button")]
    [SerializeField] private HeroReviveButton heroButtonPrefab;
    [SerializeField] private Transform heroButtonContainer;
    [SerializeField] private TMP_Text altarDesText;
    [SerializeField] private GameObject reviveBG;

    private float pendingHpPercent = 0f;
    private List<HeroReviveButton> spawnedButtons = new List<HeroReviveButton>();

    public void Init()
    {
        scrollButton.onClick.AddListener(ReviveScroll);
        priestButton.onClick.AddListener(RevivePriest);
        exitbutton.onClick.AddListener(HideUI);

        if (heroButtonContainer != null) heroButtonContainer.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        ResetMainButtons();
        RefreshButtonState();
    }

    public void HideUI()
    {
        Managers.Event.NotifyEvent(EEventType.OnNodeCompleted, this, false);
        gameObject.SetActive(false);
    }

    private void ReviveScroll() => ShowHeroSelectUI(0.5f, "정체불명의 스크롤이 반응하였습니다.");
    private void RevivePriest() => ShowHeroSelectUI(0.3f, "사제로 부활을 선택하였습니다.");


    private void ShowHeroSelectUI(float hpPercent, string description)
    {
        pendingHpPercent = hpPercent;
        altarDesText.text = description;

        ClearSpawnedButtons();

        scrollButton.gameObject.SetActive(false);
        priestButton.gameObject.SetActive(false);

        if (reviveBG != null) reviveBG.SetActive(true);

        bool anyDeadHero = false;
        foreach (Hero h in Managers.Battle.Heroes)
        {
            if (h == null || !h.UnitStat.IsDie) continue;

            anyDeadHero = true;
            Hero captured = h;

            heroButtonContainer.gameObject.SetActive(true);

            HeroReviveButton btn = Instantiate(heroButtonPrefab, heroButtonContainer);
            btn.gameObject.SetActive(true);
            btn.Init(h.UName, h.GetSprite(), () => ConfirmRevive(captured));
            spawnedButtons.Add(btn);
        }

        if (!anyDeadHero)
        {
            altarDesText.text = "제단의 힘이 사라졌습니다.";
            scrollButton.gameObject.SetActive(true);
            priestButton.gameObject.SetActive(true);
        }

    }

    private void ConfirmRevive(Hero target)
    {
        if (target == null) return;


        target.UnitStat.IsDie = false;
        target.UnitStat.ModifyHp(target.UnitStat.MAXHP * pendingHpPercent);
        target.UnitUI.SetHP(target.UnitStat.Hp, target.UnitStat.MAXHP);
        target.UnitUI.ShowIcon(true, IconType.HpBar);
        if (target.Animator != null) target.Animator.Play("Idle");
        target.UnitUI.SetUI(IconType.HpBar, target.UnitStat.Hp, target.UnitStat.MAXHP);
        target.gameObject.SetActive(true);


        ClearSpawnedButtons();
        heroButtonContainer.gameObject.SetActive(false); // ����Ʈ â �ݱ�

        if (reviveBG != null) reviveBG.SetActive(false);

        scrollButton.gameObject.SetActive(true);
        priestButton.gameObject.SetActive(true);
        scrollButton.interactable = false;
        priestButton.interactable = false;

        altarDesText.text = $"{target.UName}이 부활했습니다!";

    }

    private void RefreshButtonState()
    {
        bool hasPriest = false;
        bool hasDeadHero = false;

        foreach (Hero h in Managers.Battle.Heroes)
        {
            if (h == null) continue;
            if (h.UnitStat.IsDie) hasDeadHero = true;
            if (h.classType == EClassType.Priest && !h.UnitStat.IsDie) hasPriest = true;
        }

        scrollButton.interactable = hasDeadHero;
        priestButton.interactable = hasDeadHero && hasPriest;
    }

    private void ResetMainButtons()
    {
        ClearSpawnedButtons();
        heroButtonContainer.gameObject.SetActive(false);
        if (reviveBG != null) reviveBG.SetActive(false);

        scrollButton.gameObject.SetActive(true);
        priestButton.gameObject.SetActive(true);
        altarDesText.text = "제단에서 정체불명의 힘이 느껴집니다.";
    }

    private void ClearSpawnedButtons()
    {
        foreach (var b in spawnedButtons)
        {
            if (b != null) Destroy(b.gameObject);
        }
        spawnedButtons.Clear();
    }
}
