using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinigameSelectorController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Button backButton;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<MinigameItemData> minigames = new List<MinigameItemData>();

    public string MainMenuSceneName
    {
        get => mainMenuSceneName;
        set => mainMenuSceneName = value;
    }

    public Button BackButton
    {
        get => backButton;
        set => backButton = value;
    }

    public Transform CardsContainer
    {
        get => cardsContainer;
        set => cardsContainer = value;
    }

    public GameObject CardPrefab
    {
        get => cardPrefab;
        set => cardPrefab = value;
    }

    public List<MinigameItemData> Minigames => minigames;

    private void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackClicked);
        }

        SetupCards();
    }

    [ContextMenu("Actualizar Tarjetas")]
    public void SetupCards()
    {
        if (cardsContainer == null || cardPrefab == null) return;

        int existingCount = cardsContainer.childCount;

        if (existingCount == minigames.Count)
        {
            for (int i = 0; i < minigames.Count; i++)
            {
                var cardUI = cardsContainer.GetChild(i).GetComponent<MinigameCardUI>();
                if (cardUI != null)
                {
                    cardUI.Setup(minigames[i], LaunchMinigame);
                }
            }
        }
        else
        {
            for (int i = cardsContainer.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Destroy(cardsContainer.GetChild(i).gameObject);
                }
                else
                {
                    DestroyImmediate(cardsContainer.GetChild(i).gameObject);
                }
            }

            foreach (var mg in minigames)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardsContainer);
                cardObj.name = "MinigameCard_" + mg.id;
                cardObj.SetActive(true);
                MinigameCardUI cardUI = cardObj.GetComponent<MinigameCardUI>();
                if (cardUI != null)
                {
                    cardUI.Setup(mg, LaunchMinigame);
                }
            }
        }
    }

    public void LaunchMinigame(MinigameItemData item)
    {
        if (item != null && item.isUnlocked && !string.IsNullOrEmpty(item.sceneName))
        {
            SceneManager.LoadScene(item.sceneName);
        }
    }

    public void OnBackClicked()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
