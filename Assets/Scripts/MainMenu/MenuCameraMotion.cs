using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCameraMotion : MonoBehaviour
{
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private RectTransform titleRect;

    [Range(1.02f, 1.25f)]
    [SerializeField] private float baseScale = 1.08f;

    [SerializeField] private bool enableIdleDrift = true;

    [Range(0.05f, 1.5f)]
    [SerializeField] private float driftSpeed = 0.25f;

    [Range(0f, 60f)]
    [SerializeField] private float driftAmountX = 24f;

    [Range(0f, 40f)]
    [SerializeField] private float driftAmountY = 14f;

    [SerializeField] private bool enableZoomPulse = true;

    [Range(0.02f, 0.5f)]
    [SerializeField] private float zoomSpeed = 0.12f;

    [Range(0f, 0.08f)]
    [SerializeField] private float zoomAmount = 0.025f;

    [SerializeField] private bool enableTilt = true;

    [Range(0f, 1.5f)]
    [SerializeField] private float maxTiltAngle = 0.45f;

    [SerializeField] private bool enableMouseParallax = true;

    [Range(0f, 50f)]
    [SerializeField] private float mouseStrength = 18f;

    [Range(1f, 15f)]
    [SerializeField] private float mouseSmoothness = 3.5f;

    [Range(0f, 0.5f)]
    [SerializeField] private float titleParallaxFactor = 0.15f;

    private Vector2 currentMouseOffset;
    private Vector2 targetMouseOffset;
    private Vector2 initialBackgroundPos;
    private Vector2 initialTitlePos;
    private float seedX;
    private float seedY;
    private float seedRot;

    private void Awake()
    {
        if (backgroundRect == null)
        {
            backgroundRect = GetComponent<RectTransform>();
        }

        if (backgroundRect != null)
        {
            initialBackgroundPos = backgroundRect.anchoredPosition;
            backgroundRect.localScale = new Vector3(baseScale, baseScale, 1f);
        }

        if (titleRect != null)
        {
            initialTitlePos = titleRect.anchoredPosition;
        }

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(100f, 200f);
        seedRot = Random.Range(200f, 300f);
    }

    private void Update()
    {
        if (backgroundRect == null) return;

        float time = Time.time;

        Vector2 driftOffset = Vector2.zero;
        if (enableIdleDrift)
        {
            float tX = (time * driftSpeed) + seedX;
            float tY = (time * driftSpeed * 0.85f) + seedY;

            float x = Mathf.Sin(tX) * driftAmountX + Mathf.Sin(tX * 0.43f) * (driftAmountX * 0.35f);
            float y = Mathf.Cos(tY) * driftAmountY + Mathf.Cos(tY * 0.51f) * (driftAmountY * 0.35f);
            driftOffset = new Vector2(x, y);
        }

        if (enableMouseParallax && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float normX = (mousePos.x / Mathf.Max(1f, Screen.width) - 0.5f) * 2f;
            float normY = (mousePos.y / Mathf.Max(1f, Screen.height) - 0.5f) * 2f;

            normX = Mathf.Clamp(normX, -1f, 1f);
            normY = Mathf.Clamp(normY, -1f, 1f);

            targetMouseOffset = new Vector2(-normX * mouseStrength, -normY * (mouseStrength * 0.6f));
            currentMouseOffset = Vector2.Lerp(currentMouseOffset, targetMouseOffset, Time.deltaTime * mouseSmoothness);
        }
        else
        {
            currentMouseOffset = Vector2.Lerp(currentMouseOffset, Vector2.zero, Time.deltaTime * mouseSmoothness);
        }

        backgroundRect.anchoredPosition = initialBackgroundPos + driftOffset + currentMouseOffset;

        if (enableZoomPulse)
        {
            float zoomPhase = Mathf.Sin(time * zoomSpeed) * zoomAmount;
            float scale = baseScale + zoomPhase;
            backgroundRect.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            backgroundRect.localScale = new Vector3(baseScale, baseScale, 1f);
        }

        if (enableTilt && maxTiltAngle > 0f)
        {
            float angle = Mathf.Sin((time * driftSpeed * 0.7f) + seedRot) * maxTiltAngle;
            backgroundRect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            backgroundRect.localRotation = Quaternion.identity;
        }

        if (titleRect != null && titleParallaxFactor > 0f)
        {
            Vector2 titleOffset = -(driftOffset + currentMouseOffset) * titleParallaxFactor;
            titleRect.anchoredPosition = initialTitlePos + titleOffset;
        }
    }
}
