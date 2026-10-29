using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config Movimiento")]
    public float speed = 5f;

    [Header("Efecto Profundidad (Fake 3D)")]
    [SerializeField] private Transform topPosition;
    [SerializeField] private Transform bottomPosition;
    //La posicion mas baja de la camara
    private float minY = 0;
    //La posicion mas alta de la camara
    private float maxY = 0;

    public float maxScale = 1.5f; // Tamaño del cubo al estar pegado a la pantalla
    public float minScale = 0.5f; // Tamaño del cubo al estar al fondo

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Gira automáticamente el sprite en X según la dirección horizontal del movimiento")]
    public bool autoFlipX = true;

    private void Start()
    {
        // Buscar automáticamente los componentes visuales en hijos si no están asignados
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Inicializamos minY y maxY con las posiciones de los transform
        if (topPosition != null && bottomPosition != null)
        {
            minY = bottomPosition.position.y;
            maxY = topPosition.position.y;
        }
        else
        {
            Debug.LogWarning("TopPosition o BottomPosition no están asignados en PlayerMovement.");
        }
    }
    void Update()
    {
        // GetAxis devuelve un valor entre -1 y 1, ayuda a que el movimiento sea mas fluido con el mando
        //horizontal y vertical es el nombre de los axis de unity, compatibles pa mando y teclado
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        // Esto aplica el movimiento
        Vector3 movement = new Vector3(moveX, moveY, 0f);
        transform.position += movement * speed * Time.deltaTime;

        // Actualizar animación de movimiento
        bool isMoving = (Mathf.Abs(moveX) > 0.05f || Mathf.Abs(moveY) > 0.05f);
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // Voltear horizontalmente según la dirección
        if (autoFlipX && spriteRenderer != null)
        {
            if (moveX < -0.05f) spriteRenderer.flipX = true;
            else if (moveX > 0.05f) spriteRenderer.flipX = false;
        }

        // InverseLerp da un porcentaje de 0 a 1 depende de donde esta el jugador entre minY y maxY
        float depthPercent = Mathf.InverseLerp(minY, maxY, transform.position.y);

        // Usamos el porcentaje pa interpolar entre el maximo y el minimo de la escala
        float currentScale = Mathf.Lerp(maxScale, minScale, depthPercent);

        // Y esto aplica la escala
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
    public void SetLimitsPosition(Transform top, Transform bottom)
    {
        minY = bottom.position.y;
        maxY = top.position.y;
    }
}