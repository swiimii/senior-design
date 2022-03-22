using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum ItemType
{
    None = -1,
    FireExtinguisher = 0,
    FuelTank = 1,
    BlueID = 2,
    YellowID = 3,
}

public class InventoryItem : MonoBehaviour
{
    public ItemType type = ItemType.FireExtinguisher;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player") && other.gameObject.TryGetComponent<NetworkObject>(out var netObj) && netObj.IsLocalPlayer)
        {
            other.gameObject.GetComponent<Inventory>().Equip(type);
        }
    }
}
