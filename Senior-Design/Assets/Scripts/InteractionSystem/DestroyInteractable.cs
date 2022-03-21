using UnityEngine;

public class DestroyInteractable : Interactable {
    public override void OnInteract() {
        base.OnInteract();
        
        if(IsInteractable)
            Destroy(gameObject);
    }
}