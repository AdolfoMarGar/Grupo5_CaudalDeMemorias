using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Controlador independiente del panel de opciones.
/// Puede utilizarse tanto en el menú principal como en el menú de pausa de la Demo.
/// </summary>
public class OptionsMenuController : MonoBehaviour
{
    [Header("Botón de Cierre")]
    public Button closeButton;

    [Header("Opciones Avanzadas (Desplegable)")]
    public Button advancedOptionsToggleButton;
    public GameObject advancedOptionsContent;
    public Image advancedOptionsArrow;
    public Sprite arrowUpSprite;
    public Sprite arrowDownSprite;
    public bool isAdvancedOptionsOpen = false;

    [Header("Eventos")]
    public UnityEvent onClosed = new UnityEvent();

    private void Awake()
    {
        // Conexión defensiva: solo añadir si no existe ya un listener persistente en el Inspector
        if (closeButton != null && closeButton.onClick.GetPersistentEventCount() == 0)
        {
            closeButton.onClick.AddListener(CloseOptions);
        }

        if (advancedOptionsToggleButton != null && advancedOptionsToggleButton.onClick.GetPersistentEventCount() == 0)
        {
            advancedOptionsToggleButton.onClick.AddListener(ToggleAdvancedOptions);
        }

        UpdateAdvancedOptionsUI();
    }

    public void OpenOptions()
    {
        gameObject.SetActive(true);
        SetAdvancedOptions(false);
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
        onClosed?.Invoke();
    }

    public void ToggleAdvancedOptions()
    {
        SetAdvancedOptions(!isAdvancedOptionsOpen);
    }

    public void SetAdvancedOptions(bool open)
    {
        isAdvancedOptionsOpen = open;
        Debug.Log($"[OptionsMenu] Opciones avanzadas: {(isAdvancedOptionsOpen ? "Desplegado (Abierto)" : "Plegado (Cerrado)")}");
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
}
