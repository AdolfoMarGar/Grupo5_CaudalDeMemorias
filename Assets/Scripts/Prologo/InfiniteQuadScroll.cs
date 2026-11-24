using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class InfiniteQuadScroll : MonoBehaviour
{
    [SerializeField] public float scrollSpeed = 0.25f;
    [SerializeField] private string texturePropertyName = "_MainTex";

    private Renderer quadRenderer;
    private Material runtimeMaterial;
    private string activeProperty;
    private float offsetX;

    private void Awake()
    {
        quadRenderer = GetComponent<Renderer>();
        runtimeMaterial = quadRenderer.material;

        if (runtimeMaterial != null)
        {
            if (!string.IsNullOrEmpty(texturePropertyName) && runtimeMaterial.HasProperty(texturePropertyName))
            {
                activeProperty = texturePropertyName;
            }
            else if (runtimeMaterial.HasProperty("_BaseMap"))
            {
                activeProperty = "_BaseMap";
            }
            else if (runtimeMaterial.HasProperty("_MainTex"))
            {
                activeProperty = "_MainTex";
            }
        }
    }

    private void Update()
    {
        if (runtimeMaterial == null || string.IsNullOrEmpty(activeProperty))
            return;

        offsetX = (offsetX + scrollSpeed * Time.deltaTime) % 1f;
        if (offsetX < 0f) offsetX += 1f;

        runtimeMaterial.SetTextureOffset(activeProperty, new Vector2(offsetX, 0f));
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}