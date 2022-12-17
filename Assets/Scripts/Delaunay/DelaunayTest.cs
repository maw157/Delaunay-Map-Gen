using System.Collections.Generic;
using UnityEngine;
using Poisson;
using Delaunay;

public class DelaunayTest : MonoBehaviour
{
    [Header("Point Cloud Parameters")]
    [SerializeField] float _radius = 1;
    [SerializeField] Vector2 _regionSize = new Vector2(5, 5);
    [SerializeField] int _rejectionSamples = 30;
    [SerializeField] float _regionRadius = 5;
    [SerializeField] bool _circularRegion = false;

    [Header("Display Settings")]
    [SerializeField] float _pointRadius = 0.03f;
    [SerializeField] float _lineWidth = 0.02f;
    [SerializeField] [Range(6,60)] public int _lineCount = 25;
    [SerializeField] bool _showCircumcircles = true;

    [Header("Prefabs and References")]
    [SerializeField] GameObject _drawCirclePrefab;
    [SerializeField] GameObject _parent;

    private List<Triangle> _triangles = new List<Triangle>();
    private List<Vector2> _points = new List<Vector2>();
    private List<HalfEdge> _edges = new List<HalfEdge>();

    private List<Circumcircle> _circumcircles = new List<Circumcircle>();
    private List<DrawCircle> _drawnCircles = new List<DrawCircle>();

    private void Start()
    {
        GenerateTriangulation();
    }

    private void Update()
    {
        if (_showCircumcircles)
        {
            foreach (var c in _circumcircles)
            {
                Vector3 center = c.Center;
                float circumradius = c.Radius;

                DrawCircle temp = Instantiate(_drawCirclePrefab, center, Quaternion.identity, _parent.transform).GetComponent<DrawCircle>();
                _drawnCircles.Add(temp);

                temp.SetDrawParams(circumradius, _lineWidth, _lineCount, Color.magenta);
                temp.Draw();
            }
        }
        else if (_drawnCircles.Count > 0)
        {
            foreach (var c in _drawnCircles)
            {
                if (c.gameObject != null)
                    Destroy(c.gameObject);
            }

            _drawnCircles.Clear();
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var e in _edges)
        {
            Gizmos.DrawLine(PointToVec2(e.P), PointToVec2(e.Q));
        }

        foreach (var p in _points)
        {
            Gizmos.DrawSphere(p, _pointRadius);
        }
    }

    public void GenerateTriangulation()
    {
        if (_drawnCircles.Count > 0)
        {
            foreach (var c in _drawnCircles)
            {
                if (c.gameObject != null)
                    Destroy(c.gameObject);
            }

            _drawnCircles.Clear();
        }

        _circumcircles.Clear();
        _edges.Clear();

        if (_circularRegion)
        {
            _points = PoissonDiskSampling.GenerateSamples2D(_regionRadius, _radius, _rejectionSamples);
        }
        else
        {
            _points = PoissonDiskSampling.GenerateSamples2D(_regionSize, _radius, _rejectionSamples);
        }
        
        _triangles = DelaunayTriangulation.Triangulate2D(_points);

        foreach (var t in _triangles)
        {
            _circumcircles.Add(new Circumcircle(t));

            foreach (var e in t.Edges)
            {
                if (!_edges.Contains(e) && !_edges.Contains(e.Reverse()))
                    _edges.Add(e);
            }
        }
    }

    private Vector2 PointToVec2(Point p)
    {
        return new Vector2(p.X, p.Y);
    }
}