using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(BoxCollider2D))]
public class GrandpaCompanion : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Referencia al jugador")]
    public PlayerSideController player;

    [Tooltip("Referencia a la cámara para avisarle de dejar de seguir")]
    public GrandpaCameraFollow cameraFollow;

    [Header("Acompañamiento")]
    [Tooltip("Distancia que mantiene detrás del jugador")]
    public float followDistance = 1.3f;

    [Tooltip("Velocidad base con la que sigue al jugador")]
    public float followSpeed = 3.8f;

    [Header("Tiempos de cansancio")]
    [Tooltip("Tiempo mínimo antes de cansarse")]
    public float minTiredTime = 4f;

    [Tooltip("Tiempo máximo antes de cansarse")]
    public float maxTiredTime = 8f;

    [Header("Efecto de respiración cansada")]
    [Tooltip("Velocidad de oscilación al jadear")]
    public float breathSpeed = 8f;

    [Tooltip("Amplitud de cambio de escala en Y")]
    public float breathIntensity = 0.08f;

    [Header("Estado Actual (Inspector)")]
    [SerializeField] private bool holdingHands = false;
    [SerializeField] private bool isTired = false;
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] private float currentTimer = 0f;

    // true = abuelo debe colocarse a la izquierda del jugador, false = a la derecha
    private bool targetSideIsLeft = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;

    public bool IsHoldingHands => holdingHands;
    public bool IsTired => isTired;
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

        // El abuelo se renderiza detrás del jugador (sortingOrder 2 frente a 5 del player)
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

        // Ignorar colisiones físicas sólidas con el jugador para poder pasar por delante sin empujar
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
        // Interacción con tecla E cuando el jugador está dentro del Trigger
        if (isPlayerInRange && CheckInteractInput())
        {
            HandleInteraction();
        }

        // Lógica de cansancio mientras van de la mano
        if (holdingHands)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0f)
            {
                GetTired();
            }
        }

        // Efecto visual de jadeo/respiración agitada cuando está cansado
        if (isTired)
        {
            float breath = Mathf.Sin(Time.time * breathSpeed) * breathIntensity;
            transform.localScale = new Vector3(initialScale.x, initialScale.y * (1f + breath), initialScale.z);
        }
    }

    private void FixedUpdate()
    {
        if (holdingHands && !isTired && player != null)
        {
            // Determinar de qué lado debe ir el abuelo para estar SIEMPRE detrás del movimiento del jugador
            if (player.HorizontalInput > 0.05f)
            {
                // Jugador camina a la derecha -> el abuelo debe ir a la izquierda (detrás de él)
                targetSideIsLeft = true;
            }
            else if (player.HorizontalInput < -0.05f)
            {
                // Jugador camina a la izquierda -> el abuelo debe ir a la derecha (detrás de él)
                targetSideIsLeft = false;
            }
            // Si el jugador está quieto, se mantiene el lado en el que ya estaba colocado

            float targetX = targetSideIsLeft
                ? player.transform.position.x - followDistance
                : player.transform.position.x + followDistance;

            float diff = targetX - transform.position.x;
            float distance = Mathf.Abs(diff);

            if (distance > 0.05f)
            {
                // Si está cruzando al otro lado del jugador o está lejos, aumenta la velocidad para reposicionarse ágilmente
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

                // Orientar sprite según dirección en la que camina el abuelo
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
        else
        {
            // Quieto si no van de la mano o está cansado
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
            // El jugador ha vuelto a buscar al abuelo, reinicia el ciclo
            isTired = false;
            holdingHands = true;
            transform.localScale = initialScale; // Restaurar escala
            currentTimer = Random.Range(minTiredTime, maxTiredTime);

            // Determinar lado inicial según la posición actual respecto al jugador
            if (player != null)
            {
                targetSideIsLeft = transform.position.x < player.transform.position.x;
                player.SetHoldingHands(true);
            }

            if (cameraFollow != null)
            {
                cameraFollow.OnHandsResumed();
            }

            Debug.Log("[GrandpaCompanion] ¡El abuelo descansó y vuelve a tomar la mano del jugador!");
        }
        else
        {
            // Alternar de la mano / soltar manualmente
            holdingHands = !holdingHands;

            if (player != null)
            {
                player.SetHoldingHands(holdingHands);
            }

            if (holdingHands)
            {
                // Al tomar la mano, mantén inicialmente el lado relativo en el que ya está
                if (player != null)
                {
                    targetSideIsLeft = transform.position.x < player.transform.position.x;
                }

                currentTimer = Random.Range(minTiredTime, maxTiredTime);
                Debug.Log($"[GrandpaCompanion] Van de la mano. Tiempo hasta cansancio: {currentTimer:F1}s");
            }
            else
            {
                Debug.Log("[GrandpaCompanion] Mano soltada manualmente.");
            }
        }
    }

    private void GetTired()
    {
        isTired = true;
        holdingHands = false;

        // Jugador recupera velocidad normal
        if (player != null)
        {
            player.SetHoldingHands(false);
        }

        // El abuelo frena en el sitio
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // Avisa a la cámara para que deje de seguir al jugador y pase a enfocar al abuelo
        if (cameraFollow != null)
        {
            cameraFollow.OnGrandpaTired();
        }

        Debug.Log("[GrandpaCompanion] ¡El abuelo se ha cansado! Suelta la mano y se detiene.");
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
                // Fallback silencioso
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
            ? "<color=#FFA500><b>Abuelo: Cansado</b></color> (La cámara lo enfoca solo a él. Vuelve a su lado y pulsa E)"
            : (holdingHands
                ? $"<color=#00FF7F><b>De la mano</b></color> (Cansancio en {currentTimer:F1}s)"
                : "<color=#D3D3D3>Abuelo: Esperando</color>");

        if (isPlayerInRange)
        {
            string actionText = isTired
                ? "Pulsa [E] para descansar y volver a darle la mano"
                : (holdingHands ? "Pulsa [E] para soltar su mano" : "Pulsa [E] para darle la mano");
            statusText += $"\n<color=#FFFF00><b>{actionText}</b></color>";
        }

        int height = isPlayerInRange ? 55 : 35;
        GUI.Box(new Rect(15, 15, 520, height), statusText, style);
    }
}
