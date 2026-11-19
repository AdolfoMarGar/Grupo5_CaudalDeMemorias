using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AljibeGameController : MonoBehaviour
{
    [SerializeField] private int targetCleanWater = 15;
    [SerializeField] private int maxContamination = 5;
    [SerializeField] private float spawnInterval = 1.15f;
    [SerializeField] private float minSpawnInterval = 0.65f;
    [SerializeField] [Range(0f, 1f)] private float trashChance = 0.45f;
    [SerializeField] private float spawnY = 4.2f;
    [SerializeField] private float spawnHalfWidth = 1.9f;

    [SerializeField] private GameObject waterParticlePrefab;
    [SerializeField] private GameObject trashParticlePrefab;

    [SerializeField] private TextMeshProUGUI waterText;
    [SerializeField] private TextMeshProUGUI contaminationText;
    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI endTitleText;
    [SerializeField] private TextMeshProUGUI endSubtitleText;
    [SerializeField] private Button retryButton;

    private int cleanWaterCount;
    private int contaminationCount;
    private int trashClearedCount;
    private bool isGameRunning;
    private Coroutine spawnCoroutine;

    public GameObject WaterParticlePrefab
    {
        get => waterParticlePrefab;
        set => waterParticlePrefab = value;
    }

    public GameObject TrashParticlePrefab
    {
        get => trashParticlePrefab;
        set => trashParticlePrefab = value;
    }

    public TextMeshProUGUI WaterText
    {
        get => waterText;
        set => waterText = value;
    }

    public TextMeshProUGUI ContaminationText
    {
        get => contaminationText;
        set => contaminationText = value;
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

    public int CleanWaterCount => cleanWaterCount;
    public int ContaminationCount => contaminationCount;
    public int TrashClearedCount => trashClearedCount;
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

        AljibeParticle[] existing = FindObjectsByType<AljibeParticle>(FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length; i++)
        {
            Destroy(existing[i].gameObject);
        }

        cleanWaterCount = 0;
        contaminationCount = 0;
        trashClearedCount = 0;
        isGameRunning = true;

        if (endPanel != null) endPanel.SetActive(false);

        UpdateHUD();
        SetStatus("Deja pasar el <color=#4FD8FF><b>AGUA</b></color> y expulsa la <color=#9EDB57><b>BASURA</b></color> con las palancas (A / D o Flechas).");

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (isGameRunning)
        {
            SpawnParticle();

            float progress = targetCleanWater > 0 ? Mathf.Clamp01((float)cleanWaterCount / targetCleanWater) : 0f;
            float waitTime = Mathf.Lerp(spawnInterval, minSpawnInterval, progress);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnParticle()
    {
        bool spawnTrash = Random.value < trashChance;
        GameObject prefab = spawnTrash ? trashParticlePrefab : waterParticlePrefab;
        if (prefab == null) return;

        float x = Random.Range(-spawnHalfWidth, spawnHalfWidth);
        Vector3 pos = new Vector3(x, spawnY, 0f);
        GameObject instance = Instantiate(prefab, pos, Quaternion.identity);

        AljibeParticle particle = instance.GetComponent<AljibeParticle>();
        if (particle != null)
        {
            particle.GameController = this;
        }

        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(-1.2f, -0.4f));
        }
    }

    public void OnTrashBroken()
    {
        if (!isGameRunning) return;

        trashClearedCount++;
        SetStatus($"<color=#9EDB57>¡Impureza destruida!</color> (Total limpiado: {trashClearedCount})");
    }

    public void OnParticleEnteredZone(AljibeParticle particle, AljibeZoneType zoneType)
    {
        if (!isGameRunning || particle == null) return;

        if (zoneType == AljibeZoneType.CleanCistern)
        {
            if (particle.ParticleType == AljibeParticleType.Water)
            {
                cleanWaterCount++;
                UpdateHUD();
                SetStatus($"<color=#4FD8FF>+1 Agua limpia</color> ({cleanWaterCount}/{targetCleanWater})");

                if (cleanWaterCount >= targetCleanWater)
                {
                    FinishGame(true);
                }
            }
            else
            {
                contaminationCount++;
                UpdateHUD();
                SetStatus($"<color=#FF5555>¡Basura en el aljibe!</color> Contaminación: {contaminationCount}/{maxContamination}");

                if (contaminationCount >= maxContamination)
                {
                    FinishGame(false);
                }
            }
        }
        else if (zoneType == AljibeZoneType.TrashChute)
        {
            if (particle.ParticleType == AljibeParticleType.Trash)
            {
                trashClearedCount++;
                SetStatus($"<color=#9EDB57>¡Impureza expulsada!</color> (Total limpiado: {trashClearedCount})");
            }
            else
            {
                SetStatus("<color=#FFD166>¡Cuidado! Desperdiciaste una gota de agua.</color>");
            }
        }
    }

    private void FinishGame(bool victory)
    {
        isGameRunning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        if (victory)
        {
            Debug.Log("Victoria en Aljibe 1");
        }

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (endTitleText != null)
        {
            endTitleText.text = victory ? "<color=#4FD8FF>¡ALJIBE LIMPIO!</color>" : "<color=#FF5555>¡AGUA CONTAMINADA!</color>";
        }

        if (endSubtitleText != null)
        {
            endSubtitleText.text = victory
                ? $"Has filtrado {cleanWaterCount} gotas de agua limpia y expulsado {trashClearedCount} impurezas."
                : $"Entraron demasiadas impurezas al aljibe ({contaminationCount}/{maxContamination}).";
        }
    }

    private void UpdateHUD()
    {
        if (waterText != null)
        {
            waterText.text = $"Agua limpia: <color=#4FD8FF><b>{cleanWaterCount} / {targetCleanWater}</b></color>";
        }

        if (contaminationText != null)
        {
            contaminationText.text = $"Contaminación: <color=#FF5555><b>{contaminationCount} / {maxContamination}</b></color>";
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
        {
            statusText.text = msg;
        }
    }
}
