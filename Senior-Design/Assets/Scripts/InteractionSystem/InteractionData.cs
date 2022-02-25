using UnityEngine;

[CreateAssetMenu(fileName = "InteractionData", menuName = "InteractionSystem/InteractionData", order = 0)]
public class InteractionData : ScriptableObject {
    private Interactable interactable;

    public Interactable Interactable {
        get => interactable;
        set => interactable = value;
    }

    public void Interact() {
        interactable.OnInteract();
        ResetData();
    }

    public bool IsSameInteractable(Interactable newInteractable) => interactable == newInteractable;

    public bool IsEmpty() => interactable == null;

    public void ResetData() => interactable = null;
}