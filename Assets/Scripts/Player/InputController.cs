using UnityEngine;

public class InputController
{
    public static Vector2 MoveVector => new Vector2(
        Input.GetAxis("Horizontal"),
        Input.GetAxis("Vertical")
     );
    public static Vector2 MouseVector => new Vector2(
        Input.GetAxis("Mouse X"),
        Input.GetAxis("Mouse Y")
    );
    public static bool Fire1Down => Input.GetButtonDown("Fire1");
    public static bool Fire2Down => Input.GetButtonDown("Fire2");
    public static bool RedKeyDown => Input.GetKeyDown(KeyCode.R);
    public static bool GreenKeyDown => Input.GetKeyDown(KeyCode.G);
    public static bool BlueKeyDown => Input.GetKeyDown(KeyCode.B);
    public static bool Jump => Input.GetButtonDown("Jump");
}