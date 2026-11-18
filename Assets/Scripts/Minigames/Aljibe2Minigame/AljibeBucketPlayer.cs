using UnityEngine;
using UnityEngine.InputSystem;

public class AljibeBucketPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6.2f;
    [SerializeField] private float acceleration = 26f;
    [SerializeField] private float maxX = 3.4f;

    [SerializeField] private float tiltSensitivity = 12f;
    [SerializeField] private float tiltSpring = 24f;
    [SerializeField] private float tiltDamping = 2.4f;
    [SerializeField] private float maxTiltAngle = 56f;
    [SerializeField] private float spillTiltThreshold = 15f;
    [SerializeField] private float tiltSpillRatePerSec = 20f;
    [SerializeField] private float obstacleHitSpill = 14f;
    [SerializeField] private float sloshBuildRate = 0.75f;
    [SerializeField] private float sloshDecayRate = 0.85f;

    [SerializeField] private Transform waterFillTransform;
    [SerializeField] private GameObject spillDropPrefab;
    [SerializeField] private Aljibe2GameController gameController;

    private float currentVelocityX;
    private float previousVelocityX;
    private float tiltAngle;
    private float tiltVelocity;
    private float sloshEnergy;
    private float hitCooldown;
    private float splashCooldown;
    private int pointerDirection;
    private SpriteRenderer spriteRenderer;

    public Transform WaterFillTransform
    {
        get => waterFillTransform;
        set => waterFillTransform = value;
    }

    public GameObject SpillDropPrefab
    {
        get => spillDropPrefab;
        set => spillDropPrefab = value;
    }

    public Aljibe2GameController GameController
    {
        get => gameController;
        set => gameController = value;
    }

    public float TiltAngle => tiltAngle;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPointerDirection(int dir)
    {
        pointerDirection = dir;
    }

    public void ResetBucket()
    {
        currentVelocityX = 0f;
        previousVelocityX = 0f;
        tiltAngle = 0f;
        tiltVelocity = 0f;
        sloshEnergy = 0f;
        hitCooldown = 0f;
        transform.position = new Vector3(0f, -2.7f, 0f);
        transform.rotation = Quaternion.identity;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        UpdateWaterVisual(100f);
    }

    public void UpdateWaterVisual(float waterPercent)
    {
        if (waterFillTransform == null) return;
        float ratio = Mathf.Clamp01(waterPercent / 100f);
        Vector3 s = waterFillTransform.localScale;
        waterFillTransform.localScale = new Vector3(s.x, Mathf.Max(0.01f, ratio * 0.9f), s.z);
        Vector3 lp = waterFillTransform.localPosition;
        waterFillTransform.localPosition = new Vector3(lp.x, -0.46f + 0.43f * ratio, lp.z);
        waterFillTransform.gameObject.SetActive(ratio > 0.01f);
    }

    private void Update()
    {
        if (gameController == null || !gameController.IsGameRunning) return;

        if (hitCooldown > 0f)
        {
            hitCooldown -= Time.deltaTime;
            if (spriteRenderer != null)
            {
                float blink = Mathf.PingPong(Time.time * 14f, 1f);
                spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 0.45f, 0.45f, 0.8f), blink);
            }
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        float inputDir = pointerDirection;
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) inputDir -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) inputDir += 1f;
        }
        inputDir = Mathf.Clamp(inputDir, -1f, 1f);

        float targetVelX = inputDir * moveSpeed;
        currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelX, acceleration * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + currentVelocityX * Time.deltaTime, -maxX, maxX);
        if (Mathf.Abs(pos.x) >= maxX)
        {
            currentVelocityX = 0f;
        }
        transform.position = pos;

        float accelX = (currentVelocityX - previousVelocityX) / Mathf.Max(0.001f, Time.deltaTime);
        previousVelocityX = currentVelocityX;

        float speedRatio = Mathf.Abs(currentVelocityX) / Mathf.Max(0.1f, moveSpeed);
        if (speedRatio > 0.2f)
        {
            sloshEnergy = Mathf.Min(2f, sloshEnergy + (speedRatio * sloshBuildRate + Mathf.Abs(accelX) * 0.025f) * Time.deltaTime);
        }
        else
        {
            sloshEnergy = Mathf.Max(0f, sloshEnergy - sloshDecayRate * Time.deltaTime);
        }

        tiltVelocity += (-accelX * tiltSensitivity * 0.45f - currentVelocityX * 9.5f) * Time.deltaTime;
        tiltVelocity += (-tiltAngle * tiltSpring - tiltVelocity * tiltDamping) * Time.deltaTime;
        tiltAngle = Mathf.Clamp(tiltAngle + tiltVelocity * Time.deltaTime, -maxTiltAngle, maxTiltAngle);

        transform.rotation = Quaternion.Euler(0f, 0f, tiltAngle);

        float waterRatio = Mathf.Clamp01(gameController.CurrentWaterPercent / 100f);
        float dynamicThreshold = Mathf.Lerp(28f, spillTiltThreshold, waterRatio);

        float absTilt = Mathf.Abs(tiltAngle);
        bool overTilted = absTilt > dynamicThreshold;
        bool overSloshed = sloshEnergy > 1.0f;

        if (overTilted || overSloshed)
        {
            float excessTilt = overTilted ? (absTilt - dynamicThreshold) / Mathf.Max(1f, maxTiltAngle - dynamicThreshold) : 0f;
            float excessSlosh = overSloshed ? (sloshEnergy - 1.0f) : 0f;
            float spillAmount = tiltSpillRatePerSec * (0.35f + excessTilt + excessSlosh * 0.8f) * Time.deltaTime;
            gameController.SpillWater(spillAmount, "¡El agua se agita demasiado!");
            TrySpawnSplash(tiltAngle >= 0f ? -1f : 1f);
        }
    }

    private void TrySpawnSplash(float sideDir)
    {
        if (spillDropPrefab == null || Time.time < splashCooldown) return;
        splashCooldown = Time.time + 0.1f;
        Vector3 spawnPos = transform.position + new Vector3(sideDir * 0.42f, 0.35f, 0f);
        GameObject drop = Instantiate(spillDropPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(sideDir * Random.Range(1.2f, 2.5f), Random.Range(-0.5f, 1.0f));
        }
        Destroy(drop, 1.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameController == null || !gameController.IsGameRunning) return;
        if (hitCooldown > 0f) return;

        AljibeBucketObstacle obstacle = other.GetComponent<AljibeBucketObstacle>();
        if (obstacle == null) return;

        hitCooldown = 0.75f;
        float pushSide = transform.position.x >= obstacle.transform.position.x ? -1f : 1f;
        tiltVelocity += pushSide * 260f;
        gameController.SpillWater(obstacleHitSpill, "¡Golpe contra un obstáculo!");
        TrySpawnSplash(pushSide);
        TrySpawnSplash(-pushSide);
    }
}
