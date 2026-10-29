using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el menú de pausa de la escena Demo.
/// Al pulsar Escape pausa el juego, congela el tiempo y muestra el menú con fondo al 75% de opacidad.
/// Gestiona la reanudación, apertura del panel de opciones y vuelta al menú principal.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre de la escena del menú principal a cargar al pulsar Salir")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Paneles UI")]
    [Tooltip("Panel principal del menú de pausa (contiene fondo al 75%, título y botones)")]
    public GameObject pausePanel;

    [Tooltip("Contenedor de los botones del menú de pausa para ocultarlos al abrir opciones")]
    public GameObject pauseButtonsContainer;

    [Tooltip("Panel de opciones (el mismo que en el menú principal)")]
    public GameObject optionsPanel;

    [Header("Botones del Menú de Pausa")]
    public Button resumeButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Estado")]
    public bool isPaused = false;

    private void Start()
    {
        // Conexión de listeners de botones
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

        // Conectar el cierre de opciones para restaurar los botones de pausa
        if (optionsPanel != null)
        {
            var omc = optionsPanel.GetComponent<OptionsMenuController>();
            if (omc != null)
            {
                omc.onClosed.AddListener(OnOptionsMenuClosed);
            }
        }

        // Asegurar estado inicial no pausado
        ResumeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }

    public void HandleEscapeKey()
    {
        // 1. Si el panel de opciones está abierto, Escape lo cierra y vuelve al menú de pausa
        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            CloseOptionsMenu();
            return;
        }

        // 2. Si el menú de pausa está abierto, Escape reanuda el juego
        if (isPaused)
        {
            ResumeGame();
        }
        else // 3. Si el juego está corriendo, Escape pausa el juego
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Pausa el juego, congela el tiempo a 0 y muestra el menú de pausa.
    /// </summary>
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

        Debug.Log("[PauseMenu] Juego pausado (TimeScale = 0).");
    }

    /// <summary>
    /// Reanuda el juego, restaura el tiempo a 1 y oculta el menú de pausa.
    /// </summary>
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

        Debug.Log("[PauseMenu] Juego reanudado (TimeScale = 1).");
    }

    /// <summary>
    /// Abre el panel de opciones y oculta los botones de pausa para centrar la atención.
    /// </summary>
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

        Debug.Log("[PauseMenu] Panel de Opciones abierto desde pausa.");
    }

    /// <summary>
    /// Cierra el panel de opciones y restaura los botones de pausa.
    /// </summary>
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

    /// <summary>
    /// Vuelve al menú principal restaurando previamente la escala de tiempo.
    /// </summary>
    public void QuitToMainMenu()
    {
        Debug.Log($"[PauseMenu] Saliendo al menú principal '{mainMenuSceneName}'...");
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("[PauseMenu] No se ha especificado el nombre de la escena del menú principal.");
        }
    }

    private void OnDestroy()
    {
        // Garantizar que la escala de tiempo se restablezca siempre al salir o destruir el objeto
        Time.timeScale = 1f;
    }
}
