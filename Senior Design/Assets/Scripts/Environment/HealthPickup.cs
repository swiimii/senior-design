using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class HealthPickup : NetworkBehaviour
{
    public int healValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collision");
        if (collision.gameObject.GetComponent<Health>() && collision.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            collision.gameObject.GetComponent<Health>().HealServerRpc(healValue);
            ConsumeServerRpc();
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void ConsumeServerRpc()
    {
        if(gameObject)
            Destroy(gameObject);
    }
}
