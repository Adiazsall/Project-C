using System.Linq;
using UnityEngine;

// Coordina el cicle de gravació del nivell:
//  - En començar a gravar: reinicia el món i posa a reproduir tots els altres clons.
//  - En acabar: desa el clon, reinicia el món, torna els clons a mode espera i el jugador a l'inici.
public class LevelManager : MonoBehaviour
{
    [SerializeField] private Recorder jugador;

    private CharacterMotor motorJugador;
    private SolidWhenIdle solidesaJugador;
    private Vector2 posicioInicialJugador;
    private CloneSpawnPlatform[] plataformes;
    private IResettable[] reiniciables;

    private void OnEnable()
    {
        jugador.GravacioIniciada += EnIniciarGravacio;
        jugador.GravacioAcabada += EnAcabarGravacio;
    }

    private void OnDisable()
    {
        jugador.GravacioIniciada -= EnIniciarGravacio;
        jugador.GravacioAcabada -= EnAcabarGravacio;
    }

    void Start()
    {
        motorJugador = jugador.GetComponent<CharacterMotor>();
        solidesaJugador = jugador.GetComponent<SolidWhenIdle>();
        posicioInicialJugador = jugador.transform.position;

        plataformes = FindObjectsByType<CloneSpawnPlatform>();

        // Busquem tots els scripts de l'escena i ens quedem els que implementen IResettable
        reiniciables = FindObjectsByType<MonoBehaviour>()
            .OfType<IResettable>()
            .ToArray();
    }

    private void EnIniciarGravacio(CloneSpawnPlatform plataforma)
    {
        // Reinicia el mon
        ReiniciarMon();

        // Esborra el clon per sobreescriurel
        plataforma.EliminarClon();

        foreach (CloneSpawnPlatform p in plataformes)
        {
            if (p.Clon != null)
            {
                p.Clon.Reproduir();
            }
        }
    }

    private void EnAcabarGravacio(CloneSpawnPlatform plataforma, Recording gravacio)
    {
        plataforma.GuardarGravacio(gravacio);

        ReiniciarMon();

        foreach (CloneSpawnPlatform p in plataformes)
        {
            if (p.Clon != null)
            {
                p.Clon.Esperar();
            }
        }

        motorJugador.Teletransportar(posicioInicialJugador, Vector2.zero);
        solidesaJugador.Reiniciar();
    }

    private void ReiniciarMon()
    {
        foreach (IResettable r in reiniciables)
        {
            r.Reiniciar();
        }
    }
}
