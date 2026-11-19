using UnityEngine;

public enum AljibeParticleType
{
    Water,
    Trash
}

public class AljibeParticle : MonoBehaviour
{
    [SerializeField] private AljibeParticleType particleType = AljibeParticleType.Water;
    [SerializeField] private int maxHitsToBreak = 3;
    [SerializeField] private float maxBounceSpeed = 10.5f;

    private int hitCount;
    private float lastHitTime = -1f;
    private AljibeGameController gameController;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector3 initialScale;

    public AljibeParticleType ParticleType
    {
        get => particleType;
        set => particleType = value;
    }

    public AljibeGameController GameController
    {
        get => gameController;
        set => gameController = value;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        if (pos.y < -8f || Mathf.Abs(pos.x) > 12f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<AljibeFlipper>() == null) return;

        if (rb != null && rb.linearVelocity.magnitude > maxBounceSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxBounceSpeed;
        }

        if (particleType != AljibeParticleType.Trash) return;
        if (Time.time - lastHitTime < 0.08f) return;

        lastHitTime = Time.time;
        hitCount++;

        if (hitCount >= maxHitsToBreak)
        {
            if (gameController != null)
            {
                gameController.OnTrashBroken();
            }
            Destroy(gameObject);
            return;
        }

        float scaleFactor = 1f - (hitCount * 0.14f);
        transform.localScale = initialScale * scaleFactor;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 0.55f, 0.35f, 1f), (float)hitCount / maxHitsToBreak);
        }
    }
}
