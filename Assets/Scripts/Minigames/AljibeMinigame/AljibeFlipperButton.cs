using UnityEngine;
using UnityEngine.EventSystems;

public class AljibeFlipperButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private AljibeFlipper targetFlipper;

    public AljibeFlipper TargetFlipper
    {
        get => targetFlipper;
        set => targetFlipper = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetFlipper != null) targetFlipper.SetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetFlipper != null) targetFlipper.SetPressed(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetFlipper != null) targetFlipper.SetPressed(false);
    }
}
