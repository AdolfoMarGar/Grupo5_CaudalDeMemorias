using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSideController : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float reducedSpeed = 2.5f;
    [SerializeField] private bool isHoldingHands = false;

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
    public float NormalSpeed => normalSpeed;
    public float ReducedSpeed => reducedSpeed;
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

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }
    }

    private void Start()
    {
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

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                input -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                input += 1f;
        }

        if (Gamepad.current != null)
        {
            float stickX = Gamepad.current.leftStick.x.ReadValue();
            if (Mathf.Abs(stickX) > 0.15f)
            {
                input = stickX;
            }
        }

        return Mathf.Clamp(input, -1f, 1f);
    }
}
