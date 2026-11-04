using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameCardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image thumbnailImage;
    public GameObject lockOverlay;
    public TextMeshProUGUI statusText;
    public Button playButton;
    public TextMeshProUGUI playButtonText;

    private MinigameItemData data;
    private System.Action<MinigameItemData> onPlayClicked;

    public void Setup(MinigameItemData itemData, System.Action<MinigameItemData> clickCallback)
    {
        data = itemData;
        onPlayClicked = clickCallback;

        if (titleText != null)
        {
            titleText.text = data.title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.description;
        }

        if (thumbnailImage != null && data.thumbnail != null)
        {
            thumbnailImage.sprite = data.thumbnail;
        }

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!data.isUnlocked);
        }

        if (statusText != null)
        {
            statusText.text = data.isUnlocked ? "DISPONIBLE" : "BLOQUEADO";
            statusText.color = data.isUnlocked ? new Color(0.85f, 0.75f, 0.35f, 1f) : new Color(0.75f, 0.3f, 0.3f, 1f);
        }

        if (playButton != null)
        {
            playButton.interactable = data.isUnlocked && !string.IsNullOrEmpty(data.sceneName);
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() =>
            {
                if (data.isUnlocked && onPlayClicked != null)
                {
                    onPlayClicked(data);
                }
            });
        }

        if (playButtonText != null)
        {
            playButtonText.text = data.isUnlocked ? "JUGAR" : "BLOQUEADO";
        }
    }
}
