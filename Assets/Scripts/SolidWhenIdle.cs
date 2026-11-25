using System.Collections.Generic;
using UnityEngine;

// Fa que els jugador i clons es travessin entre ells mentre es mouen.
// Un personatge que porta tempsPerSolid segons al terra sense prémer res es torna sòlid,
// i llavors els altres hi xoquen (s'hi poden enfilar). Dos personatges xoquen si almenys un és sòlid.
// La regla és igual per al jugador i per als clons.
[RequireComponent(typeof(CharacterMotor), typeof(Collider2D))]
public class SolidWhenIdle : MonoBehaviour
{
    public float tempsPerSolid = 2f;
    [Range(0f, 1f)] public float alfaFantasma = 0.5f;

    // Mentre és fals (per exemple, un clon esperant a la plataforma) mai es torna sòlid
    [HideInInspector] public bool potSerSolid = true;

    public bool Solid { get; private set; }

    // Tots els personatges actius, per poder configurar cada parella
    private static readonly List<SolidWhenIdle> tots = new List<SolidWhenIdle>();

    private CharacterMotor motor;
    private Collider2D col;
    private SpriteRenderer sprite;
    private float tempsQuiet;

    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        tots.Add(this);
        motor.FrameAplicat += EnFrameAplicat;
        ActualitzarParelles();
        ActualitzarAspecte();
    }

    private void OnDisable()
    {
        tots.Remove(this);
        motor.FrameAplicat -= EnFrameAplicat;
    }

    // Torna a fantasma i posa el comptador a zero
    public void Reiniciar()
    {
        tempsQuiet = 0f;
        if (Solid)
        {
            CanviarEstat(false);
        }
    }

    // Mirem les entrades i no la velocitat: si algú aterra a sobre i l'empeny una mica,
    // el clon continua quiet i no deixa caure qui hi ha a sobre
    private void EnFrameAplicat(InputFrame input)
    {
        bool quiet = potSerSolid && input.moviment == 0 && !input.salt && motor.EsAlTerra;
        tempsQuiet = quiet ? tempsQuiet + Time.fixedDeltaTime : 0f;

        if (!Solid && tempsQuiet >= tempsPerSolid && !TocaAlgu())
        {
            CanviarEstat(true);
        }
        else if (Solid && !quiet)
        {
            CanviarEstat(false);
        }
    }

    private void CanviarEstat(bool solid)
    {
        Solid = solid;
        ActualitzarParelles();
        ActualitzarAspecte();
    }

    // Si ens tornéssim sòlids a dins d'un altre, la física els separaria de cop
    private bool TocaAlgu()
    {
        foreach (SolidWhenIdle altre in tots)
        {
            if (altre != this && col.Distance(altre.col).isOverlapped)
            {
                return true;
            }
        }
        return false;
    }

    // Per a cada parella on hi sóc jo: xoquen només si algun dels dos és sòlid
    private void ActualitzarParelles()
    {
        foreach (SolidWhenIdle altre in tots)
        {
            if (altre == this) continue;

            bool xoquen = Solid || altre.Solid;
            Physics2D.IgnoreCollision(col, altre.col, !xoquen);
        }
    }

    private void ActualitzarAspecte()
    {
        if (sprite == null) return;

        Color c = sprite.color;
        c.a = Solid ? 1f : alfaFantasma;
        sprite.color = c;
    }
}
