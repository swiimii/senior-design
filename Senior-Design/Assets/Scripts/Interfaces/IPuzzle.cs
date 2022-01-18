using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

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
