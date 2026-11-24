using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Prologo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Tooltip("CanvasMc")]
    [Header("=== Pantalla MC ===")]
    [SerializeField] private Canvas canvaMc;
    [SerializeField] private TextScreen textScreenMc;


    [Tooltip("CanvasAb")]
    [Header("=== Pantalla AB ===")]
    [SerializeField] private Canvas canvaAb;
    [SerializeField] private TextScreen textScreenAb;

    [Header("=== Otros ===")]

    [SerializeField] private Npcs mc;
    [SerializeField] private Npcs ab;
    [SerializeField] private SceneObject arbol;
    [SerializeField] private InfiniteQuadScroll scroll2D;
    [SerializeField] private InfiniteQuadScroll scroll2D2;
    [SerializeField] private SpriteRenderer cuadradoNegro;
    


    void Start()
    {
        canvaMc.gameObject.SetActive(false);
        canvaAb.gameObject.SetActive(false);
        ab.gameObject.SetActive(false);
        arbol.gameObject.SetActive(false);
        StartCoroutine(SecuenciaPrologo());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SecuenciaPrologo()
    {

        // Responde el MC (closeUp 0 en canvaMc, textos desde su índice actual hasta el 1)
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 1, 0));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 1, 4));
        yield return StartCoroutine(IntroducirArbol());
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 1, 5));
        yield return StartCoroutine(FundidoNegroSprite(cuadradoNegro, 1f, 1.5f));

    }

    IEnumerator IntroducirArbol()
    {
        arbol.gameObject.SetActive(true);

        // La velocidad real en unidades del mundo equivale a: scrollSpeed * ancho del Quad
        float anchoEscenario = scroll2D.GetComponent<Renderer>().bounds.size.x;
        float velocidad = scroll2D.scrollSpeed * anchoEscenario;

        float tiempo = 0f;
        while (tiempo < 4.5f)
        {
            arbol.gameObject.transform.Translate(Vector3.left * velocidad * Time.deltaTime, Space.World);
            tiempo += Time.deltaTime;
            yield return null;
        }
        scroll2D.scrollSpeed = 0f;
        scroll2D2.scrollSpeed = 0f;
    }
    private IEnumerator TextoInteractable(Interactable interact, Canvas canva, TextScreen textScreen, int closeUpId, int endTextId)
    {
        Debug.Log("textoSeleccionado: " + interact.GetSelectedText());
        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;


        canva.gameObject.SetActive(true);
        yield return null;

        // Mostramos los diálogos
        textScreen.MostrarDialogos(interact.GetTexts(), interact.GetSelectedText(), endTextId);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        Debug.Log("textoSeleccionado: " + interact.GetSelectedText());
        interact.SetSelectedText(endTextId+1);
        canva.gameObject.SetActive(false);



        void TerminoElTexto()
        {
            dialogosTerminados = true;
        }

    }
    IEnumerator FundidoNegroSprite(SpriteRenderer cuadradoNegro, float alphaObjetivo, float duracion)
    {
        cuadradoNegro.gameObject.SetActive(true);

        Color colorActual = cuadradoNegro.color;
        float alphaInicial = colorActual.a;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaObjetivo, tiempo / duracion);
            cuadradoNegro.color = new Color(colorActual.r, colorActual.g, colorActual.b, nuevoAlpha);
            yield return null;
        }

        cuadradoNegro.color = new Color(colorActual.r, colorActual.g, colorActual.b, alphaObjetivo);

        if (alphaObjetivo <= 0f)
        {
            cuadradoNegro.gameObject.SetActive(false);
        }
    }

}
