using UnityEngine;

// Font d'entrades d'un clon: en lloc del teclat, llegeix una gravació tic a tic.
// Té dos estats: esperant (quiet a la posició inicial) o reproduint.
[RequireComponent(typeof(CharacterMotor), typeof(SolidWhenIdle))]
public class CloneController : MonoBehaviour, IInputSource
{
    private CharacterMotor motor;
    private SolidWhenIdle solidesa;
    private Recording gravacio;
    private int index;
    private bool reproduint;

    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        solidesa = GetComponent<SolidWhenIdle>();
    }

    // La crida la plataforma just després de crear el clon
    public void Configurar(Recording novaGravacio)
    {
        gravacio = novaGravacio;
        Esperar();
    }

    // Torna a la posició inicial i es queda quiet
    public void Esperar()
    {
        reproduint = false;
        solidesa.potSerSolid = false; // esperant a la plataforma: sempre travessable
        solidesa.Reiniciar();
        motor.Teletransportar(gravacio.posicioInicial, Vector2.zero);
    }

    // Comença la gravació des del principi, amb el mateix estat que tenia el jugador
    public void Reproduir()
    {
        index = 0;
        reproduint = true;
        solidesa.potSerSolid = true;
        solidesa.Reiniciar();
        motor.Teletransportar(gravacio.posicioInicial, gravacio.velocitatInicial);
    }

    public InputFrame SeguentFrame()
    {
        // Esperant o gravació acabada: cap tecla premuda
        if (!reproduint || gravacio == null || index >= gravacio.frames.Count)
        {
            return new InputFrame();
        }

        InputFrame frame = gravacio.frames[index];
        index++;
        return frame;
    }
}
