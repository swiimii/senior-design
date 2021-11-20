using System;
using System.Collections;
using System.Collections.Generic;
using Aarthificial.Reanimation;
using UnityEngine;
using Extensions;

public class PlayerController : MonoBehaviour {
    private static class Drivers {
        public const string IsMoving = "isMoving";
        public const string IsMovingHorizontal = "isMovingHorizontal";
        public const string IsMovingRight = "isMovingRight";
        public const string IsMovingUp = "isMovingUp";
    }

    [SerializeField] private InputProvider provider;
    [SerializeField] private float walkSpeed = 7;
    private Reanimator reanimator;
    private CollisionDetection collisionDetection;

    private InputState inputState => provider;
    private Vector2 MovementDirection => inputState.movementDirection;
    
    private void Awake() {
        provider.EnableInput();

        reanimator = GetComponent<Reanimator>();
        collisionDetection = GetComponent<CollisionDetection>();
    }
    private void OnEnable() {
        provider.onInteract += OnInteract;
    }

    private void OnDisable() {
        provider.onInteract -= OnInteract;
    }
    private void Update() {
        Debug.Log(collisionDetection.wallContact.HasValue);
        
        reanimator.Set(Drivers.IsMoving, MovementDirection != Vector2.zero);
        reanimator.Set(Drivers.IsMovingHorizontal, MovementDirection.x != 0);
        reanimator.Set(Drivers.IsMovingRight, MovementDirection.x > 0);
        reanimator.Set(Drivers.IsMovingUp, MovementDirection.y > 0);
    }
    private void FixedUpdate() {
        UpdateMovementState();
    }
    private void UpdateMovementState() {
        var previousVelocity = collisionDetection.rigidbody2D.velocity;
        var velocityChange = Vector2.zero;

        velocityChange.x = (MovementDirection.x * walkSpeed - previousVelocity.x) / 4;
        velocityChange.y = (MovementDirection.y * walkSpeed - previousVelocity.y) / 4;
        
        if (collisionDetection.wallContact.HasValue) {
            var wallDirection = (int) Mathf.Sign(collisionDetection.wallContact.Value.point.x - transform.position.x);
            var walkDirection = (int) Mathf.Sign(MovementDirection.x);

            if (walkDirection == wallDirection)
                velocityChange.x = 0;
        }

        collisionDetection.rigidbody2D.AddForce(velocityChange, ForceMode2D.Impulse);
    }
    private void OnInteract(float arg0) {
        // Do something here
    }
}