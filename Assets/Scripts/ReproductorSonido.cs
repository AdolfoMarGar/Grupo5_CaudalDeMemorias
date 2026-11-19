using UnityEngine;

// Esto obliga a que el objeto tenga un componente AudioSource automáticamente
[RequireComponent(typeof(AudioSource))]
public class ReproductorSonido : MonoBehaviour
{
    [Header("Configuración del Audio")]
    [Tooltip("Arrastra aquí el archivo de sonido que quieres reproducir")]
    public AudioClip sonidoParaReproducir;

    [Range(0f, 1f)]
    public float volumen = 1.0f;

    private AudioSource audioSource;

    void Awake()
    {
        // Obtenemos la referencia al componente AudioSource de este mismo objeto
        audioSource = GetComponent<AudioSource>();

        // Nos aseguramos de que no suene solo al iniciar el juego ni se repita en bucle
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    /// <summary>
    /// Llama a esta función desde otro script para reproducir el sonido una sola vez.
    /// </summary>
    public void ReproducirUnaVez()
    {
        if (sonidoParaReproducir != null)
        {
            audioSource.PlayOneShot(sonidoParaReproducir, volumen);
        }
        else
        {
            Debug.LogWarning("Falta asignar el AudioClip en el script ReproductorSonido en el objeto: " + gameObject.name);
        }
    }
}