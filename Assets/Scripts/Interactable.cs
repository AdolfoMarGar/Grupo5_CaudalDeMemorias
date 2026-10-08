using System;
using System.Collections.Generic;
using UnityEngine;
public class Interactable : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private bool active = true;
    [SerializeField] protected List<string> texts = new List<string>();
    [SerializeField] private int selectedText = 0;
    private Color defaultColor;

    public event Action<Interactable> OnClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() // Cambiado de Start a Awake
    {
        if (this.id == 0)
            Debug.LogWarning("Interactable has unassigned id on GameObject " + gameObject.name);

        var sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
            defaultColor = sr.color;
        else
            Debug.LogWarning("No SpriteRenderer en " + gameObject.name);

        if (this.active == false && sr != null)
        {
            sr.color = Color.gray;
        }
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
        var sr = gameObject.GetComponent<SpriteRenderer>();
        if (!active)
        {
            if (sr != null)
                sr.color = Color.gray;
        }
        else
        {
            sr.color = defaultColor;
        }
    }

    public int GetSelectedText()
    {
        return selectedText;
    }

    public void SetSelectedText(int index)
    {
        if (texts == null)
        {
            Debug.LogError("Texts array is null in Interactable with id " + this.id);
            return;
        }

        if (index >= texts.Count || index < 0)
        {
            Debug.LogError("Index out of bounds in Interactable with id " + this.id);
            return;
        }

        if (texts[index] == null)
            Debug.LogError("Text is null in Interactable with id " + this.id);

        this.selectedText = index;
    }
    public List<string> GetTexts()
    {
        return texts;
    }

    public virtual void Interact()
    {
        //Debug.Log("Interactuando con objeto genérico ID : " + id);
        OnClicked?.Invoke(this);
    }

    private void OnMouseDown()
    {
        Debug.Log("¡Se ha detectado un clic físico en el objeto: " + gameObject.name + "!");

        if (active)
        {
            Interact();
        }
        else
        {
            Debug.Log("El objeto está inactivo (active = false), por eso no interactúa.");
        }
    }
}
