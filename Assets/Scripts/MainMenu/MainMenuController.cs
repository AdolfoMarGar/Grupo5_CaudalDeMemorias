using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string demoSceneName = "Demo";
    public string minigamesSceneName = "MinigamesMenu";

    [Header("Referencias de Botones (Esquina Inferior Izquierda)")]
    public Button startButton;
    public Button fantasiaButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Panel de Opciones")]
    public GameObject optionsPanel;
    public Button closeOptionsButton;

    [Header("Opciones Avanzadas")]
    public GameObject advancedOptionsContent;
    public Image advancedOptionsArrow;
    public Sprite arrowUpSprite;
    public Sprite arrowDownSprite;
    public Button advancedOptionsToggleButton;
    public Button creditsButton;
    public Button restartButton;
    public bool isAdvancedOptionsOpen = false;

    private void Start()
    {
        SetupButtonListener(startButton, OnStartClicked);
        SetupButtonListener(fantasiaButton, OnFantasiaClicked);
        SetupButtonListener(optionsButton, OnOptionsClicked);
        SetupButtonListener(quitButton, OnQuitClicked);
    }

    private void SetupButtonListener(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null) return;
        if (btn.onClick.GetPersistentEventCount() == 0)
        {
            btn.onClick.AddListener(action);
        }
    }

    public void OnStartClicked()
    {
        Debug.Log($"[MainMenu] Iniciando juego. Cargando escena '{demoSceneName}'...");
        if (!string.IsNullOrEmpty(demoSceneName))
        {
            SceneManager.LoadScene(demoSceneName);
        }
        else
        {
            Debug.LogError("[MainMenu] No se ha especificado el nombre de la escena de Demo.");
        }
    }

    public void OnFantasiaClicked()
    {
        Debug.Log($"[MainMenu] Botón Fantasía pulsado. Cargando minijuegos '{minigamesSceneName}'...");
        if (!string.IsNullOrEmpty(minigamesSceneName))
        {
            SceneManager.LoadScene(minigamesSceneName);
        }
        else
        {
            Debug.LogError("[MainMenu] No se ha especificado el nombre de la escena de Minijuegos.");
        }
    }

    public void OnOptionsClicked()
    {
        Debug.Log("[MainMenu] Botón de Opciones pulsado.");
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
    }

    public void OnCloseOptionsClicked()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void ToggleAdvancedOptions()
    {
        SetAdvancedOptions(!isAdvancedOptionsOpen);
    }

    public void SetAdvancedOptions(bool open)
    {
        isAdvancedOptionsOpen = open;
        UpdateAdvancedOptionsUI();
    }

    public void UpdateAdvancedOptionsUI()
    {
        if (advancedOptionsContent != null)
        {
            advancedOptionsContent.SetActive(isAdvancedOptionsOpen);
        }

        if (advancedOptionsArrow != null)
        {
            advancedOptionsArrow.sprite = isAdvancedOptionsOpen ? arrowUpSprite : arrowDownSprite;
        }
    }

    public void OnCreditsClicked()
    {
        Debug.Log("[MainMenu] Botón Créditos pulsado.");
    }

    public void OnRestartClicked()
    {
        Debug.Log("[MainMenu] Botón Reiniciar Partida pulsado.");
    }

    public void OnQuitClicked()
    {
        Debug.Log("[MainMenu] Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
