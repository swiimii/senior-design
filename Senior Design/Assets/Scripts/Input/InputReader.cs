using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "InputData/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions {
    
    // Gameplay
    public event UnityAction<Vector2> MoveEvent;
    public event UnityAction<Vector2> MousePosEvent;
    public event UnityAction InteractionStartedEvent;
    public event UnityAction InteractionCancelledEvent;


    private GameInput GameInput { get; set; }

    private void OnEnable() {
        if (GameInput == null) {
            GameInput = new GameInput();
            GameInput.Gameplay.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    private void OnDisable() => DisableAllInput();

    #region Gameplay Actions
    
    public void OnMove(InputAction.CallbackContext context) {
        MoveEvent?.Invoke(context.ReadValue<Vector2>().normalized);
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (InteractionStartedEvent != null && context.phase == InputActionPhase.Started)
            InteractionStartedEvent.Invoke();

        if (InteractionCancelledEvent != null && context.phase == InputActionPhase.Canceled)
            InteractionCancelledEvent.Invoke();
    }
    public void OnMouse(InputAction.CallbackContext context) {
        MousePosEvent?.Invoke(context.ReadValue<Vector2>());
    }

    #endregion

    public void EnableGameplayInput() {
        GameInput.Gameplay.Enable();
    }
    
    public void DisableAllInput() {
        GameInput.Gameplay.Disable();
    }
}