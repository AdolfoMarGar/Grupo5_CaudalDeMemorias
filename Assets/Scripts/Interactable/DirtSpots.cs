using UnityEngine;

public class DirtSpots : Interactable
{
    public override void Interact()
    {
        base.Interact(); // Opcional: ejecuta primero lo que tuviera la base

        //se desactiva el sprite y el collider del objeto para que no se pueda interactuar con él
        //pero no el objeto completo ya que necesitamos que siga existiendo para poder contar cuántos quedan en la escena
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        var col = GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;
    }
}