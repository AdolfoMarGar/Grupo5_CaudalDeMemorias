using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[ExecuteAlways]
public class FlowerSlider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Referencias UI")]
    [Tooltip("Imagen que muestra el relleno de flores blancas (Filled Horizontal)")]
    public Image fillImage;

    [Tooltip("Texto o etiqueta del slider (opcional)")]
    public TMPro.TextMeshProUGUI labelText;

    [Header("Valor del Slider (0 a 7)")]
    [Range(0, 7)]
    [SerializeField]
    private int currentValue = 0;

    [System.Serializable]
    public class FlowerSliderEvent : UnityEvent<int> { }

    [Header("Eventos")]
    public FlowerSliderEvent onValueChanged = new FlowerSliderEvent();

    // Puntos de corte exactos medidos en píxeles para los valles entre las 7 flores
    // 0 = ninguna flor blanca (0%)
    // 1..7 = cada flor respectiva totalmente revelada en blanco
    public static readonly float[] FlowerCutoffs = new float[]
    {
        0.000f, // 0 flores blancas (100% negro)
        0.145f, // Flor 1
        0.252f, // Flor 2
        0.391f, // Flor 3
        0.532f, // Flor 4
        0.692f, // Flor 5
        0.879f, // Flor 6
        1.000f  // Flor 7 (todas blancas)
    };

    private static readonly float[] ClickThresholds = new float[]
    {
        0.070f, // Límite para 0
        0.198f, // Límite para 1
        0.321f, // Límite para 2
        0.461f, // Límite para 3
        0.612f, // Límite para 4
        0.785f, // Límite para 5
        0.939f  // Límite para 6 (mayor o igual es 7)
    };

    public int Value
    {
        get => currentValue;
        set => SetValue(value);
    }

    private void Awake()
    {
        UpdateVisuals();
    }

    private void Start()
    {
        UpdateVisuals();
    }

    private void OnValidate()
    {
        currentValue = Mathf.Clamp(currentValue, 0, 7);
        UpdateVisuals();
    }

    public void SetValue(int newValue, bool notify = true)
    {
        int clamped = Mathf.Clamp(newValue, 0, 7);
        if (currentValue != clamped)
        {
            currentValue = clamped;
            UpdateVisuals();
            if (notify)
            {
                onValueChanged?.Invoke(currentValue);
            }
        }
        else
        {
            UpdateVisuals();
        }
    }

    public void UpdateVisuals()
    {
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = FlowerCutoffs[Mathf.Clamp(currentValue, 0, 7)];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        HandlePointerInput(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        HandlePointerInput(eventData);
    }

    private void HandlePointerInput(PointerEventData eventData)
    {
        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            float width = rectTransform.rect.width;
            if (width <= 0) return;

            // u normalizado de 0 a 1 a lo largo del ancho del slider
            float u = (localPoint.x - rectTransform.rect.xMin) / width;
            u = Mathf.Clamp01(u);

            int flowerIndex = 7;
            for (int i = 0; i < ClickThresholds.Length; i++)
            {
                if (u < ClickThresholds[i])
                {
                    flowerIndex = i;
                    break;
                }
            }

            SetValue(flowerIndex, true);
        }
    }
}
