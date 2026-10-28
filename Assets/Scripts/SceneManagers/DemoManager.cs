using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : GameplayManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool interactuable = true;
    protected override void Start()
    {
        base.Start();
        Debug.Log("Iniciando DemoManager");

        //foreach (var sceneObj in sceneObjects)
        //{
        //    Debug.Log("Desactivando " + sceneObj.name);
        //    sceneObj.gameObject.SetActive(false);
        //}
        // exits[0].gameObject.SetActive(false); // Mantener activo para poder hacer clic y viajar a GrandpaSide2D
        foreach (Image closeup in closeUps)
        {
            closeup.gameObject.SetActive(false);
        }

        if (canvas != null)
            canvas.gameObject.SetActive(false);
    }
    public override void ManejarObjetoClickeado(Interactable objetoClickeado)
    {
        if(!interactuable) return;
        Debug.Log("Manejando objeto clickeado: " + objetoClickeado.name);

        Debug.Log("Objeto clickeado: " + objetoClickeado.name);
        if (objetoClickeado == npcs[0] && npcs[0].isPlayerColliding())
        {
            interactuable = false;
            StartCoroutine(ActivarNpcPuente(objetoClickeado));
        }

        // Comparamos el componente Interactable del SceneObject[1]
        else if (objetoClickeado is SceneObject && sceneObjects[objetoClickeado.GetId() - 1].isPlayerColliding())
        {
            interactuable = false;
            StartCoroutine(textObjectScene(objetoClickeado));
        }
        else if (objetoClickeado == exits[0] && exits[0].isPlayerColliding())
        {
            playerMovementScript.SetLimitsPosition(topPositions[1], bottomPositions[1]);
            this.gameObject.transform.position = exits[1].transform.position;
            cameras[0].Priority.Value -= 1;
            cameras[1].Priority.Value += 1;
           
        }
        else if (objetoClickeado == exits[1] && exits[1].isPlayerColliding())
        {
            playerMovementScript.SetLimitsPosition(topPositions[0], bottomPositions[0]);
            this.gameObject.transform.position = exits[0].transform.position;
            cameras[1].Priority.Value -= 1;
            cameras[0].Priority.Value += 1;
            
        }
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
