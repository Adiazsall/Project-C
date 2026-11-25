// Qualsevol objecte del nivell que s'ha de tornar al seu estat inicial
// (plataformes mòbils, portes, botons...). Cada objecte guarda el seu propi
// estat inicial (per exemple a Awake) i el recupera quan se li crida Reiniciar.
public interface IResettable
{
    void Reiniciar();
}
