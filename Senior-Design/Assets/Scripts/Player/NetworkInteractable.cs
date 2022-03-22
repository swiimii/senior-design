using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkInteractable : NetworkBehaviour
{
    [SerializeField] ItemType targetItem;


    /// <summary>
    /// /// Function to be called locally when player interaction 
    /// raycast hits an an interactable object
    /// </summary>
    /// <param name="currentHeldItem"></param>
    public void DoInteract(ItemType currentHeldItem)
    {
        OnInteractServerRpc(currentHeldItem);
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void OnInteractServerRpc(ItemType currentHeldItem)
    {
        if (currentHeldItem == targetItem)
        {
            OnInteractClientRpc();
        }
        else
        {
            print("Incorrect Item");
        }
    }

    [ClientRpc]
    protected virtual void OnInteractClientRpc()
    {
        gameObject.SetActive(false);
    }
}
