using System.Collections.Generic;
using UnityEngine;


//모든 이벤트를 관리하는 클래스
public class EventManager : MonoBehaviour
{
    private Dictionary<EEventType, List<IEventListener>> eventDict = new Dictionary<EEventType, List<IEventListener>>();

    //이벤트 등록
    public void AddListener(EEventType eventType, IEventListener eventListener)
    {
        if (!eventDict.ContainsKey(eventType))
        {
            eventDict.Add(eventType, new List<IEventListener>());
        }

        eventDict[eventType].Add(eventListener);
    }


    //이벤트 해제
    public void RemoveListener(EEventType eventType, IEventListener eventListener)
    {
        if (eventDict.ContainsKey(eventType))
        {
            eventDict[eventType].Remove(eventListener);
        }
    }

    //이벤트 알림
    public void NotifyEvent(EEventType eventType, object sender, object param = null)
    {
        if (eventDict.ContainsKey(eventType))
        {
            List<IEventListener> eventListeners = new List<IEventListener>(eventDict[eventType]);
            foreach (var e in eventListeners)
            {
                e.OnEvent(eventType, sender, param);
            }
        }
    }


    public void Clear()
    {
        foreach (var eventList in eventDict.Values)
        {
            eventList.Clear();
        }
        eventDict.Clear();
    }

}
