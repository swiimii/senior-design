using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Hazard : MonoBehaviour
{
    public string description = "Unnamed Hazard";
    public int damage = 1;

    private void OnCollisionEnter(Collision collision)
    {
        NetworkObject netObj = collision.gameObject.GetComponent<NetworkObject>();
        if (netObj.IsLocalPlayer)
        {
            if (collision.gameObject.GetComponent<NetworkBehaviour>() && collision.gameObject.GetComponent<NetworkBehaviour>().IsOwner)
            {
                if (collision.gameObject.GetComponent<Health>())
                {
                    collision.gameObject.GetComponent<Health>().DamageServerRpc(damage, description);
                }
            }
        }
    }
}
