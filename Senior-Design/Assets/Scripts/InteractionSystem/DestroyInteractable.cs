﻿using UnityEngine;

public class DestroyInteractable : InteractableBase {
    public override void OnInteract() {
        base.OnInteract();
        
        if(IsInteractable)
            Destroy(gameObject);
    }
}