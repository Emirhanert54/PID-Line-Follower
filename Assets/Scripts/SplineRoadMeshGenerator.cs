using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SplineRoadMeshGenerator : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;

    [Header("Road Settings")]
    public float roadWidth = 6f;
    public int segments = 400;
    public float roadHeight = -0.02f;

    [Header("Material")]
    public Material roadMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        EnsureComponents();
        GenerateRoadMesh();
    }

    void Start()
    {
        EnsureComponents();
        GenerateRoadMesh();
    }

    void OnValidate()
    {
        EnsureComponents();
        GenerateRoadMesh();
    }

    public void GenerateRoadMesh()
    {
        EnsureComponents();

        if (splineContainer == null)
            return;

        Mesh mesh = new Mesh();
        mesh.name = "Generated Spline Road Mesh";

        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        Vector2[] uv = new Vector2[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 center = splineContainer.EvaluatePosition(t);

            float nextT = Mathf.Clamp01(t + 0.002f);
            float prevT = Mathf.Clamp01(t - 0.002f);

            Vector3 next = splineContainer.EvaluatePosition(nextT);
            Vector3 prev = splineContainer.EvaluatePosition(prevT);

            Vector3 forward = next - prev;
            forward.y = 0f;

            if (forward.magnitude < 0.001f)
                forward = Vector3.forward;

            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            Vector3 leftPoint = center - right * roadWidth * 0.5f;
            Vector3 rightPoint = center + right * roadWidth * 0.5f;

            leftPoint.y = roadHeight;
            rightPoint.y = roadHeight;

            vertices[i * 2] = transform.InverseTransformPoint(leftPoint);
            vertices[i * 2 + 1] = transform.InverseTransformPoint(rightPoint);

            uv[i * 2] = new Vector2(0f, t * 20f);
            uv[i * 2 + 1] = new Vector2(1f, t * 20f);
        }

        int triIndex = 0;

        for (int i = 0; i < segments; i++)
        {
            int index = i * 2;

            triangles[triIndex++] = index;
            triangles[triIndex++] = index + 2;
            triangles[triIndex++] = index + 1;

            triangles[triIndex++] = index + 1;
            triangles[triIndex++] = index + 2;
            triangles[triIndex++] = index + 3;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;

        if (roadMaterial != null)
            meshRenderer.sharedMaterial = roadMaterial;
    }

    private void EnsureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }
}