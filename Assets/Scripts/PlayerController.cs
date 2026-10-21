using UnityEngine;
using UnityEngine.InputSystem;

public class TuNombreDeScript : MonoBehaviour
{
    void Start()
    {
   
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.leftArrowKey.isPressed)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-500f * Time.deltaTime, 0));
        }

        if (Keyboard.current != null && Keyboard.current.rightArrowKey.isPressed)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(500f * Time.deltaTime, 0));
        }
    }
}