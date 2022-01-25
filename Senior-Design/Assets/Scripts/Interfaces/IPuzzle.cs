using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class IPuzzle : NetworkBehaviour
{
    public bool isFinished;
    public abstract bool CheckState();
    
    public GameObject puzzleInterface;

    public virtual void OnInteract()
    {
        puzzleInterface.SetActive(true);
    }
}
