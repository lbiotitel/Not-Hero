using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestUI : MonoBehaviour
{
    private struct HeroOriginalState
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        public int layer;
    }

    private Dictionary<Hero, HeroOriginalState> originalStates = new Dictionary<Hero, HeroOriginalState>();

    [Header("World References")]
    private Light globalLight;
    private float originalLightIntensity;
    [SerializeField] private GameObject campfirePrefab;
    [SerializeField] private RenderTexture sharedRT;


    [Header("UI Elements")]
    [SerializeField] private RawImage displayImage;
    [SerializeField] private Button restButton;
    [SerializeField] private Button costButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text restButtonText;

    [SerializeField] private float healAmount;


    private GameObject spawnCampfire;


    public void Init()
    {
        restButton.onClick.AddListener(HealPlayer);
        costButton.onClick.AddListener(CostIncrease);
        exitButton.onClick.AddListener(HideUI);

        restButtonText.text = "휴식";
    }

    public void ShowUI()
    {
        globalLight = GameObject.Find("Directional Light")?.GetComponent<Light>();

        if (globalLight != null) 
        { 
            originalLightIntensity = globalLight.intensity;
            globalLight.intensity = 0.2f; 
        }

        gameObject.SetActive(true);

        restButton.interactable = true;
        costButton.interactable = true;

        if (spawnCampfire == null)
        {
            spawnCampfire = Instantiate(campfirePrefab);

            Camera cam = spawnCampfire.GetComponentInChildren<Camera>();
            if (cam != null) { cam.targetTexture = sharedRT; }
            displayImage.texture = sharedRT;
        }

        RestRoom room = spawnCampfire.GetComponent<RestRoom>();
        if (room != null)
        {
            PrepareHeroes(room.heroSeats);
        }
    }

    public void HideUI()
    {
        RestoreHeroes();

        if (spawnCampfire != null)
        {
            Destroy(spawnCampfire);
            spawnCampfire = null;
        }

        if (globalLight != null)
        {
            globalLight.intensity = originalLightIntensity;
            globalLight = null;
        }

        Managers.Event.NotifyEvent(EEventType.OnNodeCompleted, this, false);
        gameObject.SetActive(false);
    }

    private void PrepareHeroes(Transform[] heroSeats)
    {
        originalStates.Clear();
        int eventLayer = LayerMask.NameToLayer("Event_Object");
        var heroes = Managers.Battle.Heroes;

        for (int i = 0; i < heroes.Count; i++)
        {
            if (heroes[i] == null) continue;

            originalStates[heroes[i]] = new HeroOriginalState
            {
                position = heroes[i].transform.position,
                scale = heroes[i].transform.localScale,
                rotation = heroes[i].transform.rotation,
                layer = heroes[i].gameObject.layer
            };

            if (i < heroSeats.Length)
            {
                heroes[i].transform.position = heroSeats[i].position;

                if (i == 1)
                {
                    heroes[i].transform.rotation = Quaternion.Euler(0, 180f, 0);
                }
                else
                {
                    heroes[i].transform.rotation = Quaternion.identity;
                }
            }

            heroes[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            SetLayerRecursively(heroes[i].gameObject, eventLayer);

            if (heroes[i].UnitUI != null) heroes[i].UnitUI.gameObject.SetActive(false);

        }
    }

    private void RestoreHeroes()
    {
        foreach (var kvp in originalStates)
        {
            Hero hero = kvp.Key;
            HeroOriginalState state = kvp.Value;

            if (hero == null) continue;

            hero.transform.position = state.position;
            hero.transform.localScale = state.scale;
            hero.transform.rotation = state.rotation;
            SetLayerRecursively(hero.gameObject, state.layer);

            if (hero.UnitUI != null) hero.UnitUI.gameObject.SetActive(true);
        }
        originalStates.Clear();
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void HealPlayer()
    {
        restButton.interactable = false;
        costButton.interactable = false;

        foreach (var hero in Managers.Battle.Heroes)
        {
            if (hero == null || hero.UnitStat.IsDie) continue;

            hero.UnitStat.ModifyHp(healAmount);
            hero.UnitUI.SetHP(hero.UnitStat.Hp, hero.UnitStat.MAXHP);

        }
    }

    private void CostIncrease()
    {
        restButton.interactable = false;
        costButton.interactable = false;
        Managers.Battle.MaxCost += 1;
    }

}
