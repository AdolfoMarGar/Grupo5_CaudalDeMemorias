using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VerticalSliceManager : GameplayManager
{
    [SerializeField] private Image señorino;
    [SerializeField] private Image señorino2;
    private int goalMision = 0;
    void Start()
    {
        Debug.Log("Iniciando VerticalSliceManager");

        foreach (var sceneObj in sceneObjects)
        {
            Debug.Log("Desactivando " + sceneObj.name);
            sceneObj.gameObject.SetActive(false);
        }
        exits[0].gameObject.SetActive(false);
        señorino.enabled = false;
        señorino2.enabled = false;
        if (canvas != null) 
            canvas.gameObject.SetActive(false);
    }

    public override void ManejarObjetoClickeado(Interactable objetoClickeado)
    {
        //necesario en caso de que se quiera mantener la funcionalidad de la clase base, pero en este caso no es necesario
        //base.ManejarObjetoClickeado(objetoClickeado);

        Debug.Log("Objeto clickeado: " + objetoClickeado.name);
        if (objetoClickeado == npcs[0])
        {
            npcs[0].SetActive(false);
            StartCoroutine(ActivarHiervas());
        }
        // Comparamos el componente Interactable del SceneObject[1]
        else if (objetoClickeado == sceneObjects[0] || objetoClickeado == sceneObjects[1] || objetoClickeado == sceneObjects[2])
        {
            StartCoroutine(PickUpGrass(objetoClickeado));
        }else if (npcs[1]== objetoClickeado)
        {
            StartCoroutine(ActivarSeñorino2(objetoClickeado));
        }


    }

    private IEnumerator ActivarSeñorino2(Interactable objetoClickeado)
    {
        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;
        canvas.gameObject.SetActive(true);
        // Mostramos los diálogos
        textScreen.MostrarDialogos(objetoClickeado.GetTexts(), objetoClickeado.GetSelectedText(), objetoClickeado.GetTexts().Count - 1);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;
        señorino.enabled = false;
        canvas.gameObject.SetActive(false);

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        

        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }
    }

    private IEnumerator PickUpGrass(Interactable objetoClickeado)
    {
        int id = objetoClickeado.GetId()-1;
        Debug.Log("Recogiendo hierva: " + objetoClickeado.name);
        objetoClickeado.gameObject.SetActive(false);


        
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
        señorino.enabled = false;
        canvas.gameObject.SetActive(false);

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        goalMision++;
        if(goalMision==2)
            npcs[1].SetActive(true);
        if (goalMision >= 3)
            exits[0].gameObject.SetActive(true);

        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }
    }

    private IEnumerator ActivarHiervas()
    {
        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;
        canvas.gameObject.SetActive(true);
        señorino.enabled = true;
        // Mostramos los diálogos
        textScreen.MostrarDialogos(npcs[0].GetTexts(), npcs[0].GetSelectedText(), npcs[0].GetTexts().Count - 1);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;
        señorino.enabled = false;
        canvas.gameObject.SetActive(false);

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        Debug.Log("ACTIVANDO hiervas");

        foreach (var sceneObj in sceneObjects)
        {
            if (sceneObj != null)
            {
                sceneObj.gameObject.SetActive(true);
            }
        }

        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }
    }
}
