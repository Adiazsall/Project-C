// Les entrades d'un sol tic de física (un FixedUpdate).
// És el que el jugador genera i el que un clon reproduirà.
[System.Serializable]
public struct InputFrame
{
    public float moviment; // -1 esquerra, 0 quiet, 1 dreta
    public bool salt;      // true només el tic en què s'ha premut el salt
}
