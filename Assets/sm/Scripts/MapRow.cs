using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapRow
{
    public List<Nodemap> nodes = new List<Nodemap>();
    public Nodemap selectedNode;
}
