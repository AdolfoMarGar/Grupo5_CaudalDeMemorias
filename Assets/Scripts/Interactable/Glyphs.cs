using System.Collections;
using UnityEngine;

public class Glyphs : TreeInteractable
{
    private bool glowing = false; // Variable para controlar si el glifo está brillando
    public override void Interact()
    {
        base.Interact(); // Opcional: ejecuta primero lo que tuviera la base


    }

    public void GlowGlyph()
    {
        var sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            // Alternamos el estado de glowing cada vez que se llama
            glowing = !glowing;

            if (glowing)
            {
                sr.color = Color.yellow; // Se enciende (amarillo)
            }
            else
            {
                sr.color = defaultColor; // Se apaga (color original)
            }
        }
    }
    public void FlashRed(float duracion = 0.3f)
    {
        StartCoroutine(RutinaParpadeoRojo(duracion));
    }

    private IEnumerator RutinaParpadeoRojo(float duracion)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            Color colorPrevio = sr.color; // Guardamos el color que tenía justo antes del fallo
            sr.color = Color.red;

            yield return new WaitForSeconds(duracion);

            sr.color = colorPrevio; // Restauramos su color original
        }
    }
    public bool IsGlowing()
    {
        return glowing;
    }
    public bool EnableBoxCollider2D(bool enable)
    {
        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.enabled = enable;
            return true; // Retorna true si se pudo habilitar/deshabilitar el collider
        }
        return false; // Retorna false si no se encontró el collider
    }
}