using UnityEngine;
using UnityEngine.EventSystems;

public class AljibeBucketMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private AljibeBucketPlayer targetPlayer;
    [SerializeField] private int direction = -1;

    public AljibeBucketPlayer TargetPlayer
    {
        get => targetPlayer;
        set => targetPlayer = value;
    }

    public int Direction
    {
        get => direction;
        set => direction = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetPlayer != null) targetPlayer.SetPointerDirection(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetPlayer != null) targetPlayer.SetPointerDirection(0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetPlayer != null) targetPlayer.SetPointerDirection(0);
    }
}
