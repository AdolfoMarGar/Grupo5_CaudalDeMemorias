using UnityEngine;
using UnityEngine.InputSystem;

public class AljibeFlipper : MonoBehaviour
{
    [SerializeField] private bool isRightFlipper;
    [SerializeField] private float restAngle = -28f;
    [SerializeField] private float activeAngle = 30f;
    [SerializeField] private float flipSpeed = 1150f;
    [SerializeField] private float returnSpeed = 900f;

    private Rigidbody2D rb;
    private bool pointerPressed;
    private bool tapQueued;

    public bool IsRightFlipper
    {
        get => isRightFlipper;
        set => isRightFlipper = value;
    }

    public float RestAngle
    {
        get => restAngle;
        set => restAngle = value;
    }

    public float ActiveAngle
    {
        get => activeAngle;
        set => activeAngle = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.rotation = restAngle;
        }
    }

    public void SetPressed(bool pressed)
    {
        pointerPressed = pressed;
        if (pressed) tapQueued = true;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        bool keyDown = isRightFlipper
            ? (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
            : (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame);

        if (keyDown)
        {
            tapQueued = true;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        bool keyHeld = false;
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            keyHeld = isRightFlipper
                ? (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
                : (kb.aKey.isPressed || kb.leftArrowKey.isPressed);
        }

        bool wantActive = pointerPressed || keyHeld || tapQueued;
        float target = wantActive ? activeAngle : restAngle;
        float speed = wantActive ? flipSpeed : returnSpeed;

        float nextAngle = Mathf.MoveTowardsAngle(rb.rotation, target, speed * Time.fixedDeltaTime);
        rb.MoveRotation(nextAngle);

        if (tapQueued && Mathf.Abs(Mathf.DeltaAngle(nextAngle, activeAngle)) < 1f)
        {
            tapQueued = false;
        }
    }
}
