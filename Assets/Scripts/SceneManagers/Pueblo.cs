using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pueblo : GameplayManager
{    
    private bool interactuable = true;
    public ReproductorSonido sonidoExit;
    public ReproductorSonido sonidoNpc;
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
        if(objetoClickeado is Npcs npc){
            sonidoNpc.ReproducirUnaVez();
            StartCoroutine(TextoNpc(npc));
        }
        
        if (objetoClickeado is Exits exit)
        {
            sonidoExit.ReproducirUnaVez();
            Debug.Log("Objeto clickeado: " + exit.name + " es una salida y el jugador está colisionando con ella.");
            Debug.Log("Su id es: " + exit.GetId());
            // InteractionExit(int idExit, int idExitTo, int idCamara, int idCamaraTo)
            if (exit.GetId() == 1)
            {
                InteractionExit(6, 1, 2);
            }
            else if (exit.GetId() == 2)
            {
                InteractionExit(5 , 1, 2);
            }
            else if (exit.GetId() == 3)
            {
                InteractionExit( 17, 1, 7);
            }
            else if (exit.GetId() == 4)
            {
                InteractionExit( 16, 1, 6);
            }
            else if (exit.GetId() == 5)
            {
                InteractionExit( 2, 2, 1);
            }
            else if (exit.GetId() == 6)
            {
                InteractionExit( 1, 2, 1);
            }
            else if (exit.GetId() == 7)
            {
                InteractionExit( 8, 2, 3);
            }
            else if (exit.GetId() == 8)
            {
                InteractionExit( 7, 3, 2);
            }
            else if (exit.GetId() == 9)
            {
                InteractionExit( 10, 3, 4);
            }
            else if (exit.GetId() == 10)
            {
                InteractionExit( 9, 4, 3);
            }
            else if (exit.GetId() == 11)
            {
                InteractionExit( 14, 4, 6);
            }
            else if (exit.GetId() == 12)
            {
                InteractionExit( 13, 4, 5);
            }
            else if (exit.GetId() == 13)
            {
                InteractionExit( 12, 5, 4);
            }
            else if (exit.GetId() == 14)
            {
                InteractionExit( 11, 6, 4);
            }
            else if (exit.GetId() == 15)
            {
                InteractionExit( 18, 6, 8);
            }
            else if (exit.GetId() == 16)
            {
                InteractionExit( 4, 6, 1);
            }
            else if (exit.GetId() == 17)
            {
                InteractionExit( 3, 7, 1);
            }
            else if (exit.GetId() == 18)
            {
                InteractionExit( 15, 8, 6);
            }
            else if (exit.GetId() == 22)
            {
                SceneManager.LoadScene("Demo");
            }


        }


    }
    private IEnumerator TextoNpc(Npcs npc)
    {
        playerMovementScript.canMove = false;
        playerInteractableScript.puedeInteractuar = false;
        Debug.Log("textoSeleccionado: " + npc.GetSelectedText());
        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;
        canvas.gameObject.SetActive(true);
        closeUps[npc.GetId() - 1].gameObject.SetActive(true);

        // Mostramos los diálogos
        textScreen.MostrarDialogos(npc.GetTexts(), 0, npc.GetTexts().Count - 1);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;
        closeUps[npc.GetId() - 1].gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        Debug.Log("textoSeleccionado: " + npc.GetSelectedText());
        interactuable = true;
        playerMovementScript.canMove = true;
        playerInteractableScript.puedeInteractuar = true;
        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }

    }
}
