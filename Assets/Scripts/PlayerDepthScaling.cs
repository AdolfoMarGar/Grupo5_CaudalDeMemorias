using UnityEngine;

public class PlayerDepthScaling : MonoBehaviour
{
    [SerializeField] private Transform topPosition;
    [SerializeField] private Transform bottomPosition;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float minScale = 0.5f;

    private float minY = 0f;
    private float maxY = 0f;

    public float MaxScale
    {
        get => maxScale;
        set => maxScale = value;
    }

    public float MinScale
    {
        get => minScale;
        set => minScale = value;
    }

    public float MinY => minY;
    public float MaxY => maxY;

    private void Start()
    {
        UpdateLimits();
    }

    private void Update()
    {
        if (Mathf.Approximately(minY, maxY))
        {
            UpdateLimits();
            if (Mathf.Approximately(minY, maxY)) return;
        }

        float depthPercent = Mathf.InverseLerp(minY, maxY, transform.position.y);
        float currentScale = Mathf.Lerp(maxScale, minScale, depthPercent);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }

    public void SetLimitsPosition(Transform top, Transform bottom)
    {
        topPosition = top;
        bottomPosition = bottom;
        UpdateLimits();
    }

    public void SetLimits(float topY, float bottomY)
    {
        topPosition = null;
        bottomPosition = null;
        minY = bottomY;
        maxY = topY;
    }

    private void UpdateLimits()
    {
        if (topPosition != null && bottomPosition != null)
        {
            minY = bottomPosition.position.y;
            maxY = topPosition.position.y;
        }
    }
}
