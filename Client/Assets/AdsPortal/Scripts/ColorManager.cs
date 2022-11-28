using UnityEngine;

public class ColorManager
{
    private Color m_ColorPrimary;
    private Color m_ColorSecondary;
    public Color ColorPrimary { get { return m_ColorPrimary; } private set { m_ColorPrimary = value; } }
    public Color ColorSecondary { get { return m_ColorSecondary; } private set { m_ColorSecondary = value; } }
    private Color TargetColorPrimary;
    private Color TargetColorSecondary;
    public static float s_ColorIncrement = 0.1f;

    private static ColorManager m_Instance;
    public static ColorManager Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = new ColorManager();
            }
            return m_Instance;
        }
    }

    private ColorManager()
    {
        GenerateNewColors();
    }

    public void GenerateNewColors()
    {
        ColorPrimary = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        ColorSecondary = GetSecondaryColor(ColorPrimary);
        GenerateNewTargetColors();
    }

    private void GenerateNewTargetColors()
    {
        TargetColorPrimary = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        TargetColorSecondary = GetSecondaryColor(TargetColorPrimary);
    }

    public Color GetSecondaryColor(Color sourceColor)
    {
        return new Color(1 - sourceColor.r, 1 - sourceColor.g, 1 - sourceColor.b, sourceColor.a);
    }

    private void UpdateColorComponent(ref float actualColorComponent, float targetColorComponent)
    {
        if (Mathf.Abs(actualColorComponent - targetColorComponent) > 0.1f)
        {
            if (actualColorComponent < targetColorComponent)
            {
                actualColorComponent += s_ColorIncrement * Time.deltaTime;
            }
            else
            {
                actualColorComponent -= s_ColorIncrement * Time.deltaTime;
            }
        }
    }

    public void Update()
    {
        // If we reached the desired color
        if (Mathf.Abs(m_ColorPrimary.r - TargetColorPrimary.r) < 0.1f && Mathf.Abs(m_ColorPrimary.g - TargetColorPrimary.g) < 0.1f && Mathf.Abs(m_ColorPrimary.b - TargetColorPrimary.b) < 0.1f)
        {
            GenerateNewTargetColors();
        }

        // Update each color component
        UpdateColorComponent(ref m_ColorPrimary.r, TargetColorPrimary.r);
        UpdateColorComponent(ref m_ColorPrimary.g, TargetColorPrimary.g);
        UpdateColorComponent(ref m_ColorPrimary.b, TargetColorPrimary.b);

        UpdateColorComponent(ref m_ColorSecondary.r, TargetColorSecondary.r);
        UpdateColorComponent(ref m_ColorSecondary.g, TargetColorSecondary.g);
        UpdateColorComponent(ref m_ColorSecondary.b, TargetColorSecondary.b);
    }
}
