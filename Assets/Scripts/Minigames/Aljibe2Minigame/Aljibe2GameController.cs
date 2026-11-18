using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Aljibe2GameController : MonoBehaviour
{
    [SerializeField] private float ascentDuration = 22f;
    [SerializeField] private float minRequiredWaterPercent = 40f;
    [SerializeField] private float spawnInterval = 1.05f;
    [SerializeField] private float minSpawnInterval = 0.58f;
    [SerializeField] private float baseObstacleSpeed = 3.6f;
    [SerializeField] private float maxObstacleSpeed = 5.4f;
    [SerializeField] private float spawnHalfWidth = 3.3f;

    [SerializeField] private AljibeBucketPlayer bucketPlayer;
    [SerializeField] private GameObject obstaclePrefab;

    [SerializeField] private TextMeshProUGUI waterPercentText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image progressFillImage;
    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI endTitleText;
    [SerializeField] private TextMeshProUGUI endSubtitleText;
    [SerializeField] private Button retryButton;

    private float currentWaterPercent;
    private float elapsedAscent;
    private bool isGameRunning;
    private Coroutine spawnCoroutine;

    public AljibeBucketPlayer BucketPlayer
    {
        get => bucketPlayer;
        set => bucketPlayer = value;
    }

    public GameObject ObstaclePrefab
    {
        get => obstaclePrefab;
        set => obstaclePrefab = value;
    }

    public TextMeshProUGUI WaterPercentText
    {
        get => waterPercentText;
        set => waterPercentText = value;
    }

    public TextMeshProUGUI ProgressText
    {
        get => progressText;
        set => progressText = value;
    }

    public Image ProgressFillImage
    {
        get => progressFillImage;
        set => progressFillImage = value;
    }

    public TextMeshProUGUI StatusText
    {
        get => statusText;
        set => statusText = value;
    }

    public GameObject EndPanel
    {
        get => endPanel;
        set => endPanel = value;
    }

    public TextMeshProUGUI EndTitleText
    {
        get => endTitleText;
        set => endTitleText = value;
    }

    public TextMeshProUGUI EndSubtitleText
    {
        get => endSubtitleText;
        set => endSubtitleText = value;
    }

    public Button RetryButton
    {
        get => retryButton;
        set => retryButton = value;
    }

    public float CurrentWaterPercent => currentWaterPercent;
    public bool IsGameRunning => isGameRunning;

    private void Start()
    {
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(StartNewGame);
        }

        StartNewGame();
    }

    public void StartNewGame()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        AljibeBucketObstacle[] existing = FindObjectsByType<AljibeBucketObstacle>(FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length; i++)
        {
            Destroy(existing[i].gameObject);
        }

        currentWaterPercent = 100f;
        elapsedAscent = 0f;
        isGameRunning = true;

        if (endPanel != null) endPanel.SetActive(false);
        if (bucketPlayer != null) bucketPlayer.ResetBucket();

        UpdateHUD();
        SetStatus("Esquiva obstáculos con <color=#FFD166><b>A / D</b></color> o las flechas sin moverte demasiado brusco.");

        spawnCoroutine = StartCoroutine(SpawnObstaclesRoutine());
    }

    private void Update()
    {
        if (!isGameRunning) return;

        elapsedAscent += Time.deltaTime;
        UpdateHUD();

        if (elapsedAscent >= ascentDuration)
        {
            bool won = currentWaterPercent >= minRequiredWaterPercent;
            FinishGame(won, won ? string.Empty : $"Solo llegaste con un {Mathf.RoundToInt(currentWaterPercent)}% de agua (mínimo {Mathf.RoundToInt(minRequiredWaterPercent)}%).");
        }
    }

    private IEnumerator SpawnObstaclesRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        while (isGameRunning)
        {
            float firstX = Random.Range(-spawnHalfWidth, spawnHalfWidth);
            SpawnObstacleAt(firstX, 6.2f);

            float progress = Mathf.Clamp01(elapsedAscent / Mathf.Max(1f, ascentDuration));
            if (progress > 0.15f && Random.value < 0.28f)
            {
                float secondX = firstX > 0f
                    ? Random.Range(-spawnHalfWidth, Mathf.Max(-spawnHalfWidth, firstX - 2.3f))
                    : Random.Range(Mathf.Min(spawnHalfWidth, firstX + 2.3f), spawnHalfWidth);

                if (Mathf.Abs(secondX - firstX) >= 2.1f)
                {
                    SpawnObstacleAt(secondX, 6.65f);
                }
            }

            float baseWait = Mathf.Lerp(spawnInterval, minSpawnInterval, progress);
            float randomizedWait = baseWait * Random.Range(0.68f, 1.32f);
            yield return new WaitForSeconds(randomizedWait);
        }
    }

    private void SpawnObstacleAt(float x, float y)
    {
        if (obstaclePrefab == null) return;

        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(-15f, 15f));
        GameObject instance = Instantiate(obstaclePrefab, new Vector3(x, y, 0f), rot);

        float progress = Mathf.Clamp01(elapsedAscent / Mathf.Max(1f, ascentDuration));
        float baseSpeed = Mathf.Lerp(baseObstacleSpeed, maxObstacleSpeed, progress);
        float speed = baseSpeed * Random.Range(0.75f, 1.35f);
        float scaleX = Random.Range(0.72f, 1.45f);
        float scaleY = scaleX * Random.Range(0.68f, 0.92f);
        instance.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        AljibeBucketObstacle obs = instance.GetComponent<AljibeBucketObstacle>();
        if (obs != null)
        {
            obs.FallSpeed = speed;
        }
    }

    public void SpillWater(float amount, string reason)
    {
        if (!isGameRunning) return;

        currentWaterPercent = Mathf.Max(0f, currentWaterPercent - amount);
        if (bucketPlayer != null)
        {
            bucketPlayer.UpdateWaterVisual(currentWaterPercent);
        }

        UpdateHUD();
        if (!string.IsNullOrEmpty(reason))
        {
            SetStatus($"<color=#FF6B6B>{reason}</color> Agua restante: {Mathf.RoundToInt(currentWaterPercent)}%");
        }

        if (currentWaterPercent < minRequiredWaterPercent)
        {
            FinishGame(false, $"¡Has bajado del {Mathf.RoundToInt(minRequiredWaterPercent)}% de agua necesaria!");
        }
    }

    private void FinishGame(bool victory, string customReason)
    {
        isGameRunning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        if (victory)
        {
            Debug.Log("Victoria en Aljibe 2");
        }

        if (endPanel != null) endPanel.SetActive(true);

        if (endTitleText != null)
        {
            endTitleText.text = victory
                ? "<color=#4FD8FF>¡SUBIDA COMPLETADA!</color>"
                : "<color=#FF5555>¡CUBO SIN AGUA SUFICIENTE!</color>";
        }

        if (endSubtitleText != null)
        {
            endSubtitleText.text = victory
                ? $"Has llegado a la superficie conservando un <b>{Mathf.RoundToInt(currentWaterPercent)}%</b> de agua."
                : customReason;
        }
    }

    private void UpdateHUD()
    {
        int pct = Mathf.RoundToInt(currentWaterPercent);
        string waterColor = pct >= minRequiredWaterPercent ? "#4FD8FF" : "#FF6B6B";
        if (waterPercentText != null)
        {
            waterPercentText.text = $"Agua: <color={waterColor}><b>{pct}%</b></color> (Mín. {Mathf.RoundToInt(minRequiredWaterPercent)}%)";
        }

        float ascentRatio = Mathf.Clamp01(elapsedAscent / Mathf.Max(1f, ascentDuration));
        if (progressText != null)
        {
            progressText.text = $"Ascenso: <color=#FFD166><b>{Mathf.RoundToInt(ascentRatio * 100f)}%</b></color>";
        }

        if (progressFillImage != null)
        {
            progressFillImage.fillAmount = ascentRatio;
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}
