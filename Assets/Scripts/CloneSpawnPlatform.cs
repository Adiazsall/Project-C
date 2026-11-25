using UnityEngine;

// Plataforma on el jugador pot gravar un clon. Una gravació per plataforma i clon.
public class CloneSpawnPlatform : MonoBehaviour
{
    public CloneController prefabClon;

    // SerializeField perquè es vegi a l'Inspector mentre jugues (frames gravats, posició inicial...)
    [SerializeField] private Recording gravacio;
    private CloneController clon;

    public Recording Gravacio => gravacio;
    public CloneController Clon => clon;

    public void GuardarGravacio(Recording novaGravacio)
    {
        gravacio = novaGravacio;

        if (clon == null)
        {
            clon = Instantiate(prefabClon, gravacio.posicioInicial, Quaternion.identity);
            clon.name = "Clon " + name;
        }
        clon.Configurar(gravacio);
    }

    // Quan es torna a gravar aquí, el clon antic desapareix: un sol clon per plataforma
    public void EliminarClon()
    {
        if (clon != null)
        {
            Destroy(clon.gameObject);
            clon = null;
        }
        gravacio = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Recorder recorder = other.GetComponent<Recorder>();
        if (recorder != null)
        {
            recorder.plataformaActual = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Recorder recorder = other.GetComponent<Recorder>();
        if (recorder != null && recorder.plataformaActual == this)
        {
            recorder.plataformaActual = null;
        }
    }
}
