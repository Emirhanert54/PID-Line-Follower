using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiveGraphUI : MonoBehaviour
{
    [Header("References")]
    public PIDLaneFollower pidCar;
    public RectTransform errorGraphPanel;
    public RectTransform controlGraphPanel;

    [Header("Graph Settings")]
    public int maxPoints = 180;
    public float errorScale = 70f;
    public float controlScale = 35f;

    [Header("Limits")]
    public float errorGoodLimit = 0.25f;
    public float errorBadLimit = 0.7f;

    private List<float> errorValues = new List<float>();
    private List<float> controlValues = new List<float>();

    private List<GameObject> dynamicObjects = new List<GameObject>();

    private TextMeshProUGUI errorValueText;
    private TextMeshProUGUI outputValueText;

    void Start()
    {
        SetupPanel(errorGraphPanel, "Error e(t)");
        SetupPanel(controlGraphPanel, "Output u(t)");

        errorValueText = CreateValueText(errorGraphPanel);
        outputValueText = CreateValueText(controlGraphPanel);
    }

    void Update()
    {
        if (pidCar == null)
            return;

        AddValue(errorValues, pidCar.error_e);
        AddValue(controlValues, pidCar.controllerOutput_u);

        ClearDynamicObjects();

        DrawGraph(errorGraphPanel, errorValues, errorScale, true);
        DrawGraph(controlGraphPanel, controlValues, controlScale, false);

        errorValueText.text = pidCar.error_e.ToString("0.00") + " m";
        outputValueText.text = pidCar.controllerOutput_u.ToString("0.00");
    }

    private void AddValue(List<float> values, float value)
    {
        values.Add(value);

        if (values.Count > maxPoints)
            values.RemoveAt(0);
    }

    private void SetupPanel(RectTransform panel, string title)
    {
        Image bg = panel.GetComponent<Image>();

        if (bg != null)
            bg.color = new Color(0f, 0f, 0f, 0.55f);

        CreateTitle(panel, title);
        CreateGrid(panel);
        CreateZeroLine(panel);
    }

    private void DrawGraph(RectTransform panel, List<float> values, float scale, bool isError)
    {
        if (values.Count < 2)
            return;

        float width = panel.rect.width;
        float height = panel.rect.height;

        for (int i = 1; i < values.Count; i++)
        {
            float x1 = ((i - 1) / (float)(maxPoints - 1)) * width - width / 2f;
            float x2 = (i / (float)(maxPoints - 1)) * width - width / 2f;

            float y1 = Mathf.Clamp(values[i - 1] * scale, -height / 2f + 15f, height / 2f - 15f);
            float y2 = Mathf.Clamp(values[i] * scale, -height / 2f + 15f, height / 2f - 15f);

            Color color;

            if (isError)
            {
                float absError = Mathf.Abs(values[i]);

                if (absError > errorBadLimit)
                    color = Color.red;
                else if (absError > errorGoodLimit)
                    color = Color.yellow;
                else
                    color = Color.green;
            }
            else
            {
                color = Color.cyan;
            }

            CreateLine(panel, new Vector2(x1, y1), new Vector2(x2, y2), color, 2f, true);
        }
    }

    private void CreateGrid(RectTransform panel)
    {
        float width = panel.rect.width;
        float height = panel.rect.height;

        Color gridColor = new Color(1f, 1f, 1f, 0.18f);

        for (int i = 1; i < 4; i++)
        {
            float x = -width / 2f + i * width / 4f;
            CreateLine(panel, new Vector2(x, -height / 2f), new Vector2(x, height / 2f), gridColor, 1f, false);
        }

        for (int i = 1; i < 3; i++)
        {
            float y = -height / 2f + i * height / 3f;
            CreateLine(panel, new Vector2(-width / 2f, y), new Vector2(width / 2f, y), gridColor, 1f, false);
        }
    }

    private void CreateZeroLine(RectTransform panel)
    {
        float width = panel.rect.width;

        CreateLine(
            panel,
            new Vector2(-width / 2f, 0f),
            new Vector2(width / 2f, 0f),
            new Color(1f, 1f, 1f, 0.45f),
            2f,
            false
        );
    }

    private void CreateLine(RectTransform parent, Vector2 start, Vector2 end, Color color, float thickness, bool dynamic)
    {
        GameObject obj = new GameObject("UILine");
        obj.transform.SetParent(parent, false);

        Image img = obj.AddComponent<Image>();
        img.color = color;

        RectTransform rt = obj.GetComponent<RectTransform>();

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        rt.sizeDelta = new Vector2(distance, thickness);
        rt.anchoredPosition = start + direction / 2f;
        rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        if (dynamic)
            dynamicObjects.Add(obj);
    }

    private void CreateTitle(RectTransform panel, string title)
    {
        GameObject obj = new GameObject(title);
        obj.transform.SetParent(panel, false);

        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(panel.rect.width - 20f, 25f);
        rt.anchoredPosition = new Vector2(10f, panel.rect.height / 2f - 15f);
    }

    private TextMeshProUGUI CreateValueText(RectTransform panel)
    {
        GameObject obj = new GameObject("ValueText");
        obj.transform.SetParent(panel, false);

        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = "0.00";
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Right;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120f, 25f);
        rt.anchoredPosition = new Vector2(panel.rect.width / 2f - 65f, panel.rect.height / 2f - 15f);

        return text;
    }

    private void ClearDynamicObjects()
    {
        for (int i = dynamicObjects.Count - 1; i >= 0; i--)
        {
            Destroy(dynamicObjects[i]);
        }

        dynamicObjects.Clear();
    }
}