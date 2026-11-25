// Qualsevol cosa que pugui donar entrades a un CharacterMotor:
// el teclat (PlayerController) o, més endavant, una gravació (el clon).
public interface IInputSource
{
    // El CharacterMotor el crida un cop per tic de física.
    InputFrame SeguentFrame();
}
