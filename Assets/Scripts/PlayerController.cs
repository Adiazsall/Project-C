using UnityEngine;
using UnityEngine.InputSystem;

// Font d'entrades del jugador: llegeix el teclat i les passa al CharacterMotor.
[RequireComponent(typeof(CharacterMotor), typeof(Recorder))]
public class PlayerController : MonoBehaviour, IInputSource
{
    [Header("Inputs configurables")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction recordAction;

    private Recorder recorder;

    // El salt es prem en un frame (Update) però es fa servir al següent tic de física (FixedUpdate).
    // Ho guardem aquí perquè no es perdi si aquell frame no hi ha hagut cap FixedUpdate.
    private bool saltPendent;

    void Awake()
    {
        recorder = GetComponent<Recorder>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        recordAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        recordAction.Disable();
    }

    void Start()
    {
        recordAction.AddBinding("<Keyboard>/r");

        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");

        jumpAction.AddBinding("<Keyboard>/upArrow");
    }

    void Update()
    {
        if (jumpAction.triggered)
        {
            saltPendent = true;
        }

        if (recordAction.triggered)
        {
            recorder.Alternar();
        }
    }

    public InputFrame SeguentFrame()
    {
        InputFrame frame = new InputFrame
        {
            moviment = moveAction.ReadValue<float>(),
            salt = saltPendent
        };
        saltPendent = false;
        return frame;
    }

    //Reconfiguració de controls
    public void CanviarTeclaSalt()
    {
        jumpAction.Disable();
        jumpAction.PerformInteractiveRebinding()
            .OnComplete(operation => {
                operation.Dispose();
                jumpAction.Enable();
                Debug.Log("Control asignat");
            })
            .Start();
    }
}
