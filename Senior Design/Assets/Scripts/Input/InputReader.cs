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
public class InputReader : ScriptableObject, IInputProvider, GameInput.IGameplayActions {
    // Gameplay
    public event UnityAction<Vector2> MoveEvent;
    public event UnityAction<Vector2> MousePosEvent;
    public event UnityAction InteractionStartedEvent;
    public event UnityAction InteractionCancelledEvent;


    private GameInput GameInput { get; set; }

    private void OnEnable()
    {
        if (GameInput == null) {
            GameInput = new GameInput();
            GameInput.Gameplay.SetCallbacks(this);
        }
        EnableInput();
    }

    private void OnDisable() => DisableInput();

    #region Gameplay Actions

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>().normalized);
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

    #endregion

    public InputState GetState()
    {
        throw new System.NotImplementedException();
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