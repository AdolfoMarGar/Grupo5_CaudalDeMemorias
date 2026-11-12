using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarDiceGameController : MonoBehaviour
{
    [SerializeField] private int playerCoins = 100;
    [SerializeField] private int initialBet = 10;
    [SerializeField] private int currentPot = 0;
    [SerializeField] private int machineDiceValue = 1;
    [SerializeField] private int playerDiceValue = 1;

    [SerializeField] private float playerRollDuration = 2.0f;
    [SerializeField] private float initialRollDuration = 2.0f;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI potText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private RectTransform machineDiceTransform;
    [SerializeField] private TextMeshProUGUI machineDiceValueText;
    [SerializeField] private Image machineDiceImage;

    [SerializeField] private RectTransform playerDiceTransform;
    [SerializeField] private TextMeshProUGUI playerDiceValueText;
    [SerializeField] private Image playerDiceImage;

    [SerializeField] private Button buttonHigher;
    [SerializeField] private Button buttonLower;
    [SerializeField] private Button buttonCashOut;
    [SerializeField] private Button buttonNewRoll;
    [SerializeField] private Button buttonExit;

    private bool isRoundActive = false;
    private bool isAnimating = false;
    private bool exitRequested = false;

    public int PlayerCoins
    {
        get => playerCoins;
        set => playerCoins = value;
    }

    public int CurrentPot => currentPot;
    public bool ExitRequested => exitRequested;
    public float PlayerRollDuration
    {
        get => playerRollDuration;
        set => playerRollDuration = value;
    }

    public float InitialRollDuration
    {
        get => initialRollDuration;
        set => initialRollDuration = value;
    }

    public Button ButtonExit
    {
        get => buttonExit;
        set => buttonExit = value;
    }

    private void Start()
    {
        if (buttonHigher != null)
        {
            buttonHigher.onClick.RemoveAllListeners();
            buttonHigher.onClick.AddListener(() => OnPlayerGuess(true));
        }

        if (buttonLower != null)
        {
            buttonLower.onClick.RemoveAllListeners();
            buttonLower.onClick.AddListener(() => OnPlayerGuess(false));
        }

        if (buttonCashOut != null)
        {
            buttonCashOut.onClick.RemoveAllListeners();
            buttonCashOut.onClick.AddListener(OnCashOut);
        }

        if (buttonNewRoll != null)
        {
            buttonNewRoll.onClick.RemoveAllListeners();
            buttonNewRoll.onClick.AddListener(StartNewRound);
        }

        if (buttonExit != null)
        {
            buttonExit.onClick.RemoveAllListeners();
            buttonExit.onClick.AddListener(RequestExit);
        }

        StartNewRound();
    }

    public void RequestExit()
    {
        exitRequested = true;
        isRoundActive = false;
        isAnimating = false;
        StopAllCoroutines();
    }

    public void StartNewRound()
    {
        if (isAnimating) return;

        exitRequested = false;

        if (playerCoins < initialBet)
        {
            playerCoins = 50;
            SetDialogue("¡Te quedaste sin blanca! Toma 50 monedas cortesía de la casa.");
        }

        playerCoins -= initialBet;
        currentPot = initialBet;
        isRoundActive = true;

        UpdateHeaderUI();
        StartCoroutine(InitialRollRoutine());
    }

    private IEnumerator InitialRollRoutine()
    {
        isAnimating = true;
        UpdateButtonStates(false);

        SetDialogue("El tabernero lanza su dado sobre la mesa...");

        float duration = Mathf.Max(0.5f, initialRollDuration);
        float elapsed = 0f;
        float changeTimer = 0f;
        float changeInterval = 0.06f;
        Vector3 origScale = machineDiceTransform != null ? machineDiceTransform.localScale : Vector3.one;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            changeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            if (changeTimer >= changeInterval)
            {
                changeTimer = 0f;
                changeInterval = Mathf.Lerp(0.04f, 0.12f, progress);
                int tempRoll = Random.Range(1, 7);
                UpdateDiceVisual(machineDiceValueText, tempRoll);
            }

            if (machineDiceTransform != null)
            {
                float maxAngle = Mathf.Lerp(22f, 4f, progress);
                float rotZ = Mathf.Sin(elapsed * 24f) * maxAngle;
                machineDiceTransform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
                float punch = 1f + Mathf.Sin(elapsed * 30f) * 0.08f * (1f - progress * 0.7f);
                machineDiceTransform.localScale = new Vector3(origScale.x * punch, origScale.y * punch, origScale.z);
            }

            yield return null;
        }

        if (machineDiceTransform != null)
        {
            machineDiceTransform.localRotation = Quaternion.identity;
            machineDiceTransform.localScale = origScale;
        }

        machineDiceValue = Random.Range(1, 7);
        UpdateDiceVisual(machineDiceValueText, machineDiceValue);
        UpdateDiceVisual(playerDiceValueText, 0);

        SetDialogue($"El tabernero sacó un <b>{machineDiceValue}</b>.\n¿Tu tirada será <b>MAYOR</b> o <b>MENOR</b> que la suya?");

        isAnimating = false;
        UpdateButtonStates(true);
    }

    public void OnPlayerGuess(bool guessHigher)
    {
        if (!isRoundActive || isAnimating) return;

        StartCoroutine(RollPlayerDiceRoutine(guessHigher));
    }

    private IEnumerator RollPlayerDiceRoutine(bool guessHigher)
    {
        isAnimating = true;
        UpdateButtonStates(false);

        SetDialogue("¡Lanzas tus dados con fuerza sobre el tapete!");

        float duration = Mathf.Max(0.5f, playerRollDuration);
        float elapsed = 0f;
        float changeTimer = 0f;
        float changeInterval = 0.06f;
        Vector3 origScale = playerDiceTransform != null ? playerDiceTransform.localScale : Vector3.one;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            changeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            if (changeTimer >= changeInterval)
            {
                changeTimer = 0f;
                changeInterval = Mathf.Lerp(0.04f, 0.14f, progress);
                int tempRoll = Random.Range(1, 7);
                UpdateDiceVisual(playerDiceValueText, tempRoll);
            }

            if (playerDiceTransform != null)
            {
                float maxAngle = Mathf.Lerp(25f, 4f, progress);
                float rotZ = Mathf.Sin(elapsed * 26f) * maxAngle;
                playerDiceTransform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
                float punch = 1f + Mathf.Sin(elapsed * 32f) * 0.1f * (1f - progress * 0.7f);
                playerDiceTransform.localScale = new Vector3(origScale.x * punch, origScale.y * punch, origScale.z);
            }

            yield return null;
        }

        if (playerDiceTransform != null)
        {
            playerDiceTransform.localRotation = Quaternion.identity;
            playerDiceTransform.localScale = origScale * 1.2f;
        }

        playerDiceValue = Random.Range(1, 7);
        UpdateDiceVisual(playerDiceValueText, playerDiceValue);

        yield return new WaitForSeconds(0.2f);

        if (playerDiceTransform != null)
        {
            playerDiceTransform.localScale = origScale;
        }

        bool isCorrect = (guessHigher && playerDiceValue > machineDiceValue) || (!guessHigher && playerDiceValue < machineDiceValue);
        bool isEqual = (playerDiceValue == machineDiceValue);

        if (isCorrect)
        {
            currentPot *= 2;
            UpdateHeaderUI();

            SetDialogue($"<color=#00FF7F>¡ACERTASTE!</color> Sacaste un <b>{playerDiceValue}</b> frente al <b>{machineDiceValue}</b> de la casa.\n¡El bote sube a <b>{currentPot} monedas</b>! ¿Sigues arriesgando o cobras?");

            isAnimating = false;
            UpdateButtonStates(true);
        }
        else
        {
            isRoundActive = false;
            string reason = isEqual
                ? $"¡Empate con un <b>{playerDiceValue}</b>! En esta taberna el empate se lo queda la banca."
                : $"¡Fallaste! Esperabas {(guessHigher ? "MAYOR" : "MENOR")} pero sacaste un <b>{playerDiceValue}</b> frente al <b>{machineDiceValue}</b>.";

            SetDialogue($"<color=#FF4500>¡PERDISTE EL BOTE!</color> {reason}\nPerdiste las {currentPot} monedas acumuladas.");
            currentPot = 0;
            UpdateHeaderUI();

            isAnimating = false;
            UpdateButtonStates(false);
            if (buttonNewRoll != null) buttonNewRoll.gameObject.SetActive(true);
        }
    }

    public void OnCashOut()
    {
        if (!isRoundActive || isAnimating || currentPot <= 0) return;

        playerCoins += currentPot;
        int collected = currentPot;
        currentPot = 0;
        isRoundActive = false;

        UpdateHeaderUI();
        SetDialogue($"<color=#FFD700>¡BOTE COBRADO!</color> Te plantas con <b>{collected} monedas</b> en el bolsillo.\nTotal acumulado: <b>{playerCoins}</b> monedas.");

        UpdateButtonStates(false);
        if (buttonNewRoll != null) buttonNewRoll.gameObject.SetActive(true);
    }

    private void UpdateHeaderUI()
    {
        if (coinsText != null) coinsText.text = $"Monedas: <color=#FFD700><b>{playerCoins}</b></color>";
        if (potText != null) potText.text = $"Bote acumulado: <color=#00FF7F><b>{currentPot}</b></color>";
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

    private void UpdateButtonStates(bool inGame)
    {
        if (buttonHigher != null) buttonHigher.interactable = inGame;
        if (buttonLower != null) buttonLower.interactable = inGame;
        if (buttonCashOut != null) buttonCashOut.interactable = inGame && currentPot > initialBet;

        if (buttonNewRoll != null)
        {
            buttonNewRoll.gameObject.SetActive(!inGame);
        }
    }
}
