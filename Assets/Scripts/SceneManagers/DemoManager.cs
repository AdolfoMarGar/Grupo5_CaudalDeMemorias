using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoManager : GameplayManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool interactuable = true;
    [SerializeField] private TreeMinigame treeMinigame;
    [SerializeField] private BarCardGameController cardScript;
    [SerializeField] private Canvas canvaCard;
    protected override void Start()
    {
        Debug.Log("Iniciando DemoManager");
        base.Start();
        treeMinigame.gameObject.SetActive(false);

        foreach (Image closeup in closeUps)
        {
            closeup.gameObject.SetActive(false);
        }

        if (canvas != null)
            canvas.gameObject.SetActive(false);
        exits[3].SetActive(false);
        canvaCard.gameObject.SetActive(false);
    }
    public override void ManejarObjetoClickeado(Interactable objetoClickeado)
    {
        if(!interactuable) return;
        Debug.Log("Manejando objeto clickeado: " + objetoClickeado.name);

        //Mostrar texto pantalla npc1
        Debug.Log("Objeto clickeado: " + objetoClickeado.name);
        if (objetoClickeado == npcs[0] && npcs[0].isPlayerColliding())
        {
            interactuable = false;
            StartCoroutine(ActivarNpcPuente(objetoClickeado));
        }

        // Mostramos texto de cualquier objeto de la escena
        else if (objetoClickeado is SceneObject && sceneObjects[objetoClickeado.GetId() - 1].isPlayerColliding())
        {
            if (objetoClickeado.GetId() == 3)
            {
                interactuable = false;
                StartCoroutine(EjecutarMinijuegoArbol(objetoClickeado));
            }
            else if(objetoClickeado.GetId() == 4)
            {
                //Activar minijuego cartas
                StartCoroutine(EjecutarMinijuegoCartas(objetoClickeado));

            }
            else
            {
                interactuable = false;
                StartCoroutine(textObjectScene(objetoClickeado));
            }
            
        }
        else if (objetoClickeado is Exits  && exits[objetoClickeado.GetId() - 1].isPlayerColliding())
        {
            Debug.Log("Objeto clickeado: " + objetoClickeado.name + " es una salida y el jugador está colisionando con ella.");

            // InteractionExit(int idExit, int idExitTo, int idCamara, int idCamaraTo)
            if (objetoClickeado.GetId() == 1)
            {
                InteractionExit(1, 2, 1, 2);
            }
            else if(objetoClickeado.GetId() == 2)
            {
                InteractionExit(2, 1, 2, 1);
            }
            else if (objetoClickeado.GetId() == 3)
            {
                InteractionExit(3, 5, 2, 3);
            }
            else if (objetoClickeado.GetId() == 5)
            {
                InteractionExit(5, 3, 3, 2);
            }
            else if (objetoClickeado.GetId() == 4)
            {
                InteractionExit(4, 6, 2, 5);
            }
            else if (objetoClickeado.GetId() == 6)
            {
                InteractionExit(6, 4, 5, 2);
            }
            else if(objetoClickeado.GetId() == 7)
            {
                SceneManager.LoadScene("GrandpaSide2D");
            }


        }

    }
    private IEnumerator EjecutarMinijuegoCartas(Interactable objetoClickeado)
    {
        canvaCard.gameObject.SetActive(true);
        interactuable = false;


        yield return new WaitUntil(() => cardScript.playerCoins<=70 || cardScript.playerCoins>=130);

        canvaCard.gameObject.SetActive(false);


        // Reactivar la interacción general
        interactuable = true;
    }

    private IEnumerator EjecutarMinijuegoArbol(Interactable objetoClickeado)
    {
        treeMinigame.gameObject.SetActive(true);
        cameras[3].Priority.Value = 2;


        yield return new WaitUntil(() => treeMinigame.ended);


        cameras[3].Priority.Value = 0;

        sceneObjects[objetoClickeado.GetId() - 1].gameObject.SetActive(false);
        sceneObjects[1].gameObject.SetActive(false);
        treeMinigame.gameObject.SetActive(false);
        exits[3].SetActive(true);

        // Reactivar la interacción general
        interactuable = true;
    }
    private void InteractionExit(int idExit, int idExitTo, int idCamara, int idCamaraTo)
    {
        var camaraDestino = cameras[idCamaraTo - 1];
        Vector3 nuevaPosicion = exits[idExitTo - 1].transform.position;
        this.gameObject.transform.position = nuevaPosicion;

        playerMovementScript.SetLimitsPosition(topPositions[idCamara - 1], bottomPositions[idCamara - 1]);

        camaraDestino.transform.position = new Vector3(
            nuevaPosicion.x,
            nuevaPosicion.y,
            camaraDestino.transform.position.z
        );
        camaraDestino.PreviousStateIsValid = false;

        cameras[idCamara-1].Priority.Value -= 1;
        cameras[idCamaraTo-1].Priority.Value += 1;

    }
    private IEnumerator ActivarNpcPuente(Interactable objetoClickeado)
    {
        Debug.Log("textoSeleccionado: " + objetoClickeado.GetSelectedText());
        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;
        canvas.gameObject.SetActive(true);
        closeUps[0].gameObject.SetActive(true);

        // Mostramos los diálogos
        textScreen.MostrarDialogos(objetoClickeado.GetTexts(), objetoClickeado.GetSelectedText(), objetoClickeado.GetTexts().Count - 1);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;
        closeUps[0].gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        Debug.Log("textoSeleccionado: " + objetoClickeado.GetSelectedText());
        interactuable = true;

        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }
    }

    private IEnumerator textObjectScene(Interactable objetoClickeado)
    {
        int id = objetoClickeado.GetId() - 1;
        Debug.Log("ObjectScene: " + objetoClickeado.name);
        //objetoClickeado.gameObject.SetActive(false);



        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;
        canvas.gameObject.SetActive(true);
        // Mostramos los diálogos
        textScreen.MostrarDialogos(sceneObjects[id].GetTexts(), sceneObjects[id].GetSelectedText(), sceneObjects[id].GetTexts().Count - 1);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;

        canvas.gameObject.SetActive(false);
        interactuable = true;

        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
