using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public struct InputState {
    public Vector2 movementDirection;
}

public interface IInputProvider {
    public event UnityAction<float> onInteract; 
    public InputState GetState();
    void EnableInput();
    void DisableInput();
}

[CreateAssetMenu(fileName = "InputReader", menuName = "InputData/Input Reader")]
public class InputProvider : ScriptableObject, IInputProvider, PlayerInput.IGameplayActions {
    // Gameplay

    public Vector2 movementDirection;
    public event UnityAction<float> onInteract;
    public event UnityAction<Vector2> MousePosEvent;

    private PlayerInput GameInput { get; set; }

    private void OnEnable()
    {
        GameInput ??= new PlayerInput();
        GameInput.Gameplay.SetCallbacks(this);
        EnableInput();
    }

    private void OnDisable() => DisableInput();

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) {
            onInteract?.Invoke(context.ReadValue<float>());
        }
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        MousePosEvent?.Invoke(context.ReadValue<Vector2>());
    }
    
    public static implicit operator InputState(InputProvider provider) => provider.GetState();
    
    public InputState GetState()
    {
        return new InputState {
            movementDirection = movementDirection,
        };
    }

    public void EnableInput()
    {
        GameInput.Gameplay.Enable();
    }

    public void DisableInput()
    {
        GameInput.Gameplay.Disable();
    }
}