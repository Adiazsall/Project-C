using System.Collections.Generic;
using UnityEngine;

// Una gravació: des d'on es va començar i què es va prémer a cada tic de física.
// El clon començarà amb la mateixa posició i velocitat i rebrà els mateixos frames.
[System.Serializable]
public class Recording
{
    public Vector2 posicioInicial;
    public Vector2 velocitatInicial;
    public List<InputFrame> frames = new List<InputFrame>();

    public float Durada => frames.Count * Time.fixedDeltaTime;

    public Recording() { }

    public Recording(Vector2 posicio, Vector2 velocitat)
    {
        posicioInicial = posicio;
        velocitatInicial = velocitat;
    }
}
