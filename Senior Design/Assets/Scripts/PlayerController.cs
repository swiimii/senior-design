using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private CollisionDetection collisionDetection;

    private void Awake()
    {
        collisionDetection = GetComponent<CollisionDetection>();
    }

    private void Update()
    {
        Debug.Log(collisionDetection.wallContact.HasValue);
    }
}
