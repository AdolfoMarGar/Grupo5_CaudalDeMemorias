using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GrandpaCameraFollow : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform player;
    public GrandpaCompanion grandpa;

    [Header("Configuración de Seguimiento")]
    public float smoothSpeed = 4f;
    public Vector3 offset = new Vector3(0f, 1.2f, -10f);

    [Header("Límites de Pantalla para el Jugador")]
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
    }

    public void OnHandsResumed()
    {
        if (playerController != null)
        {
            playerController.SetCameraBounds(false, 0f, 0f);
        }
    }

    private void LateUpdate()
    {
        if (cam == null) cam = GetComponent<Camera>();

        if (grandpa != null && grandpa.IsTired)
        {
            Vector3 targetGrandpaPos = new Vector3(
                grandpa.transform.position.x + offset.x,
                grandpa.transform.position.y + offset.y,
                offset.z
            );
            transform.position = Vector3.Lerp(transform.position, targetGrandpaPos, smoothSpeed * Time.deltaTime);

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
