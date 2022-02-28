using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Player : NetworkBehaviour {
    [SerializeField] private InputProvider inputProvider;
    private InputState inputState => inputProvider;

    private Rigidbody2D rb;
    private CollisionDetection cd;
    private Animator anim;

    [SerializeField] private float walkSpeed = 7f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CollisionDetection>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleAnimation();
        cd.ActivateTopDownCollision(inputState.movementDirection);
    }

    private void HandleMovement() {
        rb.velocity = inputState.movementDirection * walkSpeed;
    }

    private void HandleAnimation() {
        if (inputState.movementDirection.y > 0) {
            if (inputState.movementDirection.x > 0) {
                PlayAnimationClip("PlayerNorthEast");
            }
            else if (inputState.movementDirection.x < 0) {
                PlayAnimationClip("PlayerNorthWest");
            }
            else {
                PlayAnimationClip("PlayerNorth");
            }
        }
        else if (inputState.movementDirection.y < 0) {
            if (inputState.movementDirection.x > 0) {
                PlayAnimationClip("PlayerSouthEast");
            }
            else if (inputState.movementDirection.x < 0) {
                PlayAnimationClip("PlayerSouthWest");
            }
            else {
                PlayAnimationClip("PlayerSouth");
            }
        }
        else {
            if (inputState.movementDirection.x > 0) {
                PlayAnimationClip("PlayerEast");
            }
            else if (inputState.movementDirection.x < 0) {
                PlayAnimationClip("PlayerWest");
            }
            else {
                anim.speed = 0;
            }
        }
    }

    private void PlayAnimationClip(string clipName) {
        anim.speed = 1;
        anim.Play(clipName);
    }
}