using UnityEngine;

public class PlayerFog : MonoBehaviour
{
    [Header("Fog Settings")]
    public FogMode fogMode = FogMode.ExponentialSquared;
    public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Density (ExponentialSquared / Exponential)")]
    public float fogDensity = 0.05f;

    [Header("Distance (Linear mode only)")]
    public float fogStartDistance = 5f;
    public float fogEndDistance = 30f;

    [Header("Transition")]
    public float transitionSpeed = 2f;

    private float targetDensity;
    private float currentDensity;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;

        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fogEndDistance = fogEndDistance;

        currentDensity = fogDensity;
        targetDensity = fogDensity;
    }

    void Update()
    {
        currentDensity = Mathf.Lerp(currentDensity, targetDensity, Time.deltaTime * transitionSpeed);
        RenderSettings.fogDensity = currentDensity;

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fogEndDistance = fogEndDistance;
    }
    public void SetFogDensity(float density)
    {
        targetDensity = density;
    }

    public void SetFogColor(Color color)
    {
        fogColor = color;
    }

    public void DisableFog()
    {
        RenderSettings.fog = false;
    }

    public void EnableFog()
    {
        RenderSettings.fog = true;
    }
}