using UnityEngine;

// Mou un personatge a partir d'InputFrames.
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CharacterMotor : MonoBehaviour
{
    public float forcaMoviment = 10f;
    public float alcadaSalt = 3f; // en cel·les del tilemap (1 cel·la = 1 unitat)
    public float velocitatMaxima = 7f;

    [Header("Detecció de terra")]
    public float distanciaTerra = 0.05f;

    public bool EsAlTerra { get; private set; }

    // S'avisa cada tic amb l'InputFrame que s'ha aplicat (el Recorder l'escolta per gravar)
    public event System.Action<InputFrame> FrameAplicat;

    private Rigidbody2D rb;
    private Collider2D col;
    private IInputSource font;
    private ContactFilter2D filtreTerra;
    private readonly Collider2D[] resultatsTerra = new Collider2D[8];

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        font = GetComponent<IInputSource>();

        // Tots els colliders menys els triggers (zones, plataformes de clon...)
        filtreTerra = ContactFilter2D.noFilter;
        filtreTerra.useTriggers = false;
    }

    void FixedUpdate()
    {
        if (font == null) return;

        InputFrame input = font.SeguentFrame();
        Aplicar(input);
        FrameAplicat?.Invoke(input);
    }

    // Col·loca el personatge en un estat concret (per reiniciar o per començar una reproducció)
    public void Teletransportar(Vector2 posicio, Vector2 velocitat)
    {
        rb.position = posicio;
        // També el transform, perquè amb Interpolate no es vegi lliscar des de la posició antiga
        transform.position = new Vector3(posicio.x, posicio.y, transform.position.z);
        rb.linearVelocity = velocitat;
        rb.angularVelocity = 0f;
    }

    private void Aplicar(InputFrame input)
    {
        EsAlTerra = ComprovarTerra();

        //MOVIMENT LATERAL
        if (input.moviment != 0)
        {
            rb.AddForce(new Vector2(input.moviment * forcaMoviment, 0));
        }

        if (Mathf.Abs(rb.linearVelocity.x) > velocitatMaxima)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * velocitatMaxima, rb.linearVelocity.y);
        }

        //SALT
        if (input.salt && EsAlTerra)
        {
            // v = √(2·g·h): la velocitat inicial que puja exactament alcadaSalt amb aquesta gravetat.
            float gravetat = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;
            float velocitatSalt = Mathf.Sqrt(2f * gravetat * alcadaSalt);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocitatSalt);
        }
    }

    // Mira si hi ha terra just a sota dels peus amb una caixa prima.
    private bool ComprovarTerra()
    {
        Bounds b = col.bounds;
        Vector2 centre = new Vector2(b.center.x, b.min.y - distanciaTerra / 2f);
        Vector2 mida = new Vector2(b.size.x * 0.9f, distanciaTerra);

        int n = Physics2D.OverlapBox(centre, mida, 0f, filtreTerra, resultatsTerra);
        for (int i = 0; i < n; i++)
        {
            Collider2D c = resultatsTerra[i];

            // Fantasma
            if (c == col || Physics2D.GetIgnoreCollision(col, c)) continue;

            // Terra o un altre personatge (per enfilar-se a sobre d'un clon)
            if (c.CompareTag("Terra") || c.GetComponent<CharacterMotor>() != null)
            {
                return true;
            }
        }
        return false;
    }

    // Dibuixa la caixa de terra a l'escena quan el personatge està seleccionat
    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;
        Gizmos.color = EsAlTerra ? Color.green : Color.red;
        Gizmos.DrawWireCube(
            new Vector3(b.center.x, b.min.y - distanciaTerra / 2f, 0),
            new Vector3(b.size.x * 0.9f, distanciaTerra, 0));
    }
}
