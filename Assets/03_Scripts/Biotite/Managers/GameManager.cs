using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int playTime = 0;
    private float time = 0f;
    private bool gameStart = false;

    public int PlayTime => playTime;

    //Tick event
    public event Action<int> OnTimeTick;

    public void GameStart()
    {
        gameStart = true;
        Managers.Event.NotifyEvent(EEventType.OnGameStart, this);
    }

    public void GameOver(List<Hero> heroList)
    {
        gameStart = false;
        SetData(heroList, ResultData.EResult.Defeat,
            (data) => Managers.Event.NotifyEvent(EEventType.OnGameOver, this, data));
    }

    public void GameWin(List<Hero> heroList)
    {
        gameStart = false;
        SetData(heroList,ResultData.EResult.Victory,
            (data) => Managers.Event.NotifyEvent(EEventType.OnGameWin, this, data));
    }


    private void SetData(List<Hero> heroList, ResultData.EResult result ,Action<ResultData> eventAction)
    {
        ResultData data = new ResultData();
        data.result = result;
        data.playTime = Managers.Game.PlayTime;

        data.heroList = new List<Hero>(heroList);

        data.itemList = new List<ItemData>(Managers.PlayerStat.CurrentItemList);
        data.cardList = new List<CardData>(Managers.PlayerStat.CurrentCardList);

        eventAction?.Invoke(data);
    }



    private void Update()
    {
        TickTime();
    }

    //�÷��� �ð��� �帨�ϴ�.
    private void TickTime()
    {
        if (!gameStart) return;

        if (time < 1f) time += Time.deltaTime;
        else
        {
            time = 0f;
            playTime += 1;
            OnTimeTick?.Invoke(playTime);
        }
    }
}
