using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Gira automáticamente el sprite en X según la dirección horizontal del movimiento")]
    [SerializeField] private bool autoFlipX = true;
    [SerializeField] public PlayerDepthScaling depthScaling;
    public bool canMove = true;


    private Rigidbody2D rb;
    private Vector2 movementInput;

    public float Speed => speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 0f;
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (depthScaling == null) depthScaling = GetComponent<PlayerDepthScaling>();
    }

    private void Update()
    {
        bool isMoving;

        if (!canMove)
        {
            movementInput = Vector2.zero;
            isMoving = false;
        }
        else
        {
            movementInput = ReadMovementInput();
            isMoving = movementInput.sqrMagnitude > 0.0025f;

        }
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        if (autoFlipX && spriteRenderer != null)
        {
            if (movementInput.x < -0.05f) spriteRenderer.flipX = true;
            else if (movementInput.x > 0.05f) spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = movementInput * speed;
        }
        else
        {
            transform.position += new Vector3(movementInput.x * speed * Time.fixedDeltaTime, movementInput.y * speed * Time.fixedDeltaTime, 0f);
        }
    }

    private Vector2 ReadMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
        }

        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick.sqrMagnitude > 0.04f)
            {
                input = stick;
            }
        }

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        return input;
    }

    public void SetLimitsPosition(Transform top, Transform bottom)
    {
        if (depthScaling == null) depthScaling = GetComponent<PlayerDepthScaling>();
        if (depthScaling != null)
        {
            depthScaling.SetLimitsPosition(top, bottom);
        }
    }

    public void SetLimits(float topY, float bottomY)
    {
        if (depthScaling == null) depthScaling = GetComponent<PlayerDepthScaling>();
        if (depthScaling != null)
        {
            depthScaling.SetLimits(topY, bottomY);
        }
    }
}
