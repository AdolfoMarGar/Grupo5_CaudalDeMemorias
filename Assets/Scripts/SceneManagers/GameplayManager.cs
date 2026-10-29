using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Cinemachine;


public class GameplayManager : MonoBehaviour
{
    protected int scenarioId;
    [SerializeField]  private List<Scenarios> scenarios = new List<Scenarios>();
    protected List<Npcs> npcs = new List<Npcs>();
    protected List<SceneObject> sceneObjects = new List<SceneObject>();
    protected List<Exits> exits = new List<Exits>();
    [SerializeField] protected TextScreen textScreen;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected List<Image> closeUps;
    [SerializeField] protected List<CinemachineCamera> cameras;
    [SerializeField] protected PlayerMovement playerMovementScript;
    [SerializeField] protected List<Transform> topPositions;
    [SerializeField] protected List<Transform> bottomPositions;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (textScreen == null)
        {
            Debug.LogError("¡No se encontró ningún TextScreen en la escena!");
        }

        if (scenarios == null || scenarios.Count == 0)
        {
            Debug.LogError("No hay escenarios asignados en Scene1.");
            return;
        }
        if(canvas == null)
        {
            Debug.LogError("¡No se encontró ningún Canvas en la escena!");
        }
        if(cameras == null || cameras.Count == 0)
        {
            Debug.LogError("¡No se encontraron cámaras en la escena!");
        }
        if(closeUps == null || closeUps.Count == 0)
        {
            Debug.LogError("¡No se encontraron CloseUps en la escena!");
        }
        if (!FindAnyObjectByType<PlayerMovement>())
        {
            Debug.LogError("¡No se encontró ningún PlayerMovement en la escena!");
        }
        else
        {
            playerMovementScript = FindAnyObjectByType<PlayerMovement>();
        }

        RegistrarEventosInteractables();
    }

    private void RegistrarEventosInteractables()
    {
        Debug.Log("Registrando eventos de interactuables para todos los escenarios.");
        if (scenarios == null || scenarios.Count == 0) return;
        {
            Debug.Log("Escenarios disponibles: " + scenarios.Count);
            foreach (var scenario in scenarios)
            {
                foreach(Npcs npc in scenario.GetNpcs())
                {
                    npcs.Add(npc);
                }
                foreach(SceneObject sceneObj in scenario.GetSceneObjects())
                {
                    sceneObjects.Add(sceneObj);
                }
                foreach (Exits exit in scenario.GetExits())
                {
                    exits.Add(exit);
                }
            }
             
           
            // 1. Registrar SceneObjects
            foreach (var sceneObj in sceneObjects)
            {
                if (sceneObj != null)
                {
                    Interactable interactableComponent = sceneObj.GetComponent<Interactable>();
                    Debug.Log("Suscrito correctamente al SceneObject: " + sceneObj.name);
                    interactableComponent.OnClicked += ManejarObjetoClickeado;
                    
                }
            }

            // 2. Registrar NPCs
            // Dentro de RegistrarEventosInteractables, en el bucle de NPCs:
            foreach (var npc in npcs)
            {
                if (npc != null)
                {
                    Interactable interactableComponent = npc.GetComponent<Interactable>();
                    Debug.Log("Suscrito correctamente al NPC: " + npc.name);
                    interactableComponent.OnClicked += ManejarObjetoClickeado;
                    
                }
            }

            // 3. Registrar Exits
            foreach (var exit in exits)
            {
                if (exit != null)
                {
                    Interactable interactableComponent = exit.GetComponent<Interactable>();
                    Debug.Log("Suscrito correctamente al Exit: " + exit.name);
                    interactableComponent.OnClicked += ManejarObjetoClickeado;
                    
                }
                
            }
            Debug.Log("La escenas Tienen " + scenarios.Count + " escenarios, " + npcs.Count + " NPCs, " + sceneObjects.Count + " SceneObjects y " + exits.Count + " Exits. Total elementos: " + (scenarios.Count + npcs.Count + sceneObjects.Count + exits.Count));
            //Debug.Log("total elementos registrados a eventos: "+ interactableComponent.OnClicked.GetInvocationList().Length);
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
