using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
using Poisson;

public class MapGenerator : MonoBehaviour
{
    public IGraph Map { get; private set; }
    private List<Triangle> DT;

    [SerializeField] Vector2 region = new Vector2(5,10);
    [SerializeField] float radius = 1;

    public void InitializeGraph(List<Vector2> seed)
    {
        DT = DelaunayTriangulation.Triangulate2D(PoissonDiskSampling.GenerateSamples2D(region, radius, seed));
    }

    public List<HalfEdge> ConditionMap(List<Triangle> DT)
    {
        return new List<HalfEdge>();
    }

    public bool isPrevious(MapNode node, MapNode testNode)
    {
        return testNode.Position.Y < node.Position.Y;
    }
}
