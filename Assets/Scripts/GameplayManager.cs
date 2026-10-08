using UnityEngine;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{
    private int scenarioId;
    [SerializeField]  private List<Scenarios> scenarios = new List<Scenarios>();
    [SerializeField] protected List<Npcs> npcs = new List<Npcs>();
    [SerializeField] protected List<SceneObject> sceneObjects = new List<SceneObject>();
    [SerializeField] protected List<Exits> exits = new List<Exits>();
    [SerializeField] protected TextScreen textScreen;
    [SerializeField] protected Canvas canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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

        RegistrarEventosInteractables(scenarios[0].GetId());
    }

    private void RegistrarEventosInteractables(int scenarioId)
    {
        //necesario ya que los id empiezan desde 1
        scenarioId -= 1;

        Debug.Log("Registrando eventos de interactuables para el escenario con ID: " + (scenarioId + 1));
        if (scenarios != null)
        {
            Debug.Log("Escenarios disponibles: " + scenarios.Count);
            Debug.Log("Escenario actual: " + scenarioId);
            npcs = scenarios[scenarioId].GetNpcs();
            sceneObjects = scenarios[scenarioId].GetSceneObjects();
            exits = scenarios[scenarioId].GetExits();
            Debug.Log("Scene"+ scenarioId+" tiene " + scenarios.Count + " escenarios.");
            Debug.Log("Scene"+ scenarioId+" tiene " + npcs.Count + " NPCs.");
            Debug.Log("Scene"+ scenarioId+" tiene " + sceneObjects.Count + " SceneObjects.");
            Debug.Log("Scene"+ scenarioId+" tiene " + exits.Count + " Exits.");
           
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
            // Dentro de RegistrarEventosInteractables, en el bucle de NPCs:
            foreach (var npc in npcs)
            {
                if (npc != null)
                {
                    Interactable interactableComponent = npc.GetComponent<Interactable>();
                    if (interactableComponent != null)
                    {
                        Debug.Log("Suscrito correctamente al NPC: " + npc.name); // <--- ¿Aparece este log?
                        interactableComponent.OnClicked += ManejarObjetoClickeado;
                    }
                    else
                    {
                        Debug.LogError("El NPC " + npc.name + " no tiene componente Interactable!", npc);
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
        
    }

    public virtual void ManejarObjetoClickeado(Interactable objetoClickeado)
    {
        

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
    public int getScenarioId()
    {
        return scenarioId;
    }
}
