using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//파티클에 이 컴포넌트를 붙여서 풀링을 합니다.

public class PoolingParticle : MonoBehaviour,IPoolingObject
{
    public GameObject OriginPrefab { get; set; }
    private ParticleSystem particle;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        particle = GetComponent<ParticleSystem>();
        var paticleMain = particle.main;
        paticleMain.stopAction = ParticleSystemStopAction.Callback;
    }


    public void ResetObject()
    {

    }

    private void OnParticleSystemStopped()
    {
        ReturnObject();
    }

    public void ReturnObject()
    {
        Managers.Pooling.ReturnObject(gameObject);
    }
}
