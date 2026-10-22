using UnityEngine;
using System.Collections.Generic;


public class Npcs : Interactable
{
    public override void Interact()
    {
        base.Interact(); // Opcional: ejecuta primero lo que tuviera la base

        if (texts[GetSelectedText()] == null)
        {
            Debug.LogError("Texto nulo en NPC con id " + GetId());
            return;
        }

        Debug.Log("Npc de id"+ GetId()+ " dice: " + texts[GetSelectedText()]);

        // Aquí podrías abrir la interfaz gráfica (UI) de diálogos de tu juego.
    }
}
