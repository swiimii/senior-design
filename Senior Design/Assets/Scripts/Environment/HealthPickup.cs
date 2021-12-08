using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class HealthPickup : MonoBehaviour
{
    public int healValue = 1;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<NetworkBehaviour>() && collision.gameObject.GetComponent<NetworkBehaviour>().IsOwner)
        {
            if (collision.gameObject.GetComponent<Health>())
            {
                collision.gameObject.GetComponent<Health>().HealServerRpc(healValue);
                Destroy(gameObject);
            }
        }
    }
}
