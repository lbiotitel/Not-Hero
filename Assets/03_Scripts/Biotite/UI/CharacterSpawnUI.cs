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
        ); // 캐릭터 생성이 끝나면, 파티 카드 데이터를 캐싱한다. (캐싱된 데이터는 게임 시작 후에 사용된다.)

        LoadManager.Instance.LoadScene(EScene.F0, () => {
            Managers.Game.GameStart();
            Managers.Stage.StartStage();
            Managers.UI.MapUI.Show(true);
        });
    }

}
