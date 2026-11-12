using UnityEngine;
using UnityEngine.SceneManagement;

public class Pueblo : GameplayManager
{    
    private bool interactuable = true;

    protected override void Start()
    {
        Debug.Log("Iniciando Pueblo");
        base.Start();
        playerMovementScript.canMove = true;
        playerInteractableScript.puedeInteractuar = true;
    }

    public override void ManejarInteractable(Interactable objetoClickeado)
    {
        if (!interactuable) return;


        Debug.Log("Manejando interactuable: " + objetoClickeado.name);

        
        if (objetoClickeado is Exits)
        {
            Debug.Log("Objeto clickeado: " + objetoClickeado.name + " es una salida y el jugador está colisionando con ella.");
            Debug.Log("Su id es: " + objetoClickeado.GetId());
            // InteractionExit(int idExit, int idExitTo, int idCamara, int idCamaraTo)
            if (objetoClickeado.GetId() == 1)
            {
                InteractionExit(5, 1, 2);
            }
            else if (objetoClickeado.GetId() == 2)
            {
                InteractionExit(6 , 1, 2);
            }
            else if (objetoClickeado.GetId() == 3)
            {
                InteractionExit( 17, 1, 7);
            }
            else if (objetoClickeado.GetId() == 4)
            {
                InteractionExit( 16, 1, 6);
            }
            else if (objetoClickeado.GetId() == 5)
            {
                InteractionExit( 1, 2, 1);
            }
            else if (objetoClickeado.GetId() == 6)
            {
                InteractionExit( 2, 2, 1);
            }
            else if (objetoClickeado.GetId() == 7)
            {
                InteractionExit( 8, 2, 3);
            }
            else if (objetoClickeado.GetId() == 8)
            {
                InteractionExit( 7, 3, 2);
            }
            else if (objetoClickeado.GetId() == 9)
            {
                InteractionExit( 10, 3, 4);
            }
            else if (objetoClickeado.GetId() == 10)
            {
                InteractionExit( 9, 4, 3);
            }
            else if (objetoClickeado.GetId() == 11)
            {
                InteractionExit( 14, 4, 6);
            }
            else if (objetoClickeado.GetId() == 12)
            {
                InteractionExit( 13, 4, 5);
            }
            else if (objetoClickeado.GetId() == 13)
            {
                InteractionExit( 12, 5, 4);
            }
            else if (objetoClickeado.GetId() == 14)
            {
                InteractionExit( 11, 6, 4);
            }
            else if (objetoClickeado.GetId() == 15)
            {
                InteractionExit( 18, 6, 8);
            }
            else if (objetoClickeado.GetId() == 16)
            {
                InteractionExit( 4, 6, 1);
            }
            else if (objetoClickeado.GetId() == 17)
            {
                InteractionExit( 3, 7, 1);
            }
            else if (objetoClickeado.GetId() == 18)
            {
                InteractionExit( 15, 8, 6);
            }


        }


    }
}
