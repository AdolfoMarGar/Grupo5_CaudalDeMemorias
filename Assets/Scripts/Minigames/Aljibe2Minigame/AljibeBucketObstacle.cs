using UnityEngine;

public class AljibeBucketObstacle : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3.8f;

    public float FallSpeed
    {
        get => fallSpeed;
        set => fallSpeed = value;
    }

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
}
