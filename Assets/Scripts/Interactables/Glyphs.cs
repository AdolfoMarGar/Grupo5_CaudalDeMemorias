using UnityEngine;

public class Glyphs : Interactable
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
    public bool IsGlowing()
    {
        return glowing;
    }
}
