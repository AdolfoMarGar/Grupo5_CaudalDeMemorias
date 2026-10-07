using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f); //La Z en -10 ayuda pa que no atraviese los sprites

    void LateUpdate()
    {
        if (target != null)
        {
            // Esto calcula hacia donde va la camara
            Vector3 desiredPosition = target.position + offset;
            
            // Esto intenta suavizar el movimiento(no esta perfecto)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            
            transform.position = smoothedPosition;
        }
    }
}