using UnityEngine;

/// <summary>
/// Proporciona un movimiento de cámara suave y orgánico para el fondo del menú principal,
/// evitando la sensación de imagen estática (Ken Burns / Camera Drift / Parallax).
/// </summary>
public class MenuCameraMotion : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("RectTransform del fondo que se moverá (por defecto el propio objeto)")]
    public RectTransform backgroundRect;

    [Tooltip("Contenedor del título para aplicar un sutil efecto de profundidad/paralaje (opcional)")]
    public RectTransform titleRect;

    [Header("Escala Base y Margen")]
    [Tooltip("Escala base del fondo para evitar que se vean los bordes al desplazarse")]
    [Range(1.02f, 1.25f)]
    public float baseScale = 1.08f;

    [Header("Deriva Automática Continua (Idle Drift)")]
    [Tooltip("Activa el suave vaivén orgánico continuo")]
    public bool enableIdleDrift = true;

    [Tooltip("Velocidad del movimiento continuo")]
    [Range(0.05f, 1.5f)]
    public float driftSpeed = 0.25f;

    [Tooltip("Amplitud máxima horizontal en píxeles")]
    [Range(0f, 60f)]
    public float driftAmountX = 24f;

    [Tooltip("Amplitud máxima vertical en píxeles")]
    [Range(0f, 40f)]
    public float driftAmountY = 14f;

    [Header("Respiración / Zoom Lento (Ken Burns)")]
    [Tooltip("Activa una respiración lenta de zoom in / zoom out")]
    public bool enableZoomPulse = true;

    [Tooltip("Velocidad del ciclo de respiración")]
    [Range(0.02f, 0.5f)]
    public float zoomSpeed = 0.12f;

    [Tooltip("Variación de escala durante la respiración")]
    [Range(0f, 0.08f)]
    public float zoomAmount = 0.025f;

    [Header("Rotación / Inclinación Sutil")]
    [Tooltip("Ligera inclinación angular que simula brisa o cámara cinematográfica")]
    public bool enableTilt = true;

    [Tooltip("Ángulo máximo en grados")]
    [Range(0f, 1.5f)]
    public float maxTiltAngle = 0.45f;

    [Header("Paralaje con el Ratón (Mouse Parallax)")]
    [Tooltip("Responde suavemente a la posición del cursor del jugador")]
    public bool enableMouseParallax = true;

    [Tooltip("Intensidad del desplazamiento provocado por el ratón")]
    [Range(0f, 50f)]
    public float mouseStrength = 18f;

    [Tooltip("Suavidad de la amortiguación del ratón (cuanto mayor, más suave)")]
    [Range(1f, 15f)]
    public float mouseSmoothness = 3.5f;

    [Header("Efecto de Profundidad en el Título")]
    [Tooltip("Fracción de paralaje inverso aplicada al título (crea sensación 3D)")]
    [Range(0f, 0.5f)]
    public float titleParallaxFactor = 0.15f;

    // Variables internas de suavizado
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

        // Semillas aleatorias para que cada sesión inicie en un punto armónico natural
        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(100f, 200f);
        seedRot = Random.Range(200f, 300f);
    }

    private void Update()
    {
        if (backgroundRect == null) return;

        float time = Time.time;

        // 1. Deriva continua orgánica (combinación de ondas sinusoidales armónicas)
        Vector2 driftOffset = Vector2.zero;
        if (enableIdleDrift)
        {
            float tX = (time * driftSpeed) + seedX;
            float tY = (time * driftSpeed * 0.85f) + seedY;

            float x = Mathf.Sin(tX) * driftAmountX + Mathf.Sin(tX * 0.43f) * (driftAmountX * 0.35f);
            float y = Mathf.Cos(tY) * driftAmountY + Mathf.Cos(tY * 0.51f) * (driftAmountY * 0.35f);
            driftOffset = new Vector2(x, y);
        }

        // 2. Paralaje suave con el ratón
        if (enableMouseParallax)
        {
            // Coordenadas normalizadas del ratón (-1 a 1 respecto al centro de pantalla)
            float normX = (Input.mousePosition.x / Mathf.Max(1f, Screen.width) - 0.5f) * 2f;
            float normY = (Input.mousePosition.y / Mathf.Max(1f, Screen.height) - 0.5f) * 2f;

            normX = Mathf.Clamp(normX, -1f, 1f);
            normY = Mathf.Clamp(normY, -1f, 1f);

            targetMouseOffset = new Vector2(-normX * mouseStrength, -normY * (mouseStrength * 0.6f));
            currentMouseOffset = Vector2.Lerp(currentMouseOffset, targetMouseOffset, Time.deltaTime * mouseSmoothness);
        }
        else
        {
            currentMouseOffset = Vector2.Lerp(currentMouseOffset, Vector2.zero, Time.deltaTime * mouseSmoothness);
        }

        // 3. Aplicar posición final al fondo
        backgroundRect.anchoredPosition = initialBackgroundPos + driftOffset + currentMouseOffset;

        // 4. Respiración de escala (Zoom suave Ken Burns)
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

        // 5. Inclinación angular sutil (Tilt)
        if (enableTilt && maxTiltAngle > 0f)
        {
            float angle = Mathf.Sin((time * driftSpeed * 0.7f) + seedRot) * maxTiltAngle;
            backgroundRect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            backgroundRect.localRotation = Quaternion.identity;
        }

        // 6. Paralaje sutil en el título para sensación tridimensional
        if (titleRect != null && titleParallaxFactor > 0f)
        {
            Vector2 titleOffset = -(driftOffset + currentMouseOffset) * titleParallaxFactor;
            titleRect.anchoredPosition = initialTitlePos + titleOffset;
        }
    }
}
