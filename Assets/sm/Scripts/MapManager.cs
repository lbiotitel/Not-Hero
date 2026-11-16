using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<MapRow>rows = new List<MapRow>();
    public int currentRowIndex = 0;

    public GameObject nodePrefab;
    public Transform mapRoot;

    public int rowCount = 15;
    public int columnCount = 7;

    public float xSpacing = 2.5f;
    public float ySpacing = 3.5f;

    public int startNodeCount = 4;

    public static MapManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Nodemap GetCurrentNode()
    {
        return rows[currentRowIndex].selectedNode;
    }

    public void GenerateMap()
    {
        rows.Clear();

        for(int r = 0; r < rowCount; r++)
        {
            MapRow newRow = new MapRow();

            for(int c = 0; c < columnCount; c++)
            {
                float offset = (r % 2 == 0) ? 0f : xSpacing * 0.5f;

                Vector2 pos = new  Vector2(c *  xSpacing + offset, r * ySpacing);

                GameObject obj = Instantiate(nodePrefab, mapRoot);

                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.anchoredPosition = pos;

                Nodemap node = obj.GetComponent<Nodemap>();
                node.row = r;
                node.col = c;

                newRow.nodes.Add(node);
            }

            rows.Add(newRow);
        }

        ConnectNodes();
        SetStartNodes();
    }

    public void ConnectNodes()
    {
        for (int r = 0; r < rows.Count - 1; r++)
        {
            for (int c = 0; c < rows[r].nodes.Count; c++)
            {
                Nodemap current = rows[r].nodes[c];

                int connectionCount = Random.Range(1, 3);

                for (int i = 0; i < connectionCount; i++)
                {
                    int offset = Random.Range(-1, 2);
                    int targetColumn = c + offset;

                    if (targetColumn >= 0 && targetColumn < columnCount)
                    {
                        Nodemap target = rows[r + 1].nodes[targetColumn];

                        if (!current.nextNode.Contains(target))
                        {
                            current.nextNode.Add(target);
                            target.prevNode.Add(current);
                        }
                    }
                }
            }
        }
    }

    public void SetStartNodes()
    {
        MapRow firstRow = rows[0];

        List<int> selectedIndexes = new List<int>();

        while (selectedIndexes.Count < startNodeCount)
        {
            int rand = Random.Range(0, firstRow.nodes.Count);

            if (!selectedIndexes.Contains(rand))
                selectedIndexes.Add(rand);
        }

        foreach (int index in selectedIndexes)
        {
            Nodemap node = firstRow.nodes[index];
            node.isSelectNode = true; // 시작 가능 표시
        }
        Debug.Log("");
        Debug.Log("첫 줄 노드 개수: " + rows[0].nodes.Count);
    }

    public void SelectNode(Nodemap node)
    {
        rows[currentRowIndex].selectedNode = node;

        // 현재 줄 비활성화
        foreach (var n in rows[currentRowIndex].nodes)
            n.isSelectNode = false;

        currentRowIndex++;

        if (currentRowIndex >= rows.Count)
            return;

        ActivateNextNodes(node);
    }

    void ActivateNextNodes(Nodemap selected)
    {
        foreach (var next in selected.nextNode)
        {
            next.isSelectNode = true;
        }
    }
}
