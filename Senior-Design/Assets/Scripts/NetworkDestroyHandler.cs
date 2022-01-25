using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkDestroyHandler : MonoBehaviour
{
    public void OnApplicationQuit()
    {
        GetComponent<NetworkManager>().Shutdown();
    }
}
