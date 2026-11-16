using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static BattleState;
using static UnitStat;
using static UnityEngine.CullingGroup;
using static UnityEngine.UI.CanvasScaler;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class BattleManager : MonoBehaviour, IEventListener
{
    public BattleState.EBattleState battleState;
    [SerializeField] private SpawnSystem spawnSystem;
    [SerializeField] private CardSystem cardSystem;

    [SerializeField] private List<Hero> heroes = new List<Hero>();
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();

    private Queue<(Card card, List<Unit> targets)> playerActionQueue = new Queue<(Card, List<Unit>)>();

    [SerializeField] private Button turnEnd;

    public CardSystem CardSystem => cardSystem;
    public List<Hero> Heroes => heroes;
    public List<Enemy> Enemies => enemies;

    private bool isProcessingQueue = false;
    private int playerCost;
    int maxCost = 3;
    private int turn = 0;

    public int PlayerCost
    {
        get => playerCost;
        set
        { 
            playerCost = value;
            Managers.UI.CostUI.RefreshUI();
        }
    }
    public int MaxCost
    {
        get => maxCost;
        set => maxCost = value;
    }
    public int Turn => turn;
    private void Start() { }


    private void OnDisable()
    {
        if(Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnEnteredBattleNode, this);
            Managers.Event.RemoveListener(EEventType.OnAddNewItem, this);
            cardSystem.OnDrawAnimStarted -= OnDrawAnimStart;
            cardSystem.OnDrawAnimFinished -= OnDrawAnimFinish;
        }
    }

    public void OnEvent(EEventType eventType, object sender, object param)
    {
        if(eventType == EEventType.OnEnteredBattleNode)
        {
            if(param is List<string> enemyNameList)
            {
                SetEnemiesByData(enemyNameList);
            }
        }

        if (eventType == EEventType.OnAddNewItem)
        {
            ItemData item = (ItemData)param;
            var liveHero = heroes.FindAll(h => !h.UnitStat.IsDie);

            if (liveHero.Count == 0) return;

            Hero targetHero = liveHero[Random.Range(0, liveHero.Count)];

            Debug.Log(targetHero.name + " ����");
            targetHero.ChangeStateToItem(item);
        }
    }

    public void Init()
    {
        cardSystem.Init();
        cardSystem.OnDrawAnimStarted += OnDrawAnimStart;
        cardSystem.OnDrawAnimFinished += OnDrawAnimFinish;


        Managers.Event.AddListener(EEventType.OnEnteredBattleNode, this);
        Managers.Event.AddListener(EEventType.OnAddNewItem, this);
        turnEnd.onClick.AddListener(TurnEnd);
    }
    
    public void OnDrawAnimStart()
    {
        turnEnd.interactable = false;
    }

    public void OnDrawAnimFinish()
    {
        turnEnd.interactable = true;
    }

    private void Update()
    {
    }

    public IEnumerator ChangeState(BattleState.EBattleState newState)
    {
        battleState = newState;

        switch(battleState)
        {
            case BattleState.EBattleState.Start:

                Managers.Event.NotifyEvent(EEventType.OnBattleStart, this);
                turnEnd.gameObject.SetActive(true);
                StartBattle();
                yield return new WaitForSeconds(3f);
                break;

            case BattleState.EBattleState.PlayerTurn:
                Managers.Event.NotifyEvent(EEventType.OnPlayerTurn, this, ++turn);
                PlayerTurnStart();
                break;

            case BattleState.EBattleState.EnemyTurn:
                Managers.Event.NotifyEvent(EEventType.OnEnemyTurn, this);
                turnEnd.interactable = false;
                StartCoroutine(EnemyTurn());
                yield return new WaitForSeconds(2f);
                break;

            case BattleState.EBattleState.Win:
                Managers.Event.NotifyEvent(EEventType.OnBattleWin, this);
                turnEnd.gameObject.SetActive(false);
                ClearEnemy();
                yield return new WaitForSeconds(0.7f);
                break;

            case BattleState.EBattleState.Lose:
                Managers.Game.GameOver(Heroes);
                turnEnd.gameObject.SetActive(false);
                break;
        }

        yield return null;
    }

    public void SetEnemiesByData(List<string> enemyNameList)
    {
        enemies = spawnSystem.SpawnEnemy(enemyNameList);
        StartCoroutine(ChangeState(BattleState.EBattleState.Start));
    }


    public void SetHeroes(List<string> heroNameList)
    {
        heroes = spawnSystem.SpawnPlayer(heroNameList);
    }

    public void StartBattle()
    {
        turn = 0;
        playerCost = maxCost;

        Managers.UI.CostUI.Init();
        Managers.Event.NotifyEvent(EEventType.OnEnergyCostChanged, this, playerCost);

        StartCoroutine(ChangeState(BattleState.EBattleState.PlayerTurn));
    }

    public void PlayerTurnStart()
    { 
        foreach (var d in heroes)
        {
            if(d.UnitStat.IsDie) continue;
            d.UnitStat.DEF = 0;
            d.UnitUI.SetBlock(d.UnitStat.DEF);
            d.UnitUI.ShowIcon(false, IconType.Block);
        }
        Debug.Log("PlayerTurn Start ButtonInput");

        ResetEnergyCost();

        //enemyTargetSetting
        foreach(var enemy in enemies)
        {
            if(enemy.UnitStat.IsDie && enemy == null)
            {
                continue;
            }

            enemy.Idle();
        }
        PlayerTurn();
    }

    public void PlayerTurn()
    {

    }

    public void EnqueuePlayerAction(Card card, List<Unit> targets)
    {
        playerActionQueue.Enqueue((card, targets));

        if(!isProcessingQueue)
            StartCoroutine(ProcessPlayerQueue());
    }

    private IEnumerator ProcessPlayerQueue()
    {
        isProcessingQueue = true;

        while(playerActionQueue.Count > 0)
        {
            var (card, targets) = playerActionQueue.Dequeue();
            card.DoCardLogic(targets);

            yield return new WaitForSeconds(0.3f);
        }
        isProcessingQueue = false;
    }

    public IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.7f);

        foreach (var d in enemies)
        {
            if(d == null || d.UnitStat.IsDie)
            {
                continue;
            }

            d.UnitStat.DEF = 0;
            d.UnitUI.SetBlock(d.UnitStat.DEF);
            d.UnitUI.ShowIcon(false, IconType.Block);

            yield return new WaitForSeconds(0.5f);

            d.Skill();
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(ChangeState(BattleState.EBattleState.PlayerTurn));
    }

    public void TurnEnd()
    {
        if(battleState != BattleState.EBattleState.PlayerTurn)
        {
            turnEnd.interactable = false;
            return;
        }

        Managers.Sound.PlaySound(Defines.SoundKey.TURN_BUTTON_SFX, false, ESoundType.Sfx);
        StartCoroutine(ChangeState(BattleState.EBattleState.EnemyTurn));
    }

    private IEnumerator WatiQueueThenEnemyTurn()
    {
        yield return new WaitUntil(() => !isProcessingQueue && playerActionQueue.Count == 0);

        StartCoroutine(ChangeState(BattleState.EBattleState.EnemyTurn));
    }

    public void ResetEnergyCost()
    {
        playerCost = maxCost;

        Managers.Event.NotifyEvent(EEventType.OnEnergyCostChanged, this, playerCost);
    }
    public bool UseCost(int cardCost)
    {
        if(playerCost < cardCost)
        {
            Debug.Log("your not hanv cost");
            return false;
        }

        playerCost -= cardCost;

        playerCost = Mathf.Clamp(playerCost, 0, maxCost);

        Managers.Event.NotifyEvent(EEventType.OnEnergyCostChanged, this, playerCost);

        return true;
    }

    public void IsGameWin()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.UnitStat.IsDie)
                return;
        }

        StartCoroutine(ChangeState(BattleState.EBattleState.Win));
    }

    public void IsGameLose()
    {
        foreach(var hero in heroes)
        {
            if(!hero.UnitStat.IsDie)
                return;
        }

        StartCoroutine(ChangeState(BattleState.EBattleState.Lose));
    }


    public Hero GetHeroByClassType(EClassType classType)
    {
        foreach(var hero in Heroes)
        {
            if(hero.classType == classType)
                return hero;
        }

        return null;
    }

    public List<EClassType> GetHeroClassTypeList()
    {
        var heroClassList = new List<EClassType>();

        foreach(var hero in heroes)
        {
            heroClassList.Add(hero.classType);
        }

        return heroClassList;
    }

    public void ClearEnemy()
    {
        if (enemies.Count != 0)
        {
            foreach (var x in enemies)
            {
                Destroy(x.gameObject);
            }

            enemies.Clear();
        }
    }

    public void DestroyAll()
    {
        foreach(var x in heroes)
        {
            Destroy(x);
        }

        foreach(var x in enemies)
        {
            Destroy(x);
        }

        heroes.Clear();
        enemies.Clear();
    }

    public void Clear()
    {
        StopAllCoroutines();

        if (heroes != null)
        {
            foreach (var hero in heroes)
            {
                if (hero != null)
                    Destroy(hero.gameObject);
            }
            heroes.Clear();
        }

        if (enemies != null)
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                    Destroy(enemy.gameObject);
            }
            enemies.Clear();
        }

        playerActionQueue.Clear();
        isProcessingQueue = false;
        
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnEnteredBattleNode, this);
            Managers.Event.RemoveListener(EEventType.OnAddNewItem, this);
        }

        turnEnd.onClick.RemoveAllListeners();
        turnEnd.interactable = false;

        playerCost = maxCost;
        turn = 0;
    }
}
