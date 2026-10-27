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

    private void Start()
    {
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


        // InverseLerp da un porcentaje de 0 a 1 depende de donde esta el jugador entre minY y maxY
        float depthPercent = Mathf.InverseLerp(minY, maxY, transform.position.y);

        // Usamos el porcentaje pa interpolar entre el maximo y el minimo de la escala
        float currentScale = Mathf.Lerp(maxScale, minScale, depthPercent);

        // Y esto aplica la escala
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
}