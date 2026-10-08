using UnityEngine;

public class SceneObject : Interactable
{
    public override void Interact()
    {
        base.Interact(); // Opcional: ejecuta primero lo que tuviera la base

        if (texts[GetSelectedText()] == null)
        {
            Debug.LogError("Texto nulo en objeto de escenario con id " + GetId());
            return;
        }

        Debug.Log("Objeto de escenario de id" + GetId() + " dice: " + texts[GetSelectedText()]);

        // Aquí podrías abrir la interfaz gráfica (UI) de diálogos de tu juego.
    }
}
