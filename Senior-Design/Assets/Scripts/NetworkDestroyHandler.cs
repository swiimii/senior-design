using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class NetworkDestroyHandler : MonoBehaviour
{
    public void OnApplicationQuit()
    {
        GetComponent<NetworkManager>().Shutdown();
    }
}
