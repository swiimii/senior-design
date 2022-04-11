using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    [HideInInspector] public new BoxCollider2D collider;
    [SerializeField] private InputProvider provider;
    [SerializeField] private CollisionDetection collisionDetection;
    [SerializeField] private InteractionLogic interactionLogic;
    
    private InputState inputState => provider;
    private Vector2 MovementDirection => inputState.movementDirection;
    public Vector2 GetMovementDirection() => MovementDirection;
    
    private static readonly int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
    private static readonly int VerticalMovement = Animator.StringToHash("VerticalMovement");

    [SerializeField] private float walkSpeed = 7;
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        collisionDetection = GetComponent<CollisionDetection>();
        collisionDetection.ActivateTopDownCollision(MovementDirection);
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
        anim.SetFloat(HorizontalMovement, MovementDirection.x);
        anim.SetFloat(VerticalMovement, MovementDirection.y);
    }

    private void FixedUpdate() {
        UpdateMovementState();
    }

    private void UpdateMovementState() {
        var previousVelocity = collisionDetection.rigidbody2D.velocity;
        var velocityChange = Vector2.zero;

        velocityChange.x = (MovementDirection.x * walkSpeed - previousVelocity.x) / 4;
        velocityChange.y = (MovementDirection.y * walkSpeed - previousVelocity.y) / 4;

        collisionDetection.rigidbody2D.AddForce(velocityChange, ForceMode2D.Impulse);
    }

}