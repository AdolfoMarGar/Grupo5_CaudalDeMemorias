using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AljibeFishingHook : MonoBehaviour
{
    private enum HookState
    {
        Swinging,
        Extending,
        Retracting
    }

    [SerializeField] private float swingMaxAngle = 64f;
    [SerializeField] private float swingSpeed = 1.9f;
    [SerializeField] private float extendSpeed = 9.5f;
    [SerializeField] private float emptyRetractSpeed = 12f;
    [SerializeField] private float baseRetractSpeed = 7.5f;
    [SerializeField] private float maxLineLength = 9.5f;
    [SerializeField] private float minX = -4.15f;
    [SerializeField] private float maxX = 4.15f;
    [SerializeField] private float minY = -3.65f;

    [SerializeField] private Transform pivotTransform;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Aljibe3GameController gameController;

    private HookState state = HookState.Swinging;
    private float swingTime;
    private float currentAngleDeg;
    private Vector3 extendDirection;
    private AljibeFishingItem caughtItem;
    private float speedMultiplier = 1f;

    public Transform PivotTransform
    {
        get => pivotTransform;
        set => pivotTransform = value;
    }

    public LineRenderer LineRenderer
    {
        get => lineRenderer;
        set => lineRenderer = value;
    }

    public Aljibe3GameController GameController
    {
        get => gameController;
        set => gameController = value;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0.35f, 3f);
    }

    public void ResetHook()
    {
        if (caughtItem != null)
        {
            Destroy(caughtItem.gameObject);
            caughtItem = null;
        }

        state = HookState.Swinging;
        swingTime = 0f;
        speedMultiplier = 1f;
        UpdateHookTransformFromAngle(0f, 0.7f);
        UpdateLine();
    }

    private void Update()
    {
        if (gameController == null || !gameController.IsGameRunning)
        {
            UpdateLine();
            return;
        }

        if (state == HookState.Swinging)
        {
            swingTime += Time.deltaTime * swingSpeed * Mathf.Clamp(speedMultiplier, 0.6f, 1.4f);
            currentAngleDeg = Mathf.Sin(swingTime) * swingMaxAngle;
            UpdateHookTransformFromAngle(currentAngleDeg, 0.7f);

            if (WasLaunchPressed())
            {
                float rad = currentAngleDeg * Mathf.Deg2Rad;
                extendDirection = new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0f).normalized;
                state = HookState.Extending;
            }
        }
        else if (state == HookState.Extending)
        {
            transform.position += extendDirection * (extendSpeed * speedMultiplier * Time.deltaTime);

            Vector3 pivotPos = pivotTransform != null ? pivotTransform.position : Vector3.zero;
            float dist = Vector3.Distance(pivotPos, transform.position);
            Vector3 p = transform.position;
            if (dist >= maxLineLength || p.x <= minX || p.x >= maxX || p.y <= minY)
            {
                p.x = Mathf.Clamp(p.x, minX, maxX);
                p.y = Mathf.Max(p.y, minY);
                transform.position = p;
                state = HookState.Retracting;
            }
        }
        else if (state == HookState.Retracting)
        {
            Vector3 pivotPos = pivotTransform != null ? pivotTransform.position : Vector3.zero;
            Vector3 targetPos = pivotPos + extendDirection * 0.7f;

            float speed = caughtItem != null
                ? baseRetractSpeed * caughtItem.RetractSpeedMultiplier * speedMultiplier
                : emptyRetractSpeed * speedMultiplier;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) <= 0.05f)
            {
                transform.position = targetPos;
                if (caughtItem != null)
                {
                    gameController.CollectItem(caughtItem);
                    Destroy(caughtItem.gameObject);
                    caughtItem = null;
                }
                else
                {
                    gameController.OnEmptyMiss();
                }
                state = HookState.Swinging;
            }
        }

        UpdateLine();
    }

    private bool WasLaunchPressed()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.spaceKey.wasPressedThisFrame)
        {
            return true;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }
            return true;
        }

        return false;
    }

    private void UpdateHookTransformFromAngle(float angleDeg, float distance)
    {
        Vector3 pivotPos = pivotTransform != null ? pivotTransform.position : Vector3.zero;
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0f);
        transform.position = pivotPos + dir * distance;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }

    private void UpdateLine()
    {
        if (lineRenderer == null || pivotTransform == null) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pivotTransform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state != HookState.Extending || caughtItem != null) return;

        AljibeFishingItem item = other.GetComponent<AljibeFishingItem>();
        if (item == null || item.IsCaught) return;

        caughtItem = item;
        caughtItem.MarkCaught(transform);
        state = HookState.Retracting;
    }
}
