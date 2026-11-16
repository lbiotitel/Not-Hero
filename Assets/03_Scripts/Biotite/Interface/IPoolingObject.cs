using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//풀링하는 오브젝트들은 이 인터페이스를 다 상속 받습니다.
public interface IPoolingObject 
{
    public GameObject OriginPrefab { get; set; }
    public void ResetObject();
    public void ReturnObject();
}
