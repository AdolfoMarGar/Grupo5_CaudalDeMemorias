using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider2D))]
public class SceneChangerOnClick : MonoBehaviour
{
    [Header("Configuración de Cambio de Escena")]
    [Tooltip("Nombre exacto de la escena a cargar al hacer clic")]
    public string sceneToLoad = "GrandpaSide2D";

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        LoadTargetScene();
    }

    private void Update()
    {
        // Respaldo de detección de clic para compatibilidad con New Input System y Legacy Input
        bool clicked = false;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicked = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (!clicked)
        {
            try
            {
                if (Input.GetMouseButtonDown(0))
                    clicked = true;
            }
            catch { }
        }
#endif

        if (clicked && col != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 mousePos = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
                if (Mouse.current != null)
                {
                    mousePos = Mouse.current.position.ReadValue();
                }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
                if (mousePos == Vector3.zero)
                {
                    try { mousePos = Input.mousePosition; } catch { }
                }
#endif
                Vector2 worldPoint = cam.ScreenToWorldPoint(mousePos);
                if (col.OverlapPoint(worldPoint))
                {
                    LoadTargetScene();
                }
            }
        }
    }

    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"[SceneChangerOnClick] Nombre de escena vacío en {gameObject.name}");
            return;
        }

        Debug.Log($"[SceneChangerOnClick] Clic detectado en {gameObject.name}. Cargando escena '{sceneToLoad}'...");
        SceneManager.LoadScene(sceneToLoad);
    }
}
