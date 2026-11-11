using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public event Action<Interactable> OnPlayerInteractRequest;
    public bool puedeInteractuar = true;

    private List<Interactable> interactablesEnRango = new List<Interactable>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (interactablesEnRango.Count > 0)
            {
                interactablesEnRango.RemoveAll(item => item == null);

                if (interactablesEnRango.Count > 0)
                {
                    Interactable objetivo = interactablesEnRango[interactablesEnRango.Count - 1];

                    if (objetivo.IsActive())
                    {
                        OnPlayerInteractRequest?.Invoke(objetivo);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable) && !interactablesEnRango.Contains(interactable))
        {
            interactablesEnRango.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable) && interactablesEnRango.Contains(interactable))
        {
            interactablesEnRango.Remove(interactable);
        }
    }
}
