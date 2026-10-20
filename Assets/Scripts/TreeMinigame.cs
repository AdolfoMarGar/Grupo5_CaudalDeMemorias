using System.Collections.Generic;
using UnityEngine;

public class TreeMinigame : MonoBehaviour
{
    private int treeCount;
    private int dirtCount;
    private bool glyphsActivated = false;
    [SerializeField] private List<TreeBranch> treeBranch = new List<TreeBranch>();
    [SerializeField] private List<DirtSpots> dirtSpots = new List<DirtSpots>();
    [SerializeField] private List<Glyphs> glyphs = new List<Glyphs>();
    //[SerializeField] protected TextScreen textScreen;
    //[SerializeField] protected Canvas canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        treeCount = treeBranch.Count;
        dirtCount = dirtSpots.Count;

        RegistrarEventosInteractables();
    }

    private void RegistrarEventosInteractables()
    {

        Debug.Log("Registrando eventos de interactuables para minijuegoTree: ");
        if (treeBranch != null && dirtSpots != null && glyphs != null)
        {

           
            Debug.Log("Minijuego arbol tiene " + treeBranch.Count + " NPCs.");
            Debug.Log("Minijuego arbol tiene " + dirtSpots.Count + " DirtSpotss.");
            Debug.Log("Minijuego arbol tiene " + glyphs.Count + " Glyphs.");

            // 1. Registrar DirtSpotss
            foreach (var dirtSpot in dirtSpots)
            {
                if (dirtSpot != null)
                {
                    Interactable interactableComponent = dirtSpot.GetComponent<Interactable>();
                    if (interactableComponent != null)
                    {
                        interactableComponent.OnClicked += ManejarObjetoClickeado;
                    }
                }
            }

            // 2. Registrar NPCs
            // Dentro de RegistrarEventosInteractables, en el bucle de NPCs:
            foreach (var treeBranch in treeBranch)
            {
                if (treeBranch != null)
                {
                    Interactable interactableComponent = treeBranch.GetComponent<Interactable>();
                    if (interactableComponent != null)
                    {
                        Debug.Log("Suscrito correctamente al NPC: " + treeBranch.name); // <--- ¿Aparece este log?
                        interactableComponent.OnClicked += ManejarObjetoClickeado;
                    }
                    else
                    {
                        Debug.LogError("El NPC " + treeBranch.name + " no tiene componente Interactable!", treeBranch);
                    }
                }
            }

            // 3. Registrar Glyphs
            foreach (var glyph in glyphs)
            {
                if (glyph != null)
                {
                    Interactable interactableComponent = glyph.GetComponent<Interactable>();
                    if (interactableComponent != null)
                    {
                        interactableComponent.OnClicked += ManejarObjetoClickeado;
                    }
                }

            }
        }

    }

    public void ManejarObjetoClickeado(Interactable objetoClickeado)
    {
        Debug.Log("Se hizo clic en un objeto interactuable: " + objetoClickeado.name);
        if (objetoClickeado is TreeBranch)
        {
            Debug.Log("Se hizo clic en un TreeBranch: " + objetoClickeado.name);
            treeCount--;
        }
        else if (objetoClickeado is DirtSpots)
        {
            Debug.Log("Se hizo clic en un DirtSpots: " + objetoClickeado.name);
            dirtCount--;
        }
        else if (objetoClickeado is Glyphs)
        {
            Debug.Log("Se hizo clic en un Glyphs: " + objetoClickeado.name);
            // Lógica para manejar el clic en un Glyphs
        }
        Debug.Log("Contadores actuales - TreeCount: " + treeCount + ", DirtCount: " + dirtCount);

    }

    private void OnDestroy()
    {
        foreach (var dirtSpot in dirtSpots)
        {
            if (dirtSpot.gameObject != null)
            {
                Interactable interactableComponent = dirtSpot.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked -= ManejarObjetoClickeado;
                }
            }
        }
        foreach (var treeBranch in treeBranch)
        {
            if (treeBranch.gameObject != null)
            {
                Interactable interactableComponent = treeBranch.GetComponent<Interactable>();
                if (interactableComponent != null)
                {
                    interactableComponent.OnClicked -= ManejarObjetoClickeado;
                }
            }
        }
        foreach (var glyph in glyphs)
        {
            if (glyph.gameObject != null)
            {
                Interactable interactableComponent = glyph.GetComponent<Interactable>();
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

        if(glyphsActivated == false && treeCount <= 0 && dirtCount <= 0)
        {
            Debug.Log("Se han completado todas las tareas del minijuego del árbol. Activando Glyphs.");
            foreach (var glyph in glyphs)
            {
                if (glyph != null)
                {
                    glyph.SetActive(true);
                }
            }
            glyphsActivated = true;
        }
    }

}
