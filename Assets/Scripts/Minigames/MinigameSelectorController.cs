using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinigameSelectorController : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";
    public Button backButton;
    public Transform cardsContainer;
    public GameObject cardPrefab;
    public List<MinigameItemData> minigames = new List<MinigameItemData>();

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
