using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nodemap : MonoBehaviour
{
    public int row;
    public int col;

    public List<Nodemap> nextNode = new List<Nodemap>();
    public List<Nodemap> prevNode = new List<Nodemap>();

    public bool isSelectNode = false;

    Button button;
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (image != null)
            image.color = isSelectNode ? Color.white : Color.gray;
    }

    public void OnClick()
    {
        if (!isSelectNode) return;

        MapManager.Instance.SelectNode(this);
    }
}