using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public interface IInputProvider {
    public InputState GetState();
    void EnableInput();
    void DisableInput();
}

[CreateAssetMenu(fileName = "InputReader", menuName = "InputData/Input Reader")]
public class InputProvider : ScriptableObject, IInputProvider, PlayerInput.IGameplayActions {
    // Gameplay

    private PlayerInput GameInput { get; set; }
    public InputState inputState;
    public event UnityAction<Vector2> MousePosEvent;
    public event UnityAction InteractionCancelledEvent;
    public event UnityAction InteractionStartedEvent;


    private void OnEnable()
    {
        GameInput ??= new PlayerInput();
        GameInput.Gameplay.SetCallbacks(this);
    }

    //private void OnDisable() => DisableInput();

    public void OnMove(InputAction.CallbackContext context)
    {
        inputState.movementDirection = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Started:
                Debug.Log("VAR");
                inputState.interactClicked = true;
                inputState.interactReleased = false;
                inputState.isInteracting = true;
                inputState.holdTimer = 0;
                InteractionStartedEvent?.Invoke();
                break;
            case InputActionPhase.Canceled:
                inputState.interactClicked = false;
                inputState.interactReleased = true;
                inputState.isInteracting = false;
                inputState.holdTimer = 0;
                InteractionCancelledEvent?.Invoke();
                break;
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
            movementDirection = inputState.movementDirection,
            interactClicked = inputState.interactClicked,
            interactReleased = inputState.interactReleased,
            isInteracting = inputState.isInteracting,
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
        Helper.CustomLog("Gameplay Input Disabled", LogColor.White);
    }
}