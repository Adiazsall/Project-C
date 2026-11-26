using UnityEngine;

// Porta que llisca entre la posició tancada i oberta
// S'obre segons l'estat dels botons que té assignats.
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SlidingDoor : MonoBehaviour, IResettable
{
    public PressureButton[] botons;
    // Fals: n'hi ha prou amb un botó premut. Cert: calen tots alhora
    public bool calenTots = false;

    public Vector2 desplacament = new Vector2(0f, 3f); // en cel·les
    public float velocitat = 4f;                       // cel·les per segon

    public bool Oberta { get; private set; }

    private Rigidbody2D rb;
    private Vector2 posicioTancada;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Kinematic: només la movem nosaltres. Empeny els personatges, però ells no la poden moure
        rb.bodyType = RigidbodyType2D.Kinematic;
        posicioTancada = rb.position;
    }

    void FixedUpdate()
    {
        Oberta = BotonsActius();

        Vector2 objectiu = Oberta ? posicioTancada + desplacament : posicioTancada;
        // MovePosition i no transform.position: així la física sap que es mou i empeny el que toca
        rb.MovePosition(Vector2.MoveTowards(rb.position, objectiu, velocitat * Time.fixedDeltaTime));
    }

    private bool BotonsActius()
    {
        if (botons == null || botons.Length == 0) return false;

        foreach (PressureButton boto in botons)
        {
            if (boto == null) continue;

            if (calenTots && !boto.Premut) return false; // en falta un
            if (!calenTots && boto.Premut) return true;  // amb un n'hi ha prou
        }
        return calenTots;
    }

    public void Reiniciar()
    {
        Oberta = false;
        rb.position = posicioTancada;
        transform.position = new Vector3(posicioTancada.x, posicioTancada.y, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 tancada = Application.isPlaying ? (Vector3)posicioTancada : transform.position;
        Vector3 oberta = tancada + (Vector3)desplacament;

        Collider2D c = GetComponent<Collider2D>();
        Vector3 mida = c != null ? c.bounds.size : Vector3.one;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(tancada, oberta);
        Gizmos.DrawWireCube(oberta, mida);

        if (botons == null) return;

        Gizmos.color = Color.cyan;
        foreach (PressureButton boto in botons)
        {
            if (boto != null)
            {
                Gizmos.DrawLine(transform.position, boto.transform.position);
            }
        }
    }
}
