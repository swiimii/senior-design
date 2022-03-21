using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionLogic", menuName = "InteractionSystem/InteractionLogic", order = 0)]
public class InteractionLogic : ScriptableObject {
    [SerializeField] private InputProvider inputProvider;
    [SerializeField] private InteractionData interactionData;
    private Interactable interactable;

    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private Optional<float> useRequiredDistance;

    public void UpdateInteractable(PlayerController player, Vector3 center) {
        CheckForInteractable(player, center);
        CheckForInteractableInput(player);
        // CheckForSpecialInteraction(player);
    }

    private void CheckForInteractable(Component player, Vector3 center) {
        bool hitSomething = Helper.Raycast(center,
            player.transform.right, rayDistance, interactionLayer, out var ray);
        if (hitSomething) {
            interactable = ray.transform.GetComponent<Interactable>();
            if (interactable != null) {
                if (interactionData.IsEmpty()) {
                    interactionData.Interactable = interactable;
                }
                else {
                    if (!interactionData.IsSameInteractable(interactable)) {
                        interactionData.Interactable = interactable;
                    }
                }
            }
        }
        else {
            interactionData.ResetData();
            interactable = null;
        }

        Debug.DrawRay(center, player.transform.right * rayDistance,
            hitSomething ? Color.green : Color.red);
    }

    private void CheckForInteractableInput(PlayerController player) {
        if (interactionData.IsEmpty() || !inputProvider.inputState.isInteracting ||
            !interactionData.Interactable.IsInteractable ||
            interactionData.Interactable.IsSpecialInteraction) return;

        float distanceBetweenInteractable = interactable.transform.position.x - player.transform.position.x;
        Debug.Log(distanceBetweenInteractable);
        if (distanceBetweenInteractable >= interactable.RequiredDistance && useRequiredDistance.Enabled) return;

        if (interactionData.Interactable.HoldInteract) {
            inputProvider.inputState.holdTimer += Time.deltaTime;
            if (!(inputProvider.inputState.holdTimer >= interactionData.Interactable.HoldDuration)) return;
            interactionData.Interact();
            inputProvider.inputState.isInteracting = false;
        }
        else {
            interactionData.Interact();
            // Debug.Log("called Interact()");
            inputProvider.inputState.isInteracting = false;
        }
    }

    // private void CheckForSpecialInteraction(PlayerController player) {
    //     if (interactionData.IsEmpty() || !playerInputData.isUsingSpecial ||
    //         !interactionData.Interactable.IsInteractable) return;
    //     if (!interactionData.Interactable.IsSpecialInteraction) return;
    //     // dridd
    //     if (interactionData.Interactable.HoldInteract) {
    //         playerInputData.holdTimer += Time.deltaTime;
    //         if (!(playerInputData.holdTimer >= interactionData.Interactable.HoldDuration)) return;
    //         interactionData.Interact();
    //         playerInputData.isUsingSpecial = false;
    //     }
    //     else {
    //         interactionData.Interact();
    //         playerInputData.isUsingSpecial = false;
    //     }
    // }
}