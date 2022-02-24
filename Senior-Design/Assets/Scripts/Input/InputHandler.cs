using UnityEngine;

public class InputHandler : MonoBehaviour {
    
    [SerializeField] private InputProvider inputReader = default;
    [SerializeField] private InteractionInputData interactionInputData = default;

    private void OnEnable() {
        inputReader.EnableInput();
        interactionInputData.RegisterEvents();
    }
    private void OnDisable() {
        interactionInputData.UnregisterEvents();
        inputReader.DisableInput();
    }

    private void Start() {
        interactionInputData.Reset();
    }
    
}