using UnityEngine;

public class DirtSpots : Interactable
{
    public override void Interact()
    {
        base.Interact(); // Opcional: ejecuta primero lo que tuviera la base

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        var col = GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;
    }
}