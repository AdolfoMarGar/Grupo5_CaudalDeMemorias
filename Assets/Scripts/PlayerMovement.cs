using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config Movimiento")]
    public float speed = 5f;

    [Header("Efecto Profundidad (Fake 3D)")]
    //La posicion mas baja de la camara
    public float minY = -4f; 
    //La posicion mas alta de la camara
    public float maxY = 4f;  
    
    public float maxScale = 1.5f; // Tamaño del cubo al estar pegado a la pantalla
    public float minScale = 0.5f; // Tamaño del cubo al estar al fondo

    void Update()
    {
        // GetAxis devuelve un valor entre -1 y 1, ayuda a que el movimiento sea mas fluido con el mando
        //horizontal y vertical es el nombre de los axis de unity, compatibles pa mando y teclado
        float moveX = Input.GetAxis("Horizontal");//da error
        /*
         * InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package in Player Settings.
UnityEngine.Internal.InputUnsafeUtility.GetAxis (System.String axisName) (at <1fbf520707494e6fb187ca6d516a21b5>:0)
UnityEngine.Input.GetAxis (System.String axisName) (at <1fbf520707494e6fb187ca6d516a21b5>:0)
PlayerMovement.Update () (at Assets/Scripts/PlayerMovement.cs:21)

         */
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