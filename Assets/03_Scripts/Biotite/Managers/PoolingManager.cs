using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{

    private Dictionary<GameObject, Queue<GameObject>> poolDict;

    public void Init()
    {
        poolDict = new Dictionary<GameObject, Queue<GameObject>>();
    }



    //오브젝트를 가져옵니다. 
    public T GetObject<T>(GameObject goPrefab,Vector3 position) where T : Component
    {
        GameObject res = GetObject(goPrefab, position);
        if (res.TryGetComponent<T>(out T component))
        {
            return component;
        }

        Debug.LogError("해당되는 컴포넌트가 없습니다.");
        return null;
    }


    //오브젝트를 가져옵니다. 없다면 새로만들고 풀링 합니다.
    public GameObject GetObject(GameObject goPrefab, Vector3 position)
    {
        if (!goPrefab.TryGetComponent(out IPoolingObject _))
        {
            Debug.LogError($"풀링이 허용되지 않은 프리팹 입니다. 풀링을 할려면 IPoolingObject를 붙이세요");
            return null;
        }

        GameObject res = null;

        if (!poolDict.ContainsKey(goPrefab))
        {
            Queue<GameObject> newPool = new Queue<GameObject>();
            poolDict.Add(goPrefab, newPool);
        }

        Queue<GameObject> pool = poolDict[goPrefab];

        if (pool.Count <= 0)
        {
            GameObject poolingObject = Instantiate(goPrefab, transform);
            poolingObject.GetComponent<IPoolingObject>().OriginPrefab = goPrefab;
            res = poolingObject;
        }
        else
        {
            res = poolDict[goPrefab].Dequeue();
        }

        res.transform.SetParent(null);
        res.transform.position = position;
        res.gameObject.SetActive(true);

        return res;
    }

    //자신이 Clone된 원본 프리팹을 찾습니다.
    private GameObject GetObjectOriginPrefab(GameObject go)
    {
        if (go.TryGetComponent(out IPoolingObject poolingObject))
        {
            return poolingObject.OriginPrefab;
        }

        return null;
    }


    //풀링된 오브젝트들을 다시 저장합니다.
    public void ReturnObject(GameObject returnObject)
    {
        GameObject prefabKey = GetObjectOriginPrefab(returnObject);

        if (poolDict.ContainsKey(prefabKey))
        {
            returnObject.transform.SetParent(transform);
            returnObject.gameObject.SetActive(false);
            returnObject.transform.position = transform.position;

            poolDict[prefabKey].Enqueue(returnObject);
        }
        else
        {
            Debug.LogError("풀링 되지 않은 오브젝트가 Return 되었습니다.");
        }
    }
        

    //풀에 있는 오브젝트와 월드에 있는 풀링 하는 오브젝트를 모두 파괴합니다.
    private void ClearPools()
    {
        var worldObjects = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var mono in worldObjects)
        {
            if (mono is IPoolingObject)
            {
                Destroy(mono.gameObject);
            }
        }

        foreach (var queue in poolDict.Values)
        {
            foreach (var go in queue)
            {
                if (go != null)
                    Destroy(go);
            }
            queue.Clear();
        }
        poolDict.Clear();
    }

    public void Clear()
    {
        ClearPools();
    }
}
