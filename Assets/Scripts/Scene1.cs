using UnityEngine;
using System.Collections.Generic;

public class Scene1 : MonoBehaviour
{
    private int scenarioId;
    [SerializeField]  private List<Scenarios> scenarios = new List<Scenarios>();
    private List<Npcs> npcs = new List<Npcs>();
    private List<SceneObject> sceneObjects = new List<SceneObject>();
    private List<Exits> exits = new List<Exits>();
    private TextScreen textScreen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textScreen = FindAnyObjectByType<TextScreen>();
        if (textScreen == null)
        {
            Debug.LogError("¡No se encontró ningún TextScreen en la escena!");
        }

        if (scenarios == null || scenarios.Count == 0)
        {
            Debug.LogError("No hay escenarios asignados en Scene1.");
            return;
        }
        scenarioId = scenarios[0].GetId();
        npcs = scenarios[0].GetNpcs();
        sceneObjects = scenarios[0].GetSceneObjects();
        exits = scenarios[0].GetExits();
        Debug.Log("Scene1 tiene " + scenarios.Count + " escenarios.");
        Debug.Log("Scene1 tiene " + npcs.Count + " NPCs.");
        Debug.Log("Scene1 tiene " + sceneObjects.Count + " SceneObjects.");
        Debug.Log("Scene1 tiene " + exits.Count + " Exits.");

        RegistrarEventosInteractables();
    }

    private void RegistrarEventosInteractables()
    {
        // 1. Registrar SceneObjects
        foreach (var sceneObj in sceneObjects)
        {
            if (sceneObj != null)
            {
                Interactable interactableComponent = sceneObj.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked += ManejarObjetoClickeado;
                }
            }
        }

        // 2. Registrar NPCs
        foreach (var npc in npcs)
        {
            if (npc != null)
            {
                Interactable interactableComponent = npc.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked += ManejarObjetoClickeado;
                }
            }
        }

        // 3. Registrar Exits
        foreach (var exit in exits)
        {
            if (exit != null)
            {
                Interactable interactableComponent = exit.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked += ManejarObjetoClickeado;
                }
            }
        }
    }

    private void ManejarObjetoClickeado(Interactable objetoClickeado)
    {
        //Debug.Log($"[Scene1] Se hizo clic en el objeto interactuable: {objetoClickeado.gameObject.name} con ID: {objetoClickeado.GetId()}");
        if (objetoClickeado == npcs[1])
        {
            Debug.Log("ACTIVANDO objeto 2");
            sceneObjects[1].SetActive(true);
        }
        if(objetoClickeado == sceneObjects[1])
        {
            Debug.Log("ACTIVANDO salida 1");
            textScreen.MostrarDialogos(sceneObjects[1].GetTexts(), sceneObjects[1].GetSelectedText(), sceneObjects[1].GetTexts().Count - 1);
            exits[0].SetActive(true);
        }

    }

    private void OnDestroy()
    {
        foreach (var sceneObj in sceneObjects)
        {
            if (sceneObj.gameObject != null)
            {
                Interactable interactableComponent = sceneObj.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked -= ManejarObjetoClickeado;
                }
            }
        }
        foreach (var npc in npcs)
        {
            if (npc.gameObject != null)
            {
                Interactable interactableComponent = npc.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked -= ManejarObjetoClickeado;
                }
            }
        }
        foreach (var exit in exits)
        {
            if (exit.gameObject != null)
            {
                Interactable interactableComponent = exit.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked -= ManejarObjetoClickeado;
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
