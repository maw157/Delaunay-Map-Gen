using System.Collections.Generic;
using UnityEngine;
using Delaunay;

public class MapNode : MonoBehaviour, INode
{
    public Point Position { get; private set; }
    public List<INode> Neighbors { get; private set; }
    public List<HalfEdge> Connections { get; private set; }

    public MapNode(Point pos)
    {
        Position = pos;
    }
}
