using System;
using System.Collections;
using System.Collections.Generic;
using Aarthificial.Reanimation;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private static class Drivers {
        public const string IsMoving = "isMoving";
        public const string IsMovingHorizontal = "isMovingHorizontal";
        public const string IsMovingRight = "isMovingRight";
        public const string IsMovingUp = "isMovingUp";
    }

    [HideInInspector] public new BoxCollider2D collider;

    [SerializeField] private InputProvider provider;
    // [SerializeField] private Reanimator reanimator;
    [SerializeField] private CollisionDetection collisionDetection;
    [SerializeField] private InteractionLogic interactionLogic;
    private InputState inputState => provider;
    private Vector2 MovementDirection => inputState.movementDirection;

    [SerializeField] private float walkSpeed = 7;

    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        collisionDetection = GetComponent<CollisionDetection>();
    }

    private void Start() {
        if(!GetComponent<NetworkObject>().IsLocalPlayer)
        {
            this.enabled = false;
        }
        else
        {
            provider.EnableInput();
        }
    }

    private void Update() {
        UpdateAnimator();
    }

    private void UpdateAnimator() {
        interactionLogic.UpdateInteractable(this, collider.bounds.center);
        var anim = GetComponent<Animator>();
        anim.SetFloat("HorizontalMovement", MovementDirection.x);
        anim.SetFloat("VerticalMovement", MovementDirection.y);
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
}