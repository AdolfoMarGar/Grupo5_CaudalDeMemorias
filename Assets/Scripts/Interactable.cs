using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    void Start()
    {
        if (this.id == 0)
            Debug.LogWarning("Interactable has unassigned id on GameObject " + gameObject.name);
        defaultColor = gameObject.GetComponent<SpriteRenderer>().color;

        if (this.active == false)
        {
            // Alternativa estándar: SpriteRenderer
            var sr = gameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.gray;
            else
                Debug.LogWarning("No SpriteManager ni SpriteRenderer en " + gameObject.name);
            
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
        if (active)
        {
            Interact();
        }
    }
}
