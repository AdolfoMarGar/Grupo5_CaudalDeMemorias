using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class TextScreen : MonoBehaviour
{
    public TextMeshProUGUI textoUI;

    void Start()
    {
        if (textoUI == null)
        {
            Debug.LogError("¡Falta asignar el componente TextMeshProUGUI en el inspector!", this);
        }
    }

    public void MostrarDialogos(List<string> texts, int selectedText, int endText)
    {
        if (textoUI == null || texts == null || texts.Count == 0) return;
        Debug.Log("Mostrando textos desde el índice " + selectedText + " hasta " + endText);
        StartCoroutine(RutinaMostrarTextos(texts, selectedText, endText));
    }

    IEnumerator RutinaMostrarTextos(List<string> texts, int selectedText, int endText)
    {
        textoUI.enabled = true;
        
        // Necesario porque sino se salta el primer texto.
        while (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            yield return null;
        }


        for (int i = selectedText; i <= endText; i++)
        {
            textoUI.text = texts[i];

            bool clickDetectado = false;

            while (!clickDetectado)
            {
                bool mouseClick = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
                bool touchClick = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

                if (mouseClick || touchClick)
                {
                    clickDetectado = true;
                }

                yield return null;
            }
            yield return null;
        }

        textoUI.enabled = false;
    }
}