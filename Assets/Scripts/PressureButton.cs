using UnityEngine;

// Botó de terra que premen el jugador i els clons
// Mantenir: premut mentre algú hi és a sobre. Commutar: cada cop que algú el trepitja canvia d'estat.
[DefaultExecutionOrder(-10)] // s'executa abans que les portes, perquè llegeixin l'estat d'aquest mateix tic
[RequireComponent(typeof(Collider2D))]
public class PressureButton : MonoBehaviour, IResettable
{
    public enum Mode { Mantenir, Commutar }

    public Mode mode = Mode.Mantenir;
    public Color colorPremut = Color.green;

    public bool Premut { get; private set; }

    private Collider2D col;
    private SpriteRenderer sprite;
    private Color colorNormal;
    private ContactFilter2D filtre;
    private readonly Collider2D[] resultats = new Collider2D[8];
    private bool trepitjatAbans;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) colorNormal = sprite.color;

        filtre = ContactFilter2D.noFilter;
        filtre.useTriggers = false;
    }

    void FixedUpdate()
    {
        bool trepitjat = AlguASobre();

        if (mode == Mode.Mantenir)
        {
            Premut = trepitjat;
        }
        else if (trepitjat && !trepitjatAbans) // només el tic en què algú el trepitja
        {
            Premut = !Premut;
        }

        trepitjatAbans = trepitjat;
        ActualitzarAspecte();
    }

    private bool AlguASobre()
    {
        int n = col.Overlap(filtre, resultats);
        for (int i = 0; i < n; i++)
        {
            if (resultats[i].GetComponent<CharacterMotor>() != null)
            {
                return true;
            }
        }
        return false;
    }

    public void Reiniciar()
    {
        Premut = false;
        trepitjatAbans = false;
        ActualitzarAspecte();
    }

    private void ActualitzarAspecte()
    {
        if (sprite == null) return;
        sprite.color = Premut ? colorPremut : colorNormal;
    }
}
