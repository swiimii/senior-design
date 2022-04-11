using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HideOnClient : MonoBehaviour
{
    void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            gameObject.SetActive(false);
        }
    }

}
