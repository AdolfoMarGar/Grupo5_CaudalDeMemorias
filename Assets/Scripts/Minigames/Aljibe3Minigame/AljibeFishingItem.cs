using UnityEngine;

public enum FishingItemKind
{
    Leaf,
    Trash,
    Coin,
    BonusFreezeTime,
    BonusSpeed,
    DebuffSlow,
    DebuffTimePenalty
}

public class AljibeFishingItem : MonoBehaviour
{
    [SerializeField] private FishingItemKind itemKind = FishingItemKind.Leaf;
    [SerializeField] private int points = 5;
    [SerializeField] private float retractSpeedMultiplier = 1f;
    [SerializeField] private float bobAmplitude = 0.09f;
    [SerializeField] private float bobSpeed = 1.9f;
    [SerializeField] private float driftSpeed = 0.45f;
    [SerializeField] private float driftLimitX = 3.85f;
    [SerializeField] private float fadeInDuration = 0.45f;

    private bool isCaught;
    private float baseY;
    private float phaseOffset;
    private float currentDriftSpeed;
    private float fadeElapsed;
    private SpriteRenderer spriteRenderer;

    public FishingItemKind ItemKind
    {
        get => itemKind;
        set => itemKind = value;
    }

    public int Points
    {
        get => points;
        set => points = value;
    }

    public float RetractSpeedMultiplier
    {
        get => retractSpeedMultiplier;
        set => retractSpeedMultiplier = value;
    }

    public bool IsCaught => isCaught;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
        }
    }

    private void Start()
    {
        baseY = transform.position.y;
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        float dir = Random.value < 0.5f ? -1f : 1f;
        currentDriftSpeed = dir * driftSpeed * Random.Range(0.4f, 1.25f);
    }

    private void Update()
    {
        if (spriteRenderer != null && fadeElapsed < fadeInDuration)
        {
            fadeElapsed += Time.deltaTime;
            Color c = spriteRenderer.color;
            c.a = Mathf.Clamp01(fadeElapsed / Mathf.Max(0.01f, fadeInDuration));
            spriteRenderer.color = c;
        }

        if (isCaught) return;

        Vector3 pos = transform.position;
        pos.x += currentDriftSpeed * Time.deltaTime;
        if (pos.x > driftLimitX)
        {
            pos.x = driftLimitX;
            currentDriftSpeed = -Mathf.Abs(currentDriftSpeed);
        }
        else if (pos.x < -driftLimitX)
        {
            pos.x = -driftLimitX;
            currentDriftSpeed = Mathf.Abs(currentDriftSpeed);
        }

        pos.y = baseY + Mathf.Sin(Time.time * bobSpeed + phaseOffset) * bobAmplitude;
        transform.position = pos;
    }

    public void MarkCaught(Transform hookTransform)
    {
        isCaught = true;
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        transform.SetParent(hookTransform);
        transform.localPosition = new Vector3(0f, -0.35f, 0f);
    }
}
