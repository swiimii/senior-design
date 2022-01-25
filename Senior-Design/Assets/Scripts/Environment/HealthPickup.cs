using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthPickup : NetworkBehaviour
{
    public int healValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collision");
        if (collision.gameObject.TryGetComponent<Health>(out var healthComponent) && collision.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            healthComponent.HealServerRpc(healValue);
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
