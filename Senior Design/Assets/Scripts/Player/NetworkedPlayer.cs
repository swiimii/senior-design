using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    PlayerController pc => GetComponent<PlayerController>();
    void Start()
    {
        if (!IsLocalPlayer)
        {
            pc.enabled = false;
        }
    }
}
