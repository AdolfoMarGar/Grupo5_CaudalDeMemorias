using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Nombre de la escena a cargar al pulsar Inicio")]
    public string demoSceneName = "Demo";

    [Header("Referencias de Botones (Esquina Inferior Izquierda)")]
    public Button startButton;
    public Button fantasiaButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Panel de Opciones (Para implementar más adelante)")]
    [Tooltip("Panel u objeto de opciones que se abrirá en el futuro")]
    public GameObject optionsPanel;
    public Button closeOptionsButton;

    [Header("Opciones Avanzadas (Desplegable)")]
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
        // Conexión defensiva de los botones principales del menú
        SetupButtonListener(startButton, OnStartClicked);
        SetupButtonListener(fantasiaButton, OnFantasiaClicked);
        SetupButtonListener(optionsButton, OnOptionsClicked);
        SetupButtonListener(quitButton, OnQuitClicked);
    }

    private void SetupButtonListener(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null) return;
        // Si ya tiene un listener persistente configurado en el Inspector, no añadimos uno dinámico para evitar doble llamada
        if (btn.onClick.GetPersistentEventCount() == 0)
        {
            btn.onClick.AddListener(action);
        }
    }

    /// <summary>
    /// Botón 1: Inicio -> Carga directamente la escena Demo.
    /// </summary>
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

    /// <summary>
    /// Botón 2: Fantasía -> Por el momento no hace nada (dejado preparado).
    /// </summary>
    public void OnFantasiaClicked()
    {
        Debug.Log("[MainMenu] Botón Fantasía pulsado (De momento no realiza ninguna acción).");
    }

    /// <summary>
    /// Botón 3: Opciones -> Abre el panel de opciones si existe, o queda como placeholder para después.
    /// </summary>
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
        else
        {
            Debug.Log("[MainMenu] El menú de opciones se implementará más adelante.");
        }
    }

    /// <summary>
    /// Cierra el panel de opciones.
    /// </summary>
    public void OnCloseOptionsClicked()
    {
        Debug.Log("[MainMenu] Cerrando panel de Opciones.");
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Alterna el desplegable de opciones avanzadas.
    /// </summary>
    public void ToggleAdvancedOptions()
    {
        SetAdvancedOptions(!isAdvancedOptionsOpen);
    }

    /// <summary>
    /// Establece el estado del desplegable de opciones avanzadas.
    /// </summary>
    public void SetAdvancedOptions(bool open)
    {
        isAdvancedOptionsOpen = open;
        Debug.Log($"[MainMenu] Opciones Avanzadas: {(isAdvancedOptionsOpen ? "Desplegado (Abierto)" : "Plegado (Cerrado)")}");
        UpdateAdvancedOptionsUI();
    }

    /// <summary>
    /// Actualiza la interfaz del desplegable de opciones avanzadas (flecha y contenido).
    /// </summary>
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

    /// <summary>
    /// Botón 4: Salir -> Cierra el juego (o detiene la ejecución si estamos en el Editor).
    /// </summary>
    public void OnQuitClicked()
    {
        Debug.Log("[MainMenu] Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
