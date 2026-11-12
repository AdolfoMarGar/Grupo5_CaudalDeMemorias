using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButtonsContainer;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private bool isPaused = false;

    public bool IsPaused => isPaused;

    private void Start()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OpenOptionsMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitToMainMenu);
        }

        if (optionsPanel != null)
        {
            var omc = optionsPanel.GetComponent<OptionsMenuController>();
            if (omc != null)
            {
                omc.onClosed.AddListener(OnOptionsMenuClosed);
            }
        }

        ResumeGame();
    }

    private void Update()
    {
        if (CheckEscapeInput())
        {
            HandleEscapeKey();
        }
    }

    private bool CheckEscapeInput()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return true;
        }
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            return true;
        }
        return false;
    }

    public void HandleEscapeKey()
    {
        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            CloseOptionsMenu();
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (pauseButtonsContainer != null)
        {
            pauseButtonsContainer.SetActive(true);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void OpenOptionsMenu()
    {
        if (optionsPanel != null)
        {
            var omc = optionsPanel.GetComponent<OptionsMenuController>();
            if (omc != null)
            {
                omc.OpenOptions();
            }
            else
            {
                optionsPanel.SetActive(true);
            }
        }

        if (pauseButtonsContainer != null)
        {
            pauseButtonsContainer.SetActive(false);
        }
    }

    public void CloseOptionsMenu()
    {
        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            var omc = optionsPanel.GetComponent<OptionsMenuController>();
            if (omc != null)
            {
                omc.CloseOptions();
            }
            else
            {
                optionsPanel.SetActive(false);
                OnOptionsMenuClosed();
            }
        }
        else
        {
            OnOptionsMenuClosed();
        }
    }

    public void OnOptionsMenuClosed()
    {
        if (pauseButtonsContainer != null && isPaused)
        {
            pauseButtonsContainer.SetActive(true);
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
