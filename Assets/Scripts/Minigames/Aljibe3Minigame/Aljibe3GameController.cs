using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Aljibe3GameController : MonoBehaviour
{
    [SerializeField] private int targetScore = 200;
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private float freezeDuration = 5f;
    [SerializeField] private float speedBoostDuration = 6f;
    [SerializeField] private float speedBoostMultiplier = 1.8f;
    [SerializeField] private float slowDebuffDuration = 5f;
    [SerializeField] private float slowDebuffMultiplier = 0.55f;
    [SerializeField] private float hazardTimePenalty = 4f;
    [SerializeField] private int initialItemCount = 11;
    [SerializeField] private int initialHazardCount = 2;
    [SerializeField] private float bonusSpawnInterval = 8.5f;

    [SerializeField] private AljibeFishingHook fishingHook;
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject bonusFreezePrefab;
    [SerializeField] private GameObject bonusSpeedPrefab;
    [SerializeField] private GameObject hazardSlowPrefab;
    [SerializeField] private GameObject hazardTimePrefab;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI buffsText;
    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI endTitleText;
    [SerializeField] private TextMeshProUGUI endSubtitleText;
    [SerializeField] private Button retryButton;

    private int currentScore;
    private int comboStreak;
    private bool nextCatchDouble;
    private float remainingTime;
    private float freezeTimer;
    private float speedBoostTimer;
    private float slowDebuffTimer;
    private bool isGameRunning;
    private Coroutine bonusCoroutine;

    public AljibeFishingHook FishingHook
    {
        get => fishingHook;
        set => fishingHook = value;
    }

    public GameObject LeafPrefab
    {
        get => leafPrefab;
        set => leafPrefab = value;
    }

    public GameObject TrashPrefab
    {
        get => trashPrefab;
        set => trashPrefab = value;
    }

    public GameObject CoinPrefab
    {
        get => coinPrefab;
        set => coinPrefab = value;
    }

    public GameObject BonusFreezePrefab
    {
        get => bonusFreezePrefab;
        set => bonusFreezePrefab = value;
    }

    public GameObject BonusSpeedPrefab
    {
        get => bonusSpeedPrefab;
        set => bonusSpeedPrefab = value;
    }

    public GameObject HazardSlowPrefab
    {
        get => hazardSlowPrefab;
        set => hazardSlowPrefab = value;
    }

    public GameObject HazardTimePrefab
    {
        get => hazardTimePrefab;
        set => hazardTimePrefab = value;
    }

    public TextMeshProUGUI ScoreText
    {
        get => scoreText;
        set => scoreText = value;
    }

    public TextMeshProUGUI TimerText
    {
        get => timerText;
        set => timerText = value;
    }

    public TextMeshProUGUI BuffsText
    {
        get => buffsText;
        set => buffsText = value;
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

    public int CurrentScore => currentScore;
    public float RemainingTime => remainingTime;
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
        if (bonusCoroutine != null)
        {
            StopCoroutine(bonusCoroutine);
        }

        AljibeFishingItem[] existing = FindObjectsByType<AljibeFishingItem>(FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length; i++)
        {
            Destroy(existing[i].gameObject);
        }

        currentScore = 0;
        comboStreak = 0;
        nextCatchDouble = false;
        remainingTime = timeLimit;
        freezeTimer = 0f;
        speedBoostTimer = 0f;
        slowDebuffTimer = 0f;
        isGameRunning = true;

        if (endPanel != null) endPanel.SetActive(false);
        if (fishingHook != null) fishingHook.ResetHook();

        for (int i = 0; i < initialItemCount; i++)
        {
            SpawnRandomStandardItem();
        }

        for (int i = 0; i < initialHazardCount; i++)
        {
            SpawnRandomHazard();
        }

        UpdateHUD();
        SetStatus("Haz <b>clic</b> o pulsa <b>ESPACIO</b> para lanzar. ¡Rebota en las paredes y evita las trampas!");

        bonusCoroutine = StartCoroutine(BonusSpawnRoutine());
    }

    private void Update()
    {
        if (!isGameRunning) return;

        if (freezeTimer > 0f)
        {
            freezeTimer = Mathf.Max(0f, freezeTimer - Time.deltaTime);
        }
        else
        {
            remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
        }

        if (speedBoostTimer > 0f)
        {
            speedBoostTimer = Mathf.Max(0f, speedBoostTimer - Time.deltaTime);
        }

        if (slowDebuffTimer > 0f)
        {
            slowDebuffTimer = Mathf.Max(0f, slowDebuffTimer - Time.deltaTime);
        }

        ApplyHookSpeedMultiplier();
        UpdateHUD();

        if (remainingTime <= 0f)
        {
            FinishGame(false);
        }
    }

    private void ApplyHookSpeedMultiplier()
    {
        if (fishingHook == null) return;
        float mult = 1f;
        if (speedBoostTimer > 0f) mult *= speedBoostMultiplier;
        if (slowDebuffTimer > 0f) mult *= slowDebuffMultiplier;
        fishingHook.SetSpeedMultiplier(mult);
    }

    private IEnumerator BonusSpawnRoutine()
    {
        while (isGameRunning)
        {
            yield return new WaitForSeconds(bonusSpawnInterval);
            if (!isGameRunning) yield break;

            GameObject bonusPrefab = Random.value < 0.5f ? bonusFreezePrefab : bonusSpeedPrefab;
            if (bonusPrefab != null)
            {
                SpawnAtRandomPosition(bonusPrefab);
            }
        }
    }

    private void SpawnRandomStandardItem()
    {
        float roll = Random.value;
        GameObject prefab;
        if (roll < 0.44f)
        {
            prefab = leafPrefab;
        }
        else if (roll < 0.80f)
        {
            prefab = trashPrefab;
        }
        else
        {
            prefab = coinPrefab;
        }

        if (prefab != null)
        {
            SpawnAtRandomPosition(prefab);
        }
    }

    private void SpawnRandomHazard()
    {
        GameObject prefab = Random.value < 0.5f ? hazardSlowPrefab : hazardTimePrefab;
        if (prefab != null)
        {
            SpawnAtRandomPosition(prefab);
        }
    }

    private void SpawnAtRandomPosition(GameObject prefab)
    {
        float x = Random.Range(-3.75f, 3.75f);
        float y = Random.Range(-3.35f, 1.3f);
        Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
    }

    public void OnEmptyMiss()
    {
        if (!isGameRunning) return;
        if (comboStreak > 0)
        {
            comboStreak = 0;
            UpdateHUD();
            SetStatus("<color=#FFA07A>Fallaste el tiro. ¡Se reinició tu racha de combo!</color>");
        }
    }

    public void CollectItem(AljibeFishingItem item)
    {
        if (!isGameRunning || item == null) return;

        if (item.ItemKind == FishingItemKind.DebuffSlow)
        {
            comboStreak = 0;
            nextCatchDouble = false;
            currentScore = Mathf.Max(0, currentScore - Mathf.Abs(item.Points));
            slowDebuffTimer = slowDebuffDuration;
            speedBoostTimer = 0f;
            ApplyHookSpeedMultiplier();
            SetStatus($"<color=#FF5555><b>¡LODO PESADO!</b></color> -{Mathf.Abs(item.Points)} pts y caña lenta {slowDebuffDuration:0}s.");
            UpdateHUD();
            SpawnRandomHazard();
            return;
        }

        if (item.ItemKind == FishingItemKind.DebuffTimePenalty)
        {
            comboStreak = 0;
            nextCatchDouble = false;
            currentScore = Mathf.Max(0, currentScore - Mathf.Abs(item.Points));
            remainingTime = Mathf.Max(0f, remainingTime - hazardTimePenalty);
            SetStatus($"<color=#FF5555><b>¡TRAMPA OXIDADA!</b></color> -{Mathf.Abs(item.Points)} pts y -{hazardTimePenalty:0}s de tiempo.");
            UpdateHUD();
            SpawnRandomHazard();
            if (remainingTime <= 0f) FinishGame(false);
            return;
        }

        comboStreak++;

        if (item.ItemKind == FishingItemKind.BonusFreezeTime)
        {
            freezeTimer = freezeDuration;
            SetStatus($"<color=#70E4FF><b>¡TIEMPO CONGELADO!</b></color> El reloj se detiene {freezeDuration:0}s.");
            UpdateHUD();
            return;
        }

        if (item.ItemKind == FishingItemKind.BonusSpeed)
        {
            speedBoostTimer = speedBoostDuration;
            slowDebuffTimer = 0f;
            ApplyHookSpeedMultiplier();
            SetStatus($"<color=#FFD166><b>¡CAÑA VELOZ!</b></color> Velocidad aumentada durante {speedBoostDuration:0}s.");
            UpdateHUD();
            return;
        }

        int basePoints = item.Points;
        bool wasCoin = item.ItemKind == FishingItemKind.Coin;
        if (wasCoin)
        {
            basePoints = Random.Range(20, 41);
        }

        int comboBonus = comboStreak > 1 ? Mathf.Min(10, (comboStreak - 1) * 2) : 0;
        int totalPoints = basePoints + comboBonus;

        bool usedDouble = false;
        if (nextCatchDouble)
        {
            totalPoints *= 2;
            nextCatchDouble = false;
            usedDouble = true;
        }

        currentScore += totalPoints;

        if (wasCoin)
        {
            nextCatchDouble = true;
            remainingTime += 2f;
            SetStatus($"<color=#FFD700><b>¡MONEDA DE DESEOS!</b></color> +{totalPoints} pts, <b>+2s</b> y <b>x2</b> en la próxima captura.");
        }
        else
        {
            string doubleTag = usedDouble ? " <color=#FFD700>(x2 DESEO)</color>" : string.Empty;
            string comboTag = comboBonus > 0 ? $" (+{comboBonus} combo)" : string.Empty;
            SetStatus($"Pescaste <b>{GetItemLabel(item.ItemKind)}</b>: <b>+{totalPoints} pts</b>{comboTag}{doubleTag}");
        }

        UpdateHUD();
        SpawnRandomStandardItem();

        if (currentScore >= targetScore)
        {
            FinishGame(true);
        }
    }

    private string GetItemLabel(FishingItemKind kind)
    {
        switch (kind)
        {
            case FishingItemKind.Leaf: return "Hoja";
            case FishingItemKind.Trash: return "Basura";
            case FishingItemKind.Coin: return "Moneda";
            default: return "Objeto";
        }
    }

    private void FinishGame(bool victory)
    {
        isGameRunning = false;
        if (bonusCoroutine != null)
        {
            StopCoroutine(bonusCoroutine);
        }

        if (victory)
        {
            Debug.Log("Victoria en Pesca en el pozo");
        }

        if (endPanel != null) endPanel.SetActive(true);

        if (endTitleText != null)
        {
            endTitleText.text = victory
                ? "<color=#4FD8FF>¡POZO LIMPIO!</color>"
                : "<color=#FF5555>¡TIEMPO AGOTADO!</color>";
        }

        if (endSubtitleText != null)
        {
            endSubtitleText.text = victory
                ? $"Alcanzaste <b>{currentScore} puntos</b> con {remainingTime:0.0}s restantes."
                : $"Conseguiste {currentScore} / {targetScore} puntos antes de acabarse el tiempo.";
        }
    }

    private void UpdateHUD()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: <color=#FFD166><b>{currentScore} / {targetScore}</b></color>";
        }

        if (timerText != null)
        {
            string timeColor = freezeTimer > 0f ? "#70E4FF" : (remainingTime <= 10f ? "#FF5555" : "#FFFFFF");
            string suffix = freezeTimer > 0f ? " (CONGELADO)" : string.Empty;
            timerText.text = $"Tiempo: <color={timeColor}><b>{remainingTime:0.0}s{suffix}</b></color>";
        }

        if (buffsText != null)
        {
            List<string> active = new List<string>();
            if (comboStreak > 1) active.Add($"<color=#9EDB57>Combo x{comboStreak}</color>");
            if (nextCatchDouble) active.Add("<color=#FFD700>Deseo x2 listo</color>");
            if (speedBoostTimer > 0f) active.Add($"<color=#FFA94D>Caña Veloz {speedBoostTimer:0.0}s</color>");
            if (slowDebuffTimer > 0f) active.Add($"<color=#FF5555>Caña Lenta {slowDebuffTimer:0.0}s</color>");
            if (freezeTimer > 0f) active.Add($"<color=#70E4FF>Congelado {freezeTimer:0.0}s</color>");
            buffsText.text = active.Count > 0 ? string.Join("  |  ", active) : string.Empty;
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}
