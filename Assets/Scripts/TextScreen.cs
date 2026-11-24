using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class CloseUpPair
{
    public string groupName;
    public Image partUp;
    public Image partDown; // Déjalo en None si este close-up es de 1 sola parte
    public TextMeshProUGUI closeUpText; // Déjalo en None si este close-up no lleva texto
}

public class TextScreen : MonoBehaviour
{
    public TextMeshProUGUI textoUI;
    [SerializeField] private Image imageText;
    [SerializeField] private List<CloseUpPair> closeUps = new List<CloseUpPair>();

    public event Action OnDialogosTerminados;

    private Coroutine rutinaActual;
    private bool estaMostrando = false;

    void Start()
    {
        if (textoUI == null)
        {
            Debug.LogError("¡Falta asignar el componente TextMeshProUGUI en el inspector!", this);
        }

        if (imageText != null)
        {
            imageText.enabled = false;
        }

        if (textoUI != null)
        {
            textoUI.enabled = false;
        }

        HideAllCloseUps();
    }

    public void ShowCloseUp(int index)
    {
        HideAllCloseUps();

        if (closeUps == null || index < 0 || index >= closeUps.Count)
        {
            Debug.LogWarning($"Índice de CloseUp fuera de rango o lista vacía: {index}", this);
            return;
        }

        CloseUpPair pair = closeUps[index];
        if (pair == null) return;

        if (pair.partUp != null) pair.partUp.enabled = true;
        if (pair.partDown != null) pair.partDown.enabled = true;
        if (pair.closeUpText != null) pair.closeUpText.enabled = true;
    }

    public void HideAllCloseUps()
    {
        if (closeUps == null) return;

        foreach (CloseUpPair pair in closeUps)
        {
            if (pair == null) continue;
            if (pair.partUp != null) pair.partUp.enabled = false;
            if (pair.partDown != null) pair.partDown.enabled = false;
            if (pair.closeUpText != null) pair.closeUpText.enabled = false;
        }
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

        if (imageText != null)
        {
            imageText.enabled = true;
        }

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

        if (imageText != null)
        {
            imageText.enabled = false;
        }

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