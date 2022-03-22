using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Inventory : NetworkBehaviour
{
    public NetworkVariable<ItemType> equippedItem;
    public GameObject itemDisplayLocation;
    public float itemDisplayOffset = 1f;
    public Dictionary<ItemType, Sprite> itemToSprite = new Dictionary<ItemType, Sprite>();
    public Sprite[] possibleSprites;

    private void Start()
    {
        if (IsServer)
            equippedItem.Value = ItemType.None;

        for (int i = 0; i < possibleSprites.Length; i++)
        {
            itemToSprite.Add((ItemType)i, possibleSprites[i]);
        }

    }
    private void Update()
    {
        var anim = GetComponent<Animator>();
        Vector2 direction = new Vector2(anim.GetFloat("HorizontalMovement"), anim.GetFloat("VerticalMovement"));
        if (direction.magnitude > .1f)
        {
            itemDisplayLocation.transform.localPosition = new Vector3(direction.x, direction.y, itemDisplayLocation.transform.localPosition.z);
        }

        var sr = itemDisplayLocation.GetComponent<SpriteRenderer>();
        var pos = itemDisplayLocation.transform.localPosition;
        sr.flipX = pos.x < 0;

        if (IsLocalPlayer)
        {
            var keyboard = Keyboard.current;
            if (keyboard.fKey.wasPressedThisFrame)
            {
                UseItemServerRpc();
            }
            if (keyboard.fKey.wasReleasedThisFrame)
            {
                StopUsingItemServerRpc();
            }
        }
    }

    public void Equip(ItemType item)
    {
        EquipServerRpc(item);
    }

    [ServerRpc]
    private void EquipServerRpc(ItemType item)
    {
        equippedItem.Value = item;
    }

    [ServerRpc]
    public void UseItemServerRpc()
    {
        UseItemClientRpc();
    }

    [ClientRpc]
    private void UseItemClientRpc()
    {
        if (itemToSprite.TryGetValue(equippedItem.Value, out var itemSprite))
        {
            var sr = itemDisplayLocation.GetComponent<SpriteRenderer>();
            sr.enabled = true;
            sr.sprite = itemSprite;
        }
        else
        {
            print("Item Use Error");
        }
    }

    [ServerRpc]
    public void StopUsingItemServerRpc()
    {
        StopUsingItemClientRpc();
    }

    [ClientRpc]
    public void StopUsingItemClientRpc()
    {
        var sr = itemDisplayLocation.GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }
}
