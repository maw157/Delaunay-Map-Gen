using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;

public class Map : MonoBehaviour, IGraph
{
    public List<INode> AdjacencyList { get; private set; }

    public Map(List<Triangle> triangulation)
    {
        List<HalfEdge> edges = new List<HalfEdge>();
        IGraph self = this;

        foreach (var t in triangulation)
        {
            foreach (var p in t.Points)
            {
                if (AdjacencyList.Exists(n => n.Position == p))
                    continue;
                
                MapNode node = new MapNode(p);
                AdjacencyList.Add(node);
            }

            foreach (var e in t.Edges)
            {
                if (edges.Contains(e))
                    continue;
                
                edges.Add(e);
            }
        }

        foreach (var e in edges)
        {
            self.AddConnection(e);
        }
    }
}