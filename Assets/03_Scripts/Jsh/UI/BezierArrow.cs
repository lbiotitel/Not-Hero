
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierArrow : MonoBehaviour
{
    [SerializeField] private GameObject ArrowHeadPrefab;
    [SerializeField] private GameObject ArrowNodePrefab;
    [SerializeField] private int arrowNodeNum;
    [SerializeField] private float scaleFactor = 1f;

    private RectTransform origin;

    private List<RectTransform> arrowNodes = new List<RectTransform>();

    private List<Vector2> controlPoints = new List<Vector2>();
    public Vector2 StartPoint
    {
        set
        {
            if (controlPoints.Count == 0) return; // Awake 이전 호출 방지

            // CardUI에서 넘어온 World 좌표를 부모의 Local 좌표로 변환
            Vector3 localPos = transform.parent.InverseTransformPoint(value);
            controlPoints[0] = localPos;
        }
    }

    private readonly List<Vector2> controlPointFactors = new List<Vector2> { new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f) };

    private void Awake()
    {
        this.origin = this.GetComponent<RectTransform>();
        //Debug.Log($"[Init] Origin: {origin.position}, Nodes Count: {arrowNodes.Count}");

        for (int i = 0; i < this.arrowNodeNum; i++)
        {
            this.arrowNodes.Add(Instantiate(this.ArrowNodePrefab, this.transform).GetComponent<RectTransform>());
        }

        this.arrowNodes.Add(Instantiate(this.ArrowHeadPrefab, this.transform).GetComponent<RectTransform>());

        this.arrowNodes.ForEach(a => a.GetComponent<RectTransform>().position = new Vector2(-1000, -1000));

        for (int i = 0; i < 4; i++)
        {
            this.controlPoints.Add(Vector2.zero);
        }
    }

    private void Update()
    {
        //P0
        //this.controlPoints[0] = new Vector2(this.origin.position.x, this.origin.position.y);

        //P3
        if (controlPoints.Count < 4 || arrowNodes.Count == 0) return;

        // P3: 마우스의 Screen 좌표를 부모의 Local 좌표로 변환
        Vector2 mouseLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            null,
            out mouseLocalPos
        );
        this.controlPoints[3] = mouseLocalPos;
        //this.controlPoints[3] = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        //P1 = P0 + (P3 - P0) * Vector2(-0.3f, 0.8f)
        //P2 = P0 + (P3 - P0) * Vector2(0.1f, 1.4f)
        this.controlPoints[1] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[0];
        this.controlPoints[2] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[1];

        for (int i = 0; i < this.arrowNodes.Count - 1; i++)
        {
            //t 계산
            float t = (float)i / (this.arrowNodes.Count - 2) * 0.91f;

            //B( t ) = (1 - t ) ³P₀ + 3(1 - t ) ²tP₁ + 3(1 - t )t²P₂ + t³P₃
            this.arrowNodes[i].localPosition =
                Mathf.Pow(1 - t, 3) * this.controlPoints[0] +
                3 * Mathf.Pow(1 - t, 2) * t * this.controlPoints[1] +
                3 * (1 - t) * Mathf.Pow(t, 2) * this.controlPoints[2] +
                Mathf.Pow(t, 3) * this.controlPoints[3];

            if (i > 0)
            {
                var euler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[i].position - this.arrowNodes[i - 1].position));
                this.arrowNodes[i].rotation = Quaternion.Euler(euler);
            }

            float progress = (float)i / (this.arrowNodes.Count - 1);
            float curve = Mathf.Lerp(0.4f, 1.0f, Mathf.Pow(progress, 2f));
            float scale = this.scaleFactor * curve;
            this.arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
        }


        int headIndex = this.arrowNodes.Count - 1;
        this.arrowNodes[headIndex].localPosition = this.controlPoints[3]; // 마우스 위치 고정

        // 머리의 회전: 바로 전 마디를 바라보게 설정
        var headEuler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[headIndex].position - this.arrowNodes[headIndex - 1].position));
        this.arrowNodes[headIndex].localRotation = Quaternion.Euler(headEuler);

        // 머리 스케일은 가장 크게
        this.arrowNodes[headIndex].localScale = new Vector3(this.scaleFactor * 0.9f, this.scaleFactor *0.9f, 1.5f);

        this.arrowNodes[0].transform.localRotation = this.arrowNodes[1].transform.localRotation;
    }

    public void ShowArrowActive(bool active, Vector3 startPos = default)
    {
        if (active)
        {
            this.StartPoint = startPos;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetColor(Color targetColor)
    {
        for (int i = 0; i < arrowNodes.Count; i++)
        {
            if (arrowNodes[i].TryGetComponent<Image>(out Image img))
            {
                img.color = targetColor;
            }
        }
    }
}
