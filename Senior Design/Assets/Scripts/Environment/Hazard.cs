using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Hazard : NetworkBehaviour
{
    public string description = "Unnamed Hazard";
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collision");
        if (collision.gameObject.GetComponent<NetworkObject>() && collision.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            print("Check passed");
            if (collision.gameObject.GetComponent<Health>())
            {
                var knockbackDirection = collision.gameObject.transform.position - transform.position;
                collision.gameObject.GetComponent<Health>().DamageServerRpc(damage, description, knockbackDirection);
            }
        }
    }

}
