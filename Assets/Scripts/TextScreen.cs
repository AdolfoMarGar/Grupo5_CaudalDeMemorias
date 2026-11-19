using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class TextScreen : MonoBehaviour
{
    public TextMeshProUGUI textoUI;
    [SerializeField] private Image imageText;
    public event Action OnDialogosTerminados;

    private Coroutine rutinaActual;
    private bool estaMostrando = false;

    void Start()
    {
        if (textoUI == null)
        {
            Debug.LogError("¡Falta asignar el componente TextMeshProUGUI en el inspector!", this);
        }
        imageText.enabled = false;
        textoUI.enabled = false;
    }

    public void MostrarDialogos(List<string> texts, int selectedText, int endText)
    {
        if (estaMostrando) return;

        if (textoUI == null || texts == null || texts.Count == 0) return;

        if (selectedText < 0 || endText >= texts.Count || selectedText > endText)
        {
            Debug.LogWarning($"Índices inválidos en MostrarDialogos: selectedText={selectedText}, endText={endText}, total={texts.Count}");
            return;
        }

        estaMostrando = true;
        textoUI.text = texts[selectedText];
        textoUI.enabled = true;
        imageText.enabled = true;

        rutinaActual = StartCoroutine(RutinaMostrarTextos(texts, selectedText, endText));
    }

    IEnumerator RutinaMostrarTextos(List<string> texts, int selectedText, int endText)
    {
       
        yield return null;
        yield return EsperarAQueSuelteClick();

        for (int i = selectedText; i <= endText; i++)
        {
            textoUI.text = texts[i];

            yield return null;

            yield return EsperarAQuePresioneClick();
            yield return EsperarAQueSuelteClick();
        }

        textoUI.text = "";
        textoUI.enabled = false;
        imageText.enabled = false;
        estaMostrando = false;

        OnDialogosTerminados?.Invoke();
    }


    private IEnumerator EsperarAQuePresioneClick()
    {
        while (!SeHizoClick())
        {
            yield return null;
        }
    }

    private IEnumerator EsperarAQueSuelteClick()
    {
        while (SeMantienePresionado())
        {
            yield return null;
        }
    }

    private bool SeHizoClick()
    {
        bool mouseClick = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchClick = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        bool spaceClick = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        return mouseClick || touchClick || spaceClick;
    }

    private bool SeMantienePresionado()
    {
        bool mouseHold = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool touchHold = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        bool spaceHold = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        return mouseHold || touchHold || spaceHold;
    }
}