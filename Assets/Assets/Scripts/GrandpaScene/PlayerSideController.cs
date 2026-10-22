using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSideController : MonoBehaviour
{
    [Header("Velocidades")]
    [Tooltip("Velocidad normal cuando camina solo")]
    public float normalSpeed = 5f;

    [Tooltip("Velocidad reducida cuando lleva al abuelo de la mano")]
    public float reducedSpeed = 2.5f;

    [Header("Estado")]
    [SerializeField] private bool isHoldingHands = false;

    // Límites de cámara cuando el abuelo está cansado
    private bool hasCameraBounds = false;
    private float minCameraX;
    private float maxCameraX;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;

    public bool IsHoldingHands
    {
        get => isHoldingHands;
        set => isHoldingHands = value;
    }

    public float CurrentSpeed => isHoldingHands ? reducedSpeed : normalSpeed;
    public float HorizontalInput => horizontalInput;
    public bool FacingRight { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Asegurar orden de renderizado por delante de otros personajes
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }
    }

    private void Start()
    {
        // Ignorar colisiones físicas sólidas con el abuelo para poder pasar por delante de él
        var grandpa = FindAnyObjectByType<GrandpaCompanion>();
        if (grandpa != null)
        {
            IgnoreCollisionWithGrandpa(grandpa.gameObject);
        }
    }

    public void IgnoreCollisionWithGrandpa(GameObject grandpaGo)
    {
        var playerColliders = GetComponentsInChildren<Collider2D>();
        var grandpaColliders = grandpaGo.GetComponentsInChildren<Collider2D>();

        foreach (var pCol in playerColliders)
        {
            foreach (var gCol in grandpaColliders)
            {
                if (!pCol.isTrigger && !gCol.isTrigger)
                {
                    Physics2D.IgnoreCollision(pCol, gCol, true);
                }
            }
        }
    }

    public void SetCameraBounds(bool active, float minX, float maxX)
    {
        hasCameraBounds = active;
        minCameraX = minX;
        maxCameraX = maxX;
    }

    private void Update()
    {
        horizontalInput = ReadHorizontalInput();

        // Orientación del sprite según dirección de movimiento
        if (horizontalInput > 0.05f)
        {
            FacingRight = true;
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
        else if (horizontalInput < -0.05f)
        {
            FacingRight = false;
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }

        // Si los límites de cámara están activos, clampear posición
        if (hasCameraBounds)
        {
            float currentX = transform.position.x;
            float clampedX = Mathf.Clamp(currentX, minCameraX, maxCameraX);
            if (!Mathf.Approximately(currentX, clampedX))
            {
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
            }
        }
    }

    private void FixedUpdate()
    {
        float targetVelocityX = horizontalInput * CurrentSpeed;

        // Impedir que el jugador empuje hacia fuera de los bordes de la cámara cuando está activo el límite
        if (hasCameraBounds)
        {
            if (transform.position.x <= minCameraX && targetVelocityX < 0f)
            {
                targetVelocityX = 0f;
            }
            else if (transform.position.x >= maxCameraX && targetVelocityX > 0f)
            {
                targetVelocityX = 0f;
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        }
        else
        {
            transform.position += new Vector3(targetVelocityX * Time.fixedDeltaTime, 0f, 0f);
        }
    }

    public void SetHoldingHands(bool holding)
    {
        isHoldingHands = holding;
    }

    private float ReadHorizontalInput()
    {
        float input = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                input -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                input += 1f;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Mathf.Approximately(input, 0f))
        {
            try
            {
                input = Input.GetAxisRaw("Horizontal");
            }
            catch
            {
                // Esto es un fallback Fallback silencioso
            }
        }
#endif

        return Mathf.Clamp(input, -1f, 1f);
    }
}
