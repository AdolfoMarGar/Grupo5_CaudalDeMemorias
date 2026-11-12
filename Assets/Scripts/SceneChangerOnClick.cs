using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class SceneChangerOnClick : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GrandpaSide2D";

    private Collider2D col;

    public string SceneToLoad
    {
        get => sceneToLoad;
        set => sceneToLoad = value;
    }

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
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && col != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
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
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
