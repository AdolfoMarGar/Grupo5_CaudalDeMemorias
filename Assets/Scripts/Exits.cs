using System.Collections.Generic;
using UnityEngine;

public class Exits : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private bool active = true;
    [SerializeField] private List<string> texts = new List<string>();
    [SerializeField] private int selectedText = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.id == 0)
            Debug.LogWarning("Exits has unassigned id on GameObject " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {

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
    public int GetSelectedText()
    {
        return selectedText;
    }
    public void SetSelectedText(int index)
    {
        if (texts == null)
        {
            Debug.LogError("Texts array is null in Exits with id " + this.id);
            return;
        }

        if (index >= texts.Count || index < 0)
        {
            Debug.LogError("Index out of bounds in Exits with id " + this.id);
            return;
        }

        if (texts[index] == null)
            Debug.LogError("Text is null in Exits with id " + this.id);

        this.selectedText = index;
    }

    public void PrintText()
    {
        if (texts[selectedText] != null)
        {
            Debug.Log("Exit dice (" + id + "): " + texts[selectedText]);
        }
        else
        {
            Debug.LogWarning("Exits with id " + this.id + " has no texts.");
        }
    }
}
