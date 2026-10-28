using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarCardGameController : MonoBehaviour
{
    [Header("Configuración Económica")]
    [Tooltip("Monedas actuales del jugador")]
    public int playerCoins = 100;

    [Tooltip("Apuesta inicial por cada mano")]
    public int initialBet = 10;

    [Tooltip("Bote acumulado en la racha actual")]
    public int currentPot = 0;

    [Header("Estado de la Baraja")]
    [Tooltip("Valor de la carta actual en mesa (1 al 12)")]
    public int currentCard = 1;
    public string currentSuit = "OROS";

    [Header("Referencias UI")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI potText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI cardValueText;
    public TextMeshProUGUI cardSuitText;
    public TextMeshProUGUI cardCornerTopText;
    public TextMeshProUGUI cardCornerBottomText;
    public RectTransform cardTransform;
    public Image cardImage;

    [Header("Botones")]
    public Button buttonHigher;
    public Button buttonLower;
    public Button buttonCashOut;
    public Button buttonNewHand;

    private bool isRoundActive = false;
    private bool isAnimating = false;

    private readonly string[] suits = { "OROS", "COPAS", "ESPADAS", "BASTOS" };
    private readonly string[] suitSymbols = { "[O]", "[C]", "[E]", "[B]" };
    private readonly Color[] suitColors = {
        new Color(0.88f, 0.68f, 0.08f), // Oros (dorado)
        new Color(0.85f, 0.22f, 0.22f), // Copas (rojo)
        new Color(0.22f, 0.48f, 0.88f), // Espadas (azul acero)
        new Color(0.22f, 0.65f, 0.25f)  // Bastos (verde madera)
    };
    private int currentSuitIndex = 0;

    private void Start()
    {
        // Conectar eventos onClick en Start() para que funcione al instante
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

        if (buttonNewHand != null)
        {
            buttonNewHand.onClick.RemoveAllListeners();
            buttonNewHand.onClick.AddListener(StartNewRound);
        }

        // Iniciar la ronda inicial al darle a Play
        StartNewRound();
    }

    public void StartNewRound()
    {
        if (isAnimating) return;

        // Cortesía del tabernero si te quedas sin monedas
        if (playerCoins < initialBet)
        {
            playerCoins = 50;
            SetDialogue("¡Te quedaste sin blanca! Toma 50 monedas cortesía de la casa.");
        }

        // Deducir apuesta inicial y fijar el bote inicial
        playerCoins -= initialBet;
        currentPot = initialBet;
        isRoundActive = true;

        // Sacar carta aleatoria (1-12)
        currentCard = Random.Range(1, 13);
        currentSuitIndex = Random.Range(0, suits.Length);
        currentSuit = suits[currentSuitIndex];

        UpdateCardVisual(currentCard, currentSuitIndex);
        UpdateHeaderUI();

        SetDialogue($"¡Hagan juego! Carta actual: <b>{GetCardName(currentCard)}</b> de <b>{currentSuit}</b>.\n¿La siguiente será <b>MAYOR</b> o <b>MENOR</b>?");

        UpdateButtonStates(true);
    }

    public void OnPlayerGuess(bool guessHigher)
    {
        if (!isRoundActive || isAnimating) return;

        StartCoroutine(FlipAndRevealRoutine(guessHigher));
    }

    private IEnumerator FlipAndRevealRoutine(bool guessHigher)
    {
        isAnimating = true;
        UpdateButtonStates(false);

        SetDialogue("El tabernero voltea la siguiente carta...");

        // Efecto giro escala X de 1 a 0 en 0.15s
        float elapsed = 0f;
        float halfDuration = 0.15f;
        Vector3 originalScale = cardTransform != null ? cardTransform.localScale : Vector3.one;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            if (cardTransform != null)
            {
                cardTransform.localScale = new Vector3(Mathf.Lerp(1f, 0f, t), originalScale.y, originalScale.z);
            }
            yield return null;
        }

        //  Revelar la nueva carta
        int nextCard = Random.Range(1, 13);
        int nextSuitIndex = Random.Range(0, suits.Length);
        string nextSuit = suits[nextSuitIndex];

        UpdateCardVisual(nextCard, nextSuitIndex);

        //  Efecto giro escala X de 0 a 1 en 0.15s
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            if (cardTransform != null)
            {
                cardTransform.localScale = new Vector3(Mathf.Lerp(0f, 1f, t), originalScale.y, originalScale.z);
            }
            yield return null;
        }

        if (cardTransform != null)
        {
            cardTransform.localScale = originalScale;
        }

        // Evalua la condición de victoria o derrota
        bool isCorrect = (guessHigher && nextCard > currentCard) || (!guessHigher && nextCard < currentCard);
        bool isEqual = (nextCard == currentCard);

        if (isCorrect)
        {
            // El bote se duplica, 10,20,40,80 etc
            currentPot *= 2;
            currentCard = nextCard;
            currentSuitIndex = nextSuitIndex;
            currentSuit = nextSuit;

            UpdateHeaderUI();
            SetDialogue($"<color=#00FF7F>¡CORRECTO!</color> Salió <b>{GetCardName(nextCard)}</b> de <b>{nextSuit}</b>.\n¡El bote sube a <b>{currentPot} monedas</b>! ¿Sigues arriesgando o cobras?");

            isAnimating = false;
            UpdateButtonStates(true);
        }
        else
        {
            isRoundActive = false;
            string reason = isEqual
                ? $"¡Empate con un <b>{nextCard}</b>! En este bar el empate se lo queda la banca."
                : $"¡Fallaste! Esperabas {(guessHigher ? "MAYOR" : "MENOR")} pero salió el <b>{GetCardName(nextCard)}</b> de <b>{nextSuit}</b>.";

            SetDialogue($"<color=#FF4500>¡PERDISTE EL BOTE!</color> {reason}\nPerdiste las {currentPot} monedas acumuladas.");
            currentPot = 0;
            UpdateHeaderUI();

            isAnimating = false;
            UpdateButtonStates(false);
            if (buttonNewHand != null) buttonNewHand.gameObject.SetActive(true);
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
        if (buttonNewHand != null) buttonNewHand.gameObject.SetActive(true);
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

    private void UpdateCardVisual(int number, int suitIdx)
    {
        string numStr = number.ToString();
        string symbol = suitSymbols[suitIdx];
        Color col = suitColors[suitIdx];

        if (cardValueText != null)
        {
            cardValueText.text = numStr;
            cardValueText.color = col;
        }

        if (cardSuitText != null)
        {
            cardSuitText.text = suits[suitIdx];
            cardSuitText.color = col;
        }

        if (cardCornerTopText != null)
        {
            cardCornerTopText.text = $"{numStr}\n{symbol}";
            cardCornerTopText.color = col;
        }

        if (cardCornerBottomText != null)
        {
            cardCornerBottomText.text = $"{numStr}\n{symbol}";
            cardCornerBottomText.color = col;
        }
    }

    private void UpdateButtonStates(bool inGame)
    {
        if (buttonHigher != null) buttonHigher.interactable = inGame;
        if (buttonLower != null) buttonLower.interactable = inGame;
        if (buttonCashOut != null) buttonCashOut.interactable = inGame && currentPot > initialBet;

        if (buttonNewHand != null)
        {
            buttonNewHand.gameObject.SetActive(!inGame);
        }
    }

    private string GetCardName(int number)
    {
        switch (number)
        {
            case 1: return "As (1)";
            case 10: return "Sota (10)";
            case 11: return "Caballo (11)";
            case 12: return "Rey (12)";
            default: return $"{number}";
        }
    }
}
