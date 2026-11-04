using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(BoxCollider2D))]
public class GrandpaCompanion : MonoBehaviour
{
    public PlayerSideController player;
    public GrandpaCameraFollow cameraFollow;

    public float followDistance = 1.3f;
    public float followSpeed = 3.8f;

    public float minTiredTime = 16f;
    public float maxTiredTime = 26f;

    public float stillTimeToWander = 2.5f;
    public float wanderSpeed = 1.4f;

    public float breathSpeed = 8f;
    public float breathIntensity = 0.08f;

    [SerializeField] private bool holdingHands = false;
    [SerializeField] private bool isTired = false;
    [SerializeField] private bool isWandering = false;
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] private float currentTiredTimer = 0f;
    [SerializeField] private float currentStillTimer = 0f;

    private bool targetSideIsLeft = true;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;

    private float wanderTimer = 0f;
    private bool wanderIsWalking = false;
    private float wanderDirection = 0f;

    public bool IsHoldingHands => holdingHands;
    public bool IsTired => isTired;
    public bool IsWandering => isWandering;
    public bool IsPlayerInRange => isPlayerInRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 2;
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerSideController>();
        }
        if (cameraFollow == null)
        {
            cameraFollow = FindAnyObjectByType<GrandpaCameraFollow>();
        }

        IgnorePlayerCollision();
    }

    public void IgnorePlayerCollision()
    {
        if (player == null) return;

        var playerColliders = player.GetComponentsInChildren<Collider2D>();
        var myColliders = GetComponentsInChildren<Collider2D>();

        foreach (var pCol in playerColliders)
        {
            foreach (var mCol in myColliders)
            {
                if (!pCol.isTrigger && !mCol.isTrigger)
                {
                    Physics2D.IgnoreCollision(pCol, mCol, true);
                }
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && CheckInteractInput())
        {
            HandleInteraction();
        }

        if (holdingHands && !isTired && !isWandering)
        {
            bool isMoving = player != null && Mathf.Abs(player.HorizontalInput) > 0.05f;
            if (isMoving)
            {
                currentStillTimer = stillTimeToWander;
                currentTiredTimer -= Time.deltaTime;
                if (currentTiredTimer <= 0f)
                {
                    GetTired();
                }
            }
            else
            {
                currentStillTimer -= Time.deltaTime;
                if (currentStillTimer <= 0f)
                {
                    StartWandering();
                }
            }
        }
        else if (isWandering && !isTired)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0f)
            {
                wanderIsWalking = !wanderIsWalking;
                if (wanderIsWalking)
                {
                    wanderDirection = Random.value < 0.5f ? -1f : 1f;
                    wanderTimer = Random.Range(1.2f, 1.8f);
                }
                else
                {
                    wanderDirection = 0f;
                    wanderTimer = Random.Range(1.8f, 2.5f);
                }
            }
        }

        if (isTired)
        {
            float breath = Mathf.Sin(Time.time * breathSpeed) * breathIntensity;
            transform.localScale = new Vector3(initialScale.x, initialScale.y * (1f + breath), initialScale.z);
        }
    }

    private void FixedUpdate()
    {
        if (holdingHands && !isTired && !isWandering && player != null)
        {
            if (player.HorizontalInput > 0.05f)
            {
                targetSideIsLeft = true;
            }
            else if (player.HorizontalInput < -0.05f)
            {
                targetSideIsLeft = false;
            }

            float targetX = targetSideIsLeft
                ? player.transform.position.x - followDistance
                : player.transform.position.x + followDistance;

            float diff = targetX - transform.position.x;
            float distance = Mathf.Abs(diff);

            if (distance > 0.05f)
            {
                float speedMultiplier = distance > followDistance ? 1.6f : 1.0f;
                float currentFollowSpeed = followSpeed * speedMultiplier;
                float step = Mathf.Sign(diff) * Mathf.Min(distance * 6f, currentFollowSpeed);

                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(step, rb.linearVelocity.y);
                }
                else
                {
                    transform.position = new Vector3(
                        Mathf.MoveTowards(transform.position.x, targetX, currentFollowSpeed * Time.fixedDeltaTime),
                        transform.position.y,
                        transform.position.z
                    );
                }

                if (spriteRenderer != null && Mathf.Abs(step) > 0.05f)
                {
                    spriteRenderer.flipX = step < 0;
                }
            }
            else
            {
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                }
            }
        }
        else if (isWandering && !isTired)
        {
            if (wanderIsWalking && Mathf.Abs(wanderDirection) > 0.05f)
            {
                float step = wanderDirection * wanderSpeed;
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(step, rb.linearVelocity.y);
                }
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = step < 0;
                }
            }
            else
            {
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                }
            }
        }
        else
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    private void HandleInteraction()
    {
        if (isTired)
        {
            isTired = false;
            isWandering = false;
            holdingHands = true;
            transform.localScale = initialScale;
            currentTiredTimer = Random.Range(minTiredTime, maxTiredTime);
            currentStillTimer = stillTimeToWander;

            if (player != null)
            {
                targetSideIsLeft = transform.position.x < player.transform.position.x;
                player.SetHoldingHands(true);
            }

            if (cameraFollow != null)
            {
                cameraFollow.OnHandsResumed();
            }
        }
        else if (isWandering)
        {
            isWandering = false;
            holdingHands = true;
            currentTiredTimer = Random.Range(minTiredTime, maxTiredTime);
            currentStillTimer = stillTimeToWander;

            if (player != null)
            {
                targetSideIsLeft = transform.position.x < player.transform.position.x;
                player.SetHoldingHands(true);
            }
        }
        else if (!holdingHands)
        {
            holdingHands = true;
            currentTiredTimer = Random.Range(minTiredTime, maxTiredTime);
            currentStillTimer = stillTimeToWander;

            if (player != null)
            {
                targetSideIsLeft = transform.position.x < player.transform.position.x;
                player.SetHoldingHands(true);
            }
        }
    }

    private void StartWandering()
    {
        holdingHands = false;
        isWandering = true;

        if (player != null)
        {
            player.SetHoldingHands(false);
        }

        wanderIsWalking = true;
        wanderDirection = Random.value < 0.5f ? -1f : 1f;
        wanderTimer = Random.Range(1.2f, 1.8f);
    }

    private void GetTired()
    {
        isTired = true;
        holdingHands = false;
        isWandering = false;

        if (player != null)
        {
            player.SetHoldingHands(false);
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (cameraFollow != null)
        {
            cameraFollow.OnGrandpaTired();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayerCollider(collision))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayerCollider(collision))
        {
            isPlayerInRange = false;
        }
    }

    private bool IsPlayerCollider(Collider2D col)
    {
        if (player != null && (col.gameObject == player.gameObject || col.transform.IsChildOf(player.transform)))
            return true;
        return col.GetComponent<PlayerSideController>() != null || col.CompareTag("Player");
    }

    private bool CheckInteractInput()
    {
        bool pressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            pressed = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (!pressed)
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.E))
                    pressed = true;
            }
            catch
            {
            }
        }
#endif

        return pressed;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleLeft;

        string statusText = isTired
            ? "<color=#FFA500><b>Abuelo: Cansado</b></color> (Cámara fija. Vuelve a su lado y pulsa E)"
            : (isWandering
                ? "<color=#FF69B4><b>Abuelo: Desorientado</b></color> (Te detuviste y se soltó a caminar solo)"
                : (holdingHands
                    ? $"<color=#00FF7F><b>De la mano</b></color> (No te detengas - Cansancio en {currentTiredTimer:F1}s)"
                    : "<color=#D3D3D3>Abuelo: Esperando</color>"));

        if (isPlayerInRange)
        {
            string actionText = isTired
                ? "Pulsa [E] para descansar y volver a darle la mano"
                : (isWandering || !holdingHands
                    ? "Pulsa [E] para darle la mano"
                    : "");

            if (!string.IsNullOrEmpty(actionText))
            {
                statusText += $"\n<color=#FFFF00><b>{actionText}</b></color>";
            }
        }

        int height = isPlayerInRange && (!holdingHands || isTired || isWandering) ? 55 : 35;
        GUI.Box(new Rect(15, 15, 520, height), statusText, style);
    }
}
