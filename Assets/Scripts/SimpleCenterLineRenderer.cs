using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class SimpleCenterLineRenderer : MonoBehaviour
{
    public SplineContainer splineContainer;
    public Material lineMaterial;

    public int segments = 500;
    public float lineWidth = 0.03f;
    public float lineHeight = 0.06f;

    private LineRenderer lr;

    void Awake()
    {
        SetupLine();
        DrawLine();
    }

    void Start()
    {
        SetupLine();
        DrawLine();
    }

    void OnValidate()
    {
        SetupLine();
        DrawLine();
    }

    void Update()
    {
        if (!Application.isPlaying)
            DrawLine();
    }

    private void SetupLine()
    {
        lr = GetComponent<LineRenderer>();

        lr.useWorldSpace = true;
        lr.positionCount = segments;
        lr.widthMultiplier = lineWidth;

        if (lineMaterial != null)
            lr.material = lineMaterial;
    }

    private void DrawLine()
    {
        if (splineContainer == null || lr == null)
            return;

        lr.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            Vector3 pos = splineContainer.EvaluatePosition(t);
            pos.y = lineHeight;

            lr.SetPosition(i, pos);
        }
    }
}