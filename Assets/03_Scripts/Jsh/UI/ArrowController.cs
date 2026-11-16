using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public static ArrowController instance { get; private set; }

    [SerializeField] private BezierArrow bezierArrow;

    public Vector3 StartPoint
    {
        set
        {
            if (bezierArrow != null)
                bezierArrow.StartPoint = value;
        }
    }

    private void Awake()
    {
        instance = this;
        if (bezierArrow == null)
        {
            bezierArrow = GetComponentInChildren<BezierArrow>(true); // (true)를 넣어야 꺼져있는 자식도 찾습니다.
        }
    }

    public void SetArrowActive(bool active, Vector3 startPos = default)
    {
        if (bezierArrow == null)
        {
            Debug.LogWarning("ArrowEmitter(BezierArrow)가 연결되지 않았습니다!");
            return;
        }

        if (active)
        {
            bezierArrow.gameObject.SetActive(active);
            this.StartPoint = startPos;
        }
        else
        {
            bezierArrow.gameObject.SetActive(false);
        }
    }

    public void SetArrowColor(Color color)
    {
        if (bezierArrow != null)
        {
            bezierArrow.SetColor(color);
        }
    }
}
