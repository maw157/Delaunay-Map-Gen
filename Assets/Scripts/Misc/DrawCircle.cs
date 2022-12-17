using UnityEngine;

// code adapted from https://stackoverflow.com/questions/13708395/how-can-i-draw-a-circle-in-unity3d

public class DrawCircle : MonoBehaviour
{
    [SerializeField] float _radius = 1f;
    [SerializeField] public float _width = 0.02f;
    [SerializeField] [Range(6,60)] public int _lineCount = 25;
    [SerializeField] Color _color = Color.cyan;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = false;
    }

    public void SetDrawParams(float r, float w, int lineCount, Color color)
    {
        _radius = r;
        _width = w;
        _lineCount = lineCount;
        _color = color;
    }

    public void Draw()
    {
        _lineRenderer.positionCount = _lineCount;
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
        _lineRenderer.startColor = _color;

        float theta = (2f * Mathf.PI) / _lineCount;  //find radians per segment
        float angle = 0;

        for (int i = 0; i < _lineCount; i++)
        {
            float x = _radius * Mathf.Cos(angle);
            float y = _radius * Mathf.Sin(angle);
            _lineRenderer.SetPosition(i, new Vector2(x, y));
            angle += theta;
        }
    }
}