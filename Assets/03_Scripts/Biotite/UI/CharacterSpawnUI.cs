using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpawnUI : MonoBehaviour
{
    [SerializeField] private FirstInitSystem characterSpawnSystem;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        startButton.onClick.AddListener(() => GameStart());
    }

    private void GameStart()
    {
        characterSpawnSystem.SetHeroObjects();

        CardUtility.GetPartyFilterCards(
            Managers.Data.GetDataList<HeroCardData>(),
            Managers.Battle.Heroes
        ); // ĳ���� ������ ������, ��Ƽ ī�� �����͸� ĳ���Ѵ�. (ĳ�̵� �����ʹ� ���� ���� �Ŀ��� �����ȴ�.)

        LoadManager.Instance.LoadScene(EScene.F0, () => {
            Managers.Game.GameStart();
            Managers.Stage.StartStage();
            Managers.UI.MapUI.Show(true);
        });
    }

}
