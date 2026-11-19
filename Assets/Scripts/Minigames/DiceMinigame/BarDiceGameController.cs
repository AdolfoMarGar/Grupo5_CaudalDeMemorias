using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarDiceGameController : MonoBehaviour
{
    [SerializeField] private int targetStreak = 3;
    [SerializeField] private float baseTimeLimit = 4.5f;
    [SerializeField] private float minTimeLimit = 1.5f;
    [SerializeField] private float timeDecayPerWin = 0.72f;
    [SerializeField] private float revealPauseDuration = 1.1f;

    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI bestStreakText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Image timerFillImage;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private RectTransform machineDiceTransform;
    [SerializeField] private TextMeshProUGUI machineDiceValueText;
    [SerializeField] private RectTransform playerDiceTransform;
    [SerializeField] private TextMeshProUGUI playerDiceValueText;
    [SerializeField] private TextMeshProUGUI vsBadgeText;

    [SerializeField] private Button buttonPares;
    [SerializeField] private Button buttonNones;
    [SerializeField] private Button buttonPlayAgain;

    private int currentStreak;
    private int totalWins;
    private float currentTimeLimit;
    private float remainingTime;
    private bool isWaitingForChoice;
    private bool hasChosen;
    private bool chosenPares;
    private Coroutine activeRoundCoroutine;

    public int TargetStreak
    {
        get => targetStreak;
        set => targetStreak = value;
    }

    public int CurrentStreak => currentStreak;
    public int TotalWins => totalWins;
    public float CurrentTimeLimit => currentTimeLimit;

    public float BaseTimeLimit
    {
        get => baseTimeLimit;
        set => baseTimeLimit = value;
    }

    public float MinTimeLimit
    {
        get => minTimeLimit;
        set => minTimeLimit = value;
    }

    public float TimeDecayPerWin
    {
        get => timeDecayPerWin;
        set => timeDecayPerWin = value;
    }

    public float RevealPauseDuration
    {
        get => revealPauseDuration;
        set => revealPauseDuration = value;
    }

    public TextMeshProUGUI StreakText
    {
        get => streakText;
        set => streakText = value;
    }

    public TextMeshProUGUI BestStreakText
    {
        get => bestStreakText;
        set => bestStreakText = value;
    }

    public TextMeshProUGUI DialogueText
    {
        get => dialogueText;
        set => dialogueText = value;
    }

    public Image TimerFillImage
    {
        get => timerFillImage;
        set => timerFillImage = value;
    }

    public TextMeshProUGUI TimerText
    {
        get => timerText;
        set => timerText = value;
    }

    public RectTransform MachineDiceTransform
    {
        get => machineDiceTransform;
        set => machineDiceTransform = value;
    }

    public TextMeshProUGUI MachineDiceValueText
    {
        get => machineDiceValueText;
        set => machineDiceValueText = value;
    }

    public RectTransform PlayerDiceTransform
    {
        get => playerDiceTransform;
        set => playerDiceTransform = value;
    }

    public TextMeshProUGUI PlayerDiceValueText
    {
        get => playerDiceValueText;
        set => playerDiceValueText = value;
    }

    public TextMeshProUGUI VsBadgeText
    {
        get => vsBadgeText;
        set => vsBadgeText = value;
    }

    public Button ButtonPares
    {
        get => buttonPares;
        set => buttonPares = value;
    }

    public Button ButtonNones
    {
        get => buttonNones;
        set => buttonNones = value;
    }

    public Button ButtonPlayAgain
    {
        get => buttonPlayAgain;
        set => buttonPlayAgain = value;
    }

    private void Start()
    {
        if (buttonPares != null)
        {
            buttonPares.onClick.RemoveAllListeners();
            buttonPares.onClick.AddListener(() => SelectParity(true));
        }

        if (buttonNones != null)
        {
            buttonNones.onClick.RemoveAllListeners();
            buttonNones.onClick.AddListener(() => SelectParity(false));
        }

        if (buttonPlayAgain != null)
        {
            buttonPlayAgain.onClick.RemoveAllListeners();
            buttonPlayAgain.onClick.AddListener(StartNewGame);
        }

        StartNewGame();
    }

    public void StartNewGame()
    {
        if (activeRoundCoroutine != null)
        {
            StopCoroutine(activeRoundCoroutine);
        }

        currentStreak = 0;
        currentTimeLimit = Mathf.Max(minTimeLimit, baseTimeLimit);
        UpdateHeaderUI();
        activeRoundCoroutine = StartCoroutine(RoundRoutine());
    }

    public void SelectParity(bool pares)
    {
        if (!isWaitingForChoice || hasChosen) return;

        chosenPares = pares;
        hasChosen = true;
    }

    private void StartNextRound()
    {
        currentTimeLimit = Mathf.Max(minTimeLimit, baseTimeLimit * Mathf.Pow(timeDecayPerWin, currentStreak));
        activeRoundCoroutine = StartCoroutine(RoundRoutine());
    }

    private IEnumerator RoundRoutine()
    {
        isWaitingForChoice = true;
        hasChosen = false;

        SetChoiceButtonsVisible(true);
        SetChoiceButtonsInteractable(true);
        if (buttonPlayAgain != null) buttonPlayAgain.gameObject.SetActive(false);
        if (vsBadgeText != null) vsBadgeText.text = "+";

        SetDialogue($"<b>Ronda {currentStreak + 1} / {targetStreak}</b> — ¡Los dados ruedan! ¿Será <b>PARES</b> o <b>NONES</b>?");

        remainingTime = currentTimeLimit;
        float changeTimer = 0f;
        float changeInterval = 0.055f;
        Vector3 machineOrigScale = machineDiceTransform != null ? machineDiceTransform.localScale : Vector3.one;
        Vector3 playerOrigScale = playerDiceTransform != null ? playerDiceTransform.localScale : Vector3.one;

        while (remainingTime > 0f && !hasChosen)
        {
            remainingTime -= Time.deltaTime;
            changeTimer += Time.deltaTime;

            UpdateTimerUI(Mathf.Max(0f, remainingTime), currentTimeLimit);

            if (changeTimer >= changeInterval)
            {
                changeTimer = 0f;
                UpdateDiceVisual(machineDiceValueText, Random.Range(1, 7));
                UpdateDiceVisual(playerDiceValueText, Random.Range(1, 7));
            }

            AnimateDice(machineDiceTransform, machineOrigScale, remainingTime, 24f, 18f);
            AnimateDice(playerDiceTransform, playerOrigScale, remainingTime, -26f, 18f);

            yield return null;
        }

        isWaitingForChoice = false;
        SetChoiceButtonsInteractable(false);
        ResetDiceTransform(machineDiceTransform, machineOrigScale);
        ResetDiceTransform(playerDiceTransform, playerOrigScale);

        if (!hasChosen)
        {
            UpdateTimerUI(0f, currentTimeLimit);
            UpdateDiceVisual(machineDiceValueText, 0);
            UpdateDiceVisual(playerDiceValueText, 0);
            HandleLoss("<color=#FF4500>¡TIEMPO AGOTADO!</color> Tardaste demasiado en elegir.\nTu racha y la velocidad se han reiniciado.");
            yield break;
        }

        int machineValue = Random.Range(1, 7);
        int playerValue = Random.Range(1, 7);
        int total = machineValue + playerValue;
        bool isEven = (total % 2) == 0;
        bool won = (chosenPares == isEven);
        string parityLabel = isEven ? "PARES" : "NONES";

        UpdateDiceVisual(machineDiceValueText, machineValue);
        UpdateDiceVisual(playerDiceValueText, playerValue);
        if (vsBadgeText != null) vsBadgeText.text = $"= {total}";

        if (machineDiceTransform != null) machineDiceTransform.localScale = machineOrigScale * 1.18f;
        if (playerDiceTransform != null) playerDiceTransform.localScale = playerOrigScale * 1.18f;

        if (won)
        {
            currentStreak++;

            if (currentStreak >= targetStreak)
            {
                totalWins++;
                UpdateHeaderUI();
                if (machineDiceTransform != null) machineDiceTransform.localScale = machineOrigScale;
                if (playerDiceTransform != null) playerDiceTransform.localScale = playerOrigScale;
                HandleVictory(machineValue, playerValue, total, parityLabel);
                yield break;
            }

            UpdateHeaderUI();

            float nextLimit = Mathf.Max(minTimeLimit, baseTimeLimit * Mathf.Pow(timeDecayPerWin, currentStreak));
            SetDialogue($"<color=#00FF7F>¡ACERTASTE!</color> {machineValue} + {playerValue} = <b>{total} ({parityLabel})</b>.\n¡Racha: <b>{currentStreak} / {targetStreak}</b>! Siguiente ronda ({nextLimit:0.0}s)...");

            float pause = Mathf.Max(0.65f, revealPauseDuration - currentStreak * 0.08f);
            float pauseElapsed = 0f;
            while (pauseElapsed < pause)
            {
                pauseElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(pauseElapsed / 0.2f);
                if (machineDiceTransform != null) machineDiceTransform.localScale = Vector3.Lerp(machineOrigScale * 1.18f, machineOrigScale, t);
                if (playerDiceTransform != null) playerDiceTransform.localScale = Vector3.Lerp(playerOrigScale * 1.18f, playerOrigScale, t);
                yield return null;
            }

            StartNextRound();
        }
        else
        {
            if (machineDiceTransform != null) machineDiceTransform.localScale = machineOrigScale;
            if (playerDiceTransform != null) playerDiceTransform.localScale = playerOrigScale;
            string chosenLabel = chosenPares ? "PARES" : "NONES";
            HandleLoss($"<color=#FF4500>¡FALLASTE!</color> Elegiste <b>{chosenLabel}</b>, pero {machineValue} + {playerValue} = <b>{total} ({parityLabel})</b>.\nTu racha y la velocidad se han reiniciado.");
        }
    }

    private void HandleVictory(int machineValue, int playerValue, int total, string parityLabel)
    {
        Debug.Log("Victoria en los dados");
        SetDialogue($"<color=#FFD700>¡VICTORIA!</color> {machineValue} + {playerValue} = <b>{total} ({parityLabel})</b>.\n¡Has ganado {targetStreak} rondas seguidas!");
        SetChoiceButtonsVisible(false);

        if (buttonPlayAgain != null)
        {
            buttonPlayAgain.gameObject.SetActive(true);
        }
    }

    private void HandleLoss(string message)
    {
        currentStreak = 0;
        currentTimeLimit = Mathf.Max(minTimeLimit, baseTimeLimit);
        UpdateHeaderUI();
        SetDialogue(message);
        SetChoiceButtonsVisible(false);

        if (buttonPlayAgain != null)
        {
            buttonPlayAgain.gameObject.SetActive(true);
        }
    }

    private void AnimateDice(RectTransform diceTransform, Vector3 origScale, float timeValue, float rotSpeed, float maxAngle)
    {
        if (diceTransform == null) return;
        float rotZ = Mathf.Sin(timeValue * rotSpeed) * maxAngle;
        diceTransform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
        float punch = 1f + Mathf.Sin(timeValue * 30f) * 0.08f;
        diceTransform.localScale = new Vector3(origScale.x * punch, origScale.y * punch, origScale.z);
    }

    private void ResetDiceTransform(RectTransform diceTransform, Vector3 origScale)
    {
        if (diceTransform == null) return;
        diceTransform.localRotation = Quaternion.identity;
        diceTransform.localScale = origScale;
    }

    private void UpdateTimerUI(float remaining, float limit)
    {
        float ratio = limit > 0f ? Mathf.Clamp01(remaining / limit) : 0f;

        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = ratio;
            if (ratio > 0.5f)
            {
                timerFillImage.color = Color.Lerp(new Color(0.95f, 0.72f, 0.2f, 1f), new Color(0.25f, 0.85f, 0.45f, 1f), (ratio - 0.5f) * 2f);
            }
            else
            {
                timerFillImage.color = Color.Lerp(new Color(0.92f, 0.22f, 0.2f, 1f), new Color(0.95f, 0.72f, 0.2f, 1f), ratio * 2f);
            }
        }

        if (timerText != null)
        {
            timerText.text = $"{remaining:0.0}s";
        }
    }

    private void UpdateHeaderUI()
    {
        if (streakText != null) streakText.text = $"Racha: <color=#00FF7F><b>{currentStreak} / {targetStreak}</b></color>";
        if (bestStreakText != null) bestStreakText.text = $"Victorias: <color=#FFD700><b>{totalWins}</b></color>";
    }

    private void SetDialogue(string msg)
    {
        if (dialogueText != null) dialogueText.text = msg;
    }

    private void UpdateDiceVisual(TextMeshProUGUI valText, int val)
    {
        if (valText == null) return;
        valText.text = val <= 0 ? "?" : val.ToString();
    }

    private void SetChoiceButtonsInteractable(bool interactable)
    {
        if (buttonPares != null) buttonPares.interactable = interactable;
        if (buttonNones != null) buttonNones.interactable = interactable;
    }

    private void SetChoiceButtonsVisible(bool visible)
    {
        if (buttonPares != null) buttonPares.gameObject.SetActive(visible);
        if (buttonNones != null) buttonNones.gameObject.SetActive(visible);
    }
}
