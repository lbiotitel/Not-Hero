using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//적이 아군을 타겟팅 하는 곡선을 그립니다
public class TargetLine : MonoBehaviour
{
    [SerializeField] private int resolution = 20; 
    [SerializeField] private float curveHeight = 2.0f;
    [SerializeField] private float animSpeed = 2f;
    [SerializeField] private Vector3 offset;
    private LineRenderer lineRenderer;
    private Transform endPoint;
    private Transform startPoint;


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DoLineAnimation();
    }


    private void DoLineAnimation()
    {
        float offset = Time.time * animSpeed;
        lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(-offset, 0));
    }

    public void Show(Transform startPoint, Transform endPoint)
    {
        gameObject.SetActive(true);

        this.startPoint = startPoint;
        this.endPoint = endPoint;

        lineRenderer.positionCount = resolution;

        Vector3 midPoint = (this.startPoint.position + this.endPoint.position) / 2;
        Vector3 controlPoint = midPoint + Vector3.up * curveHeight;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            lineRenderer.SetPosition(i, CalculateBezierPoint(t, this.startPoint.position + offset, 
                controlPoint + offset,
                this.endPoint.position + offset));
        }
    }

    public void Hide() => gameObject.SetActive(false);

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }
}
