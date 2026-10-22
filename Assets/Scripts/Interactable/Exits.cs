using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exits : Interactable
{
    [Header("Cambio de Escena")]
    [Tooltip("Escena a la que lleva esta salida")]
    public string targetScene = "GrandpaSide2D";

    public override void Interact()
    {
        base.Interact();

        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"[Exits] Salida activada. Cargando escena: {targetScene}");
            SceneManager.LoadScene(targetScene);
            return;
        }

        if (texts != null && GetSelectedText() < texts.Count && texts[GetSelectedText()] != null)
        {
            Debug.Log("Salida de id " + GetId() + " dice: " + texts[GetSelectedText()]);
        }
    }
}
