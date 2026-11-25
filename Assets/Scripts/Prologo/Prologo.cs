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
    [SerializeField] private SpriteRenderer cuadradoBlanco;
    [SerializeField] private SpriteRenderer cuadradoAmarillo;
    [SerializeField] private Canvas canvaFondoNegro;
    [SerializeField] private Canvas canvaFondoBlanco;
    [SerializeField] private TextScreen textScreenFondoNegro;
    [SerializeField] private TextScreen textScreenFondoBlanco;
    [SerializeField] private Camera cam;






    void Start()
    {
        canvaMc.gameObject.SetActive(false);
        canvaAb.gameObject.SetActive(false);
        canvaFondoNegro.gameObject.SetActive(false);
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

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 0));
        yield return new WaitForSeconds(3f);//pausa drama
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 4));
        yield return StartCoroutine(IntroducirArbol());//aparece arbol en escena y paramos el scroll
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 5));
        yield return StartCoroutine(AndarMcAlArbol());//mc anda al arbol
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FundidoNegroSprite(cuadradoNegro, 1f, 1.5f));//fundido a negro
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(TextoInteractable(mc, canvaFondoNegro, textScreenFondoNegro, -1, 8));//texto en negro
        yield return new WaitForSeconds(1f);

        ab.gameObject.SetActive(true);//activamos abuelo en posicion

        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 1, 0));//texto fundido negro
        yield return StartCoroutine(FundidoNegroSprite(cuadradoNegro, 0f, 0.3f));//deshacemos fundido a negro

        yield return new WaitForSeconds(1f);//pausa drama

        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 9));//texto mc
        mc.gameObject.GetComponent<SpriteRenderer>().flipX = true;

        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 10));//texto mc
        StartCoroutine(CorrerMcAlAb());//corre al abuelo mientras texto mc
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 11));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 1));//texto abuelo
        yield return StartCoroutine(SepararMcAlAb());//
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 12));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 4));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 13));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 6));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 14));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 8));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 15));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 9));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 16));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 10));//texto abuelo
        //mc tiembla
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 20));
        //fundido a negro
        yield return StartCoroutine(FundidoNegroSprite(cuadradoNegro, 1f, 1.5f));//fundido a negro
        yield return StartCoroutine(TextoInteractable(mc, canvaFondoNegro, textScreenFondoNegro, -1, 25));
        //fundido a blanco
        yield return StartCoroutine(FundidoNegroSprite(cuadradoBlanco, 1f, 1.5f));//fundido a blanco
        yield return StartCoroutine(TextoInteractable(mc, canvaFondoBlanco, textScreenFondoBlanco, -1, 30));
        //Fundido a negro
        yield return StartCoroutine(FundidoNegroSprite(cuadradoBlanco, 0f, 0.5f));//fundido a blanco

        yield return StartCoroutine(TextoInteractable(mc, canvaFondoNegro, textScreenFondoNegro, -1, 34));
        //flash amarillo
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 1f, 0.3f));//flash amarillo
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 0f, 0.3f));//flash amarillo

        yield return StartCoroutine(TextoInteractable(mc, canvaFondoNegro, textScreenFondoNegro, -1, 35));

        //flash amarillo
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 1f, 0.3f));//fundido a amarillo
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 0f, 0.3f));//fundido a amarillo

        yield return StartCoroutine(TextoInteractable(mc, canvaFondoNegro, textScreenFondoNegro, -1, 36));
        //Pantalla en  amarillo
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 1f, 1.5f));//fundido a amarillo

        yield return StartCoroutine(TextoInteractable(mc, canvaFondoNegro, textScreenFondoNegro, -1, 37));
        //Desfundio de amarillo 
        CentrarCamaraEnX(cam, arbol.gameObject.transform);        //se centra en arbol


        yield return StartCoroutine(FundidoNegroSprite(cuadradoNegro, 0f, 0f));//fundido a negro
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 0f, 1.5f));//fundido a amarillo


        //Luz tenue del arbol
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 39));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 11));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 40));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 12));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 41));
        yield return StartCoroutine(TextoInteractable(ab, canvaAb, textScreenAb, 0, 14));//texto abuelo
        yield return StartCoroutine(TextoInteractable(mc, canvaMc, textScreenMc, 0, 42));
        // mc se acerca al arbol
        mc.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        yield return CorrerMcAlArbol();
        //flash into fundido a a marillo
        yield return StartCoroutine(FundidoNegroSprite(cuadradoAmarillo, 1f, 1.5f));//fundido a negro















    }
    IEnumerator CorrerMcAlArbol()
    {
        float velocidad = 4f;
        Vector3 destino = new Vector3(
            arbol.transform.position.x - 0.7f,
            mc.transform.position.y,
            mc.transform.position.z
        );

        while (Mathf.Abs(mc.transform.position.x - destino.x) > 0.01f)
        {
            mc.transform.position = Vector3.MoveTowards(
                mc.transform.position,
                destino,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        mc.transform.position = destino;

    }
    public static void CentrarCamaraEnX(Camera camara, Transform objetivo, float offsetX = 0f)
    {
        if (camara == null || objetivo == null) return;

        Transform camTransform = camara.transform;
        camTransform.position = new Vector3(
            objetivo.position.x + offsetX,
            camTransform.position.y,
            camTransform.position.z
        );
    }
    IEnumerator CorrerMcAlAb()
    {
        float velocidad = 4f;
        Vector3 destino = new Vector3(
            ab.transform.position.x+0.3f,
            mc.transform.position.y,
            mc.transform.position.z
        );

        while (Mathf.Abs(mc.transform.position.x - destino.x) > 0.01f)
        {
            mc.transform.position = Vector3.MoveTowards(
                mc.transform.position,
                destino,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        mc.transform.position = destino;

    }
    IEnumerator SepararMcAlAb()
    {
        float velocidad = 10f;
        Vector3 destino = new Vector3(
            ab.transform.position.x + 1.2f,
            mc.transform.position.y,
            mc.transform.position.z
        );

        while (Mathf.Abs(mc.transform.position.x - destino.x) > 0.01f)
        {
            mc.transform.position = Vector3.MoveTowards(
                mc.transform.position,
                destino,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        mc.transform.position = destino;

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

    IEnumerator AndarMcAlArbol()
    {
        float velocidad = 2f;
        Vector3 destino = new Vector3(
            arbol.transform.position.x,
            mc.transform.position.y,
            mc.transform.position.z
        );

        while (Mathf.Abs(mc.transform.position.x - destino.x) > 0.01f)
        {
            mc.transform.position = Vector3.MoveTowards(
                mc.transform.position,
                destino,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        mc.transform.position = destino;

    }
    private IEnumerator TextoInteractable(Interactable interact, Canvas canva, TextScreen textScreen, int closeUpId, int endTextId)
    {
        Debug.Log("textoSeleccionado: " + interact.GetSelectedText());
        Debug.Log("canva: " + canva.name);
        Debug.Log("textScreen: " + textScreen.name);
        Debug.Log("closeUpId: " + closeUpId);
        Debug.Log("endTextId: " + endTextId);



        bool dialogosTerminados = false;

        // Nos suscribimos temporalmente al evento de que el texto terminó
        textScreen.OnDialogosTerminados += TerminoElTexto;


        canva.gameObject.SetActive(true);
        yield return null;
        if (closeUpId >= 0)
        {
            Debug.Log("Mostrando closeup con id: " + closeUpId);
            textScreen.ShowCloseUp(closeUpId);
        }
        yield return null;

        // Mostramos los diálogos
        textScreen.MostrarDialogos(interact.GetTexts(), interact.GetSelectedText(), endTextId);

        // Pausamos la ejecución de este código hasta que el evento devuelva true
        yield return new WaitUntil(() => dialogosTerminados);

        // Desuscribimos el evento para evitar bugs
        textScreen.OnDialogosTerminados -= TerminoElTexto;

        // --- A PARTIR DE AQUÍ SE EJECUTA CUANDO EL JUGADOR TERMINA DE LEER ---
        Debug.Log("textoSeleccionado: " + interact.GetSelectedText());
        interact.SetSelectedText(endTextId + 1);
        canva.gameObject.SetActive(false);
        textScreen.HideAllCloseUps();




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
