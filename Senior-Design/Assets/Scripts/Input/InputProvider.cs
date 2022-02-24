using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public struct InputState {
    public Vector2 movementDirection;
}

public interface IInputProvider {
    public InputState GetState();
    void EnableInput();
    void DisableInput();
}

[CreateAssetMenu(fileName = "InputReader", menuName = "InputData/Input Reader")]
public class InputProvider : ScriptableObject, IInputProvider, PlayerInput.IGameplayActions {
    // Gameplay

    private Vector2 movementDirection;
    public event UnityAction<Vector2> MousePosEvent;
    public event UnityAction InteractionCancelledEvent;
    public event UnityAction InteractionStartedEvent;

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
        if (InteractionStartedEvent != null && context.phase == InputActionPhase.Started)
            InteractionStartedEvent.Invoke();

        if (InteractionCancelledEvent != null && context.phase == InputActionPhase.Canceled)
            InteractionCancelledEvent.Invoke();
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
        Helper.CustomLog("Gameplay Input Enabled", LogColor.White);
    }
    public void DisableInput()
    {
        GameInput.Gameplay.Disable();
    }
}