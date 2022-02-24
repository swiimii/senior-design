using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionLogic", menuName = "InteractionSystem/InteractionLogic", order = 0)]
public class InteractionLogic : ScriptableObject {
    [SerializeField] private Optional<float> useRequiredDistance;
    [SerializeField] private InteractionInputData interactionInputData;
    [SerializeField] private InteractionData interactionData;

    private InteractableBase interactable;
    
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask interactionLayer;

    public void UpdateInteractable(PlayerController player, Vector3 center) {
        CheckForInteractable(player, center);
        CheckForInteractableInput(player);
        // CheckForSpecialInteraction(player);
    }

    private void CheckForInteractable(Component player, Vector3 center) {
        bool hitSomething = Helper.Raycast(center,
            player.transform.right, rayDistance, interactionLayer, out var ray);
        if (hitSomething) {
            interactable = ray.transform.GetComponent<InteractableBase>();
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
        if (interactionData.IsEmpty() || !interactionInputData.isInteracting ||
            !interactionData.Interactable.IsInteractable ||
            interactionData.Interactable.IsSpecialInteraction) return;

        var distanceBetweenInteractable = interactable.transform.position.x - player.transform.position.x;
        Debug.Log(distanceBetweenInteractable);
        if (distanceBetweenInteractable >= interactable.RequiredDistance && useRequiredDistance.Enabled) return;

        if (interactionData.Interactable.HoldInteract) {
            interactionInputData.holdTimer += Time.deltaTime;
            if (!(interactionInputData.holdTimer >= interactionData.Interactable.HoldDuration)) return;
            interactionData.Interact();
            interactionInputData.isInteracting = false;
        }
        else {
            interactionData.Interact();
            // Debug.Log("called Interact()");
            interactionInputData.isInteracting = false;
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