using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Hazard : NetworkBehaviour
{
    public string description = "Unnamed Hazard";
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collision");
        if (collision.gameObject && collision.gameObject.TryGetComponent<NetworkObject>(out var nObj) && nObj.IsLocalPlayer)
        {
            print("Check passed");
            if (collision.gameObject.TryGetComponent<Health>(out var healthComponent))
            {
                var knockbackDirection = collision.gameObject.transform.position - transform.position;
                healthComponent.DamageServerRpc(damage, description, knockbackDirection);
            }
        }
    }

}
