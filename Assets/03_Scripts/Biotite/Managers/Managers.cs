using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


//��� �Ŵ����� �����ϴ� ������ �Ŵ��� �Դϴ�.
public class Managers : Singleton<Managers>
{

    private GameManager gameManager;
    private EventManager eventManager;
    private StageManager stageManager;
    private SoundManager soundManager;
    private UIManager uiManager;
    private DataManager dataManager;
    private PlayerStatManager playerStatManager;
    private PoolingManager poolingManager;
    private BattleManager battleManager;


    public static GameManager Game => Instance.gameManager;
    public static EventManager Event => Instance.eventManager;
    public static StageManager Stage => Instance.stageManager;  
    public static SoundManager Sound => Instance.soundManager;
    public static UIManager UI => Instance.uiManager;
    public static DataManager Data => Instance.dataManager;
    public static PlayerStatManager PlayerStat => Instance.playerStatManager;
    public static PoolingManager Pooling => Instance.poolingManager;
    public static BattleManager Battle => Instance.battleManager;


    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitManagers();
    }

    public void Clear()
    {
        if (eventManager != null) eventManager.Clear();
        if (dataManager != null) dataManager.Clear();
        if (soundManager != null) soundManager.Clear() ; 
        if (uiManager != null) uiManager.Clear();       
        if (poolingManager != null)poolingManager.Clear(); 
        if (playerStatManager != null) playerStatManager.Clear();
        if (stageManager != null) stageManager.Clear();
        if (battleManager != null) battleManager.Clear();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Destroy(gameObject);
    }



    private void InitManagers()
    {
        gameManager = GetComponentInChildren<GameManager>();
        dataManager = GetComponentInChildren<DataManager>();
        eventManager = GetComponentInChildren<EventManager>();
        uiManager = GetComponentInChildren<UIManager>();
        playerStatManager = GetComponentInChildren<PlayerStatManager>();
        soundManager = GetComponentInChildren<SoundManager>();
        stageManager = GetComponentInChildren<StageManager>();
        poolingManager = GetComponentInChildren<PoolingManager>();
        battleManager = GetComponentInChildren<BattleManager>();

        dataManager.Init();
        uiManager.Init();
        playerStatManager.Init();
        soundManager.Init();
        stageManager.Init();
        poolingManager.Init();
        battleManager.Init();



        if (SceneManager.GetActiveScene().name == EScene.BootStrap.ToString())  
        {
            //나중에 이벤트 호출로 뺌
            LoadManager.Instance.LoadScene(EScene.CharacterSpawn);
        }

    }
}
