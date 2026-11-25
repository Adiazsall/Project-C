using UnityEngine;

// Grava els InputFrames del jugador (màxim duradaMaxima segons).
[RequireComponent(typeof(CharacterMotor))]
public class Recorder : MonoBehaviour
{
    public float duradaMaxima = 60f;

    // La plataforma que el jugador té a sota ara mateix (la posa i la treu la plataforma)
    [HideInInspector] public CloneSpawnPlatform plataformaActual;

    public bool Gravant => gravacioActual != null;
    public float TempsRestant => Gravant ? duradaMaxima - gravacioActual.Durada : 0f;

    public event System.Action<CloneSpawnPlatform> GravacioIniciada;
    public event System.Action<CloneSpawnPlatform, Recording> GravacioAcabada;

    private CharacterMotor motor;
    private Rigidbody2D rb;
    private Recording gravacioActual;
    private CloneSpawnPlatform plataformaOrigen;
    private int maxFrames;

    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Ens subscrivim a l'event del motor: cada cop que aplica un frame, ens el passa
    private void OnEnable()
    {
        motor.FrameAplicat += RegistrarFrame;
    }

    private void OnDisable()
    {
        motor.FrameAplicat -= RegistrarFrame;
    }

    // La crida el PlayerController quan es prem la tecla de gravar
    public void Alternar()
    {
        if (Gravant)
        {
            Aturar();
        }
        else
        {
            Iniciar();
        }
    }

    private void Iniciar()
    {
        if (plataformaActual == null)
        {
            Debug.Log("Has d'estar sobre una plataforma de clon per gravar");
            return;
        }

        plataformaOrigen = plataformaActual;
        gravacioActual = new Recording(rb.position, rb.linearVelocity);
        maxFrames = Mathf.RoundToInt(duradaMaxima / Time.fixedDeltaTime);

        Debug.Log("Gravant a " + plataformaOrigen.name + "...");
        GravacioIniciada?.Invoke(plataformaOrigen);
    }

    private void Aturar()
    {
        Recording gravacio = gravacioActual;
        CloneSpawnPlatform plataforma = plataformaOrigen;

        gravacioActual = null;
        plataformaOrigen = null;

        Debug.Log("Gravació acabada a " + plataforma.name + ": " + gravacio.Durada.ToString("F1") + " s");
        GravacioAcabada?.Invoke(plataforma, gravacio);
    }

    private void RegistrarFrame(InputFrame frame)
    {
        if (!Gravant) return;

        gravacioActual.frames.Add(frame);

        if (gravacioActual.frames.Count >= maxFrames)
        {
            Aturar();
        }
    }
}
