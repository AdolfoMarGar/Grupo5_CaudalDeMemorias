using System.Collections.Generic;
using UnityEngine;

public class Exits : Interactable
{
    public override void Interact()
    {
        base.Interact(); // Opcional: ejecuta primero lo que tuviera la base

        if (texts[GetSelectedText()] == null)
        {
            Debug.LogError("Texto nulo en Salida con id " + GetId());
            return;
        }

        Debug.Log("Salida de id" + GetId() + " dice: " + texts[GetSelectedText()]);

        // Aquí podrías abrir la interfaz gráfica (UI) de diálogos de tu juego.
    }
}
