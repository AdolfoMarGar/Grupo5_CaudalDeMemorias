using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GrandpaCameraFollow : MonoBehaviour
{
    [Header("Objetivos")]
    [Tooltip("Transform del Player2D a seguir cuando todo va bien")]
    public Transform player;

    [Tooltip("Referencia al abuelo para seguirlo a él cuando se canse")]
    public GrandpaCompanion grandpa;

    [Header("Configuración de Seguimiento")]
    [Tooltip("Velocidad de suavizado del movimiento de la cámara")]
    public float smoothSpeed = 4f;

    [Tooltip("Offset respecto al objetivo actual (mantener Z en -10 para 2D)")]
    public Vector3 offset = new Vector3(0f, 1.2f, -10f);

    [Header("Límites de Pantalla para el Jugador")]
    [Tooltip("Margen respecto a los bordes laterales visibles de la cámara")]
    public float screenPadding = 0.5f;

    private Camera cam;
    private PlayerSideController playerController;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (player == null)
        {
            var p = FindAnyObjectByType<PlayerSideController>();
            if (p != null) player = p.transform;
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerSideController>();
        }

        if (grandpa == null)
        {
            grandpa = FindAnyObjectByType<GrandpaCompanion>();
        }

        if (player != null)
        {
            transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
        }
    }

    public void OnGrandpaTired()
    {
        // Se llama cuando el abuelo entra en estado IsTired
    }

    public void OnHandsResumed()
    {
        // Al retomar la mano, liberar inmediatamente los límites de la pantalla
        if (playerController != null)
        {
            playerController.SetCameraBounds(false, 0f, 0f);
        }
    }

    private void LateUpdate()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Si el abuelo está cansado, la cámara deja de seguir al jugador y sigue SOLO al abuelo
        if (grandpa != null && grandpa.IsTired)
        {
            // 1. Seguir suavemente al abuelo
            Vector3 targetGrandpaPos = new Vector3(
                grandpa.transform.position.x + offset.x,
                grandpa.transform.position.y + offset.y,
                offset.z
            );
            transform.position = Vector3.Lerp(transform.position, targetGrandpaPos, smoothSpeed * Time.deltaTime);

            // 2. Confinar al jugador dentro de los bordes laterales visibles de la cámara
            if (cam != null && cam.orthographic)
            {
                float halfWidth = cam.orthographicSize * cam.aspect;
                float minX = transform.position.x - halfWidth + screenPadding;
                float maxX = transform.position.x + halfWidth - screenPadding;

                if (playerController != null)
                {
                    playerController.SetCameraBounds(true, minX, maxX);
                }

                if (player != null)
                {
                    Vector3 p = player.position;
                    float clampedX = Mathf.Clamp(p.x, minX, maxX);
                    if (!Mathf.Approximately(p.x, clampedX))
                    {
                        player.position = new Vector3(clampedX, p.y, p.z);
                    }
                }
            }
        }
        else
        {
            // El abuelo no está cansado, liberar límites y seguir al jugador
            if (playerController != null)
            {
                playerController.SetCameraBounds(false, 0f, 0f);
            }

            if (player != null)
            {
                Vector3 targetPlayerPos = new Vector3(
                    player.position.x + offset.x,
                    player.position.y + offset.y,
                    offset.z
                );
                transform.position = Vector3.Lerp(transform.position, targetPlayerPos, smoothSpeed * Time.deltaTime);
            }
        }
    }
}
