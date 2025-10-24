using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputActions inputActions;
    private static InputManager instance;
    
    

    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        inputActions = new InputActions();
    }
    
    public Vector2 GetPlayerMovement()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return inputActions.Player.Look.ReadValue<Vector2>();
    }

    public bool GetSprint()
    {
        return inputActions.Player.Sprint.IsPressed();
    }


    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
