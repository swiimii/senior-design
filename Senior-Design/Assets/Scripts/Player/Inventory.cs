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
    public FireExtinguisher fireExtinguisher;

    private void Start()
    {
        if (IsServer)
        {
            equippedItem.Value = ItemType.None;
        }

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
        sr.flipX = pos.x < 0 ? true : pos.x > 0 ? false : sr.flipX;

        if (IsLocalPlayer)
        {
            var keyboard = Keyboard.current;
            if (keyboard.fKey.wasPressedThisFrame)
            {
                var useDirection = (itemDisplayLocation.transform.position - transform.position).normalized;
                UseItemServerRpc();
                if (TryInteract(useDirection, out var interactable))
                {
                    interactable.DoInteract(equippedItem.Value);
                }
                if (equippedItem.Value == ItemType.FireExtinguisher)
                {
                    fireExtinguisher.DoFireExtinguisher();
                }
            }
            if (keyboard.fKey.wasReleasedThisFrame)
            {
                StopUsingItemServerRpc();
                fireExtinguisher.StopFireExtinguisher();
            }
        }
    }

    public void Equip(ItemType item)
    {
        InventoryUI.singleton.SetDisplayItem(item);
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

    public bool TryInteract(Vector2 direction, out NetworkInteractable interactable)
    {
        var distance = 2f;
        var hit = Physics2D.Raycast(transform.position, direction, distance);
        if (hit && hit.collider.gameObject && hit.collider.gameObject.TryGetComponent<NetworkInteractable>(out var hitComponent))
        {
            interactable = hitComponent;
            return true;
        }
        else
        {
            interactable = null;
            return false;
        }
    }
}
