using System.Collections.Generic;
using UnityEngine;
using Poisson;

public class PoissonTest : MonoBehaviour
{
    [Header("Point Cloud Parameters")]
    [SerializeField] float _radius = 1;
    [SerializeField] Vector2 _regionSize = new Vector2(5, 5);
    [SerializeField] float _regionRadius = 5f;
    [SerializeField] int _rejectionSamples = 30;
    [SerializeField] List<Vector2> _seed = new List<Vector2>();

    [Header("Display Settings")]
    [SerializeField] bool _showCells = false;
    [SerializeField] bool _circularRegion = false;
    [SerializeField] bool _showCircularRegion = false;
    [SerializeField] bool _useSeed = false;

    [SerializeField] float _lineWidth = 0.02f;
    [SerializeField] [Range(6,60)] public int _lineCount = 25;
    [SerializeField] float _displayRadius = 1;
    [SerializeField] float _centerRadius = 0.03f;

    [Header("Prefabs and References")]
    [SerializeField] GameObject _drawCirclePrefab;
    [SerializeField] GameObject _parent;

    private List<Vector2> _points = new List<Vector2>();
    private List<DrawCircle> _poissonPoints = new List<DrawCircle>();
    private DrawCircle _regionCenter;

    private void Start()
    {
        GeneratePoints();
    }

    private void OnDrawGizmos()
    {
        if (_showCells)
        {
            float cellSize = _radius / Mathf.Sqrt(2);

            if (_circularRegion)
            {
                foreach (var cellCenter in GetCellCenters(_regionRadius, cellSize))
                {
                    Gizmos.DrawWireCube(cellCenter, new Vector2(cellSize, cellSize));
                }
            }
            else
            {                
                foreach (var cellCenter in GetCellCenters(_regionSize, cellSize))
                {
                    Gizmos.DrawWireCube(cellCenter, new Vector2(cellSize, cellSize));
                }
            }
        }

        if (_points.Count > 0)
        {
            foreach (var p in _points)
            {
                Gizmos.DrawSphere(p, _centerRadius);
            }
        }
    }

    public void GeneratePoints()
    {
        if (_poissonPoints.Count > 0)
        {
            foreach (var p in _poissonPoints)
            {
                Destroy(p.gameObject);
            }

            _poissonPoints.Clear();
        }

        if (_regionCenter != null)
        {
            Destroy(_regionCenter.gameObject);
        }

        if (_useSeed)
        {
            if (_circularRegion)
            {
                _points = PoissonDiskSampling.GenerateSamples2D(_regionRadius, _radius, _seed, _rejectionSamples);
            }
            else
            {
                _points = PoissonDiskSampling.GenerateSamples2D(_regionSize, _radius, _seed, _rejectionSamples);
            }
        }
        else
        {
            if (_circularRegion)
            {
                _points = PoissonDiskSampling.GenerateSamples2D(_regionRadius, _radius, _rejectionSamples);

                if (_showCircularRegion)
                {
                    Vector3 centerPos = new Vector3(_regionRadius, _regionRadius, 0);

                    _regionCenter = Instantiate(_drawCirclePrefab, centerPos, Quaternion.identity).GetComponent<DrawCircle>();
                    _regionCenter.SetDrawParams(_regionRadius, _lineWidth, _lineCount, Color.blue);
                    _regionCenter.Draw();
                }
            }
            else
            {
                _points = PoissonDiskSampling.GenerateSamples2D(_regionSize, _radius, _rejectionSamples);
            }
        }

        foreach (var p in _points)
        {
            Vector3 vector3 = p;

            DrawCircle temp = Instantiate(_drawCirclePrefab, vector3, Quaternion.identity, _parent.transform).GetComponent<DrawCircle>();
            _poissonPoints.Add(temp);

            temp.SetDrawParams(_displayRadius, _lineWidth, _lineCount, Color.magenta);
            temp.Draw();
        }
    }

    private List<Vector2> GetCellCenters(Vector2 regionSize, float cellSize)
    {
        List<Vector2> centers = new List<Vector2>();

        int cellCountX = Mathf.CeilToInt(regionSize.x / cellSize);
        int cellCountY = Mathf.CeilToInt(regionSize.y / cellSize);

        for (int x = 0; x < cellCountX; x++)
        {
            for (int y = 0; y < cellCountY; y++)
            {
                float cellCenterX = x * cellSize + cellSize / 2;
                float cellCenterY = y * cellSize + cellSize / 2;

                centers.Add(new Vector2(cellCenterX, cellCenterY));
            }
        }

        return centers;
    }

    private List<Vector2> GetCellCenters(float regionRadius, float cellSize)
    {
        List<Vector2> centers = new List<Vector2>();

        int cellCountX = Mathf.CeilToInt(2 * regionRadius / cellSize);
        int cellCountY = Mathf.CeilToInt(2 * regionRadius / cellSize);

        for (int x = 0; x < cellCountX; x++)
        {
            for (int y = 0; y < cellCountY; y++)
            {
                float cellCenterX = x * cellSize + cellSize / 2;
                float cellCenterY = y * cellSize + cellSize / 2;

                centers.Add(new Vector2(cellCenterX, cellCenterY));
            }
        }

        return centers;
    }
}
