using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FuelTank : NetworkInteractable
{
    public EscapePod escapePod;
    public Sprite completedSprite;

    [ClientRpc]
    protected override void OnInteractClientRpc()
    {
        GetComponent<SpriteRenderer>().sprite = completedSprite;
        escapePod.enabled = true;
    }
}
