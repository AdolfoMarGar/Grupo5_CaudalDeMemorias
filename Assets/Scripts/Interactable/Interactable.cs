using System;
using System.Collections.Generic;
using UnityEngine;
public class Interactable : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private bool active = true;
    [SerializeField] protected List<string> texts = new List<string>();
    [SerializeField] private int selectedText = 0;
    protected Color defaultColor;
    private Collider2D col2D;
    protected bool inRange = false;
    public event Action<Interactable> OnClicked;

    void Awake() 
    {
        
        col2D = GetComponent<Collider2D>();
        if (col2D == null)
            Debug.LogWarning("No Collider2D en " + gameObject.name);
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
        if (active == false)
        {
            col2D.enabled = false;
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
        if(sr == null)
        {
            sr= gameObject.GetComponentInChildren<SpriteRenderer>();
        }

        if (!active)
        {
            if (sr != null)
                sr.enabled = false;

            if (col2D != null)
                col2D.enabled = false;
        }
        else
        {
            if (sr != null)
                sr.enabled = true;

            if (col2D != null)
                col2D.enabled = true;
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
        // si estaba administrando desde un script externo, como puedo seguir haciendolo desde ahi? te paso un ejemplo.
    }

}
