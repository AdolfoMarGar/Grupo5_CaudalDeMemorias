using System.Collections.Generic;
using UnityEngine;

public class Scenarios : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private bool active = true;
    [SerializeField] private List<Npcs> npcs = new List<Npcs>();
    [SerializeField] private List<Interactable> interactable = new List<Interactable>();
    [SerializeField] private List<Exits> exits = new List<Exits>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.id == 0)
            Debug.LogWarning("Scenario has unassigned id on GameObject " + gameObject.name);
    }

    public int GetId()
    {
        return id;
    }

    public bool IsActive()
    {
        return active;
    }
    public void SetActive(bool active)
    {
        this.active = active;
    }

    public Npcs GetNpcById(int npcId)
    {
        foreach (var npc in npcs)
        {
            if (npc.GetId() == npcId)
                return npc;
        }
        Debug.LogWarning("Npc with id " + npcId + " not found in Scenario with id " + this.id);
        return null;
    }
    public Interactable GetInteractableById(int interactableId)
    {
        foreach (var interact in interactable)
        {
            if (interact.GetId() == interactableId)
                return interact;
        }
        Debug.LogWarning("Interactable with id " + interactableId + " not found in Scenario with id " + this.id);
        return null;
    }

    public Exits GetExitById(int exitId)
    {
        foreach (var exit in exits)
        {
            if (exit.GetId() == exitId)
                return exit;
        }
        Debug.LogWarning("Scenario with id " + exitId + " not found in Scenario with id " + this.id);
        return null;
    }
}
