using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Player : NetworkBehaviour {
    [SerializeField] private InputProvider inputProvider;
    private InputState inputState => inputProvider;

    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private CollisionDetection collisionDetection;

    public List<Sprite> nSprites;
    public List<Sprite> neSprites;
    public List<Sprite> eSprites;
    public List<Sprite> seSprites;
    public List<Sprite> sSprites;
    
    
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float frameRate = 8f;
    private float idleTime;

    private void Awake() {
        
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collisionDetection = GetComponent<CollisionDetection>();
    }

    private void Start() {
        switch (IsLocalPlayer) {
            case false:
                name = "ClonePlayer";
                enabled = false;
                break;
            case true:
                name = "MainPlayer";
                break;
        }
    }
    
    private void FixedUpdate() {
        if (!IsOwner) return;

        HandleMovement();
        HandleAnimation();
        HandleFlip();
        collisionDetection.ActivateTopDownCollision(inputState.movementDirection);
    }
    
    private void HandleAnimation() {
        if (!IsLocalPlayer) return;
        List<Sprite> directionSprites = GetSpriteDirection();
        if (directionSprites != null) {
            float playTime = Time.time - idleTime;
            var totalFrames = (int)(playTime * frameRate);
            int frame = totalFrames % directionSprites.Count;
            
            spriteRenderer.sprite = directionSprites[frame];
        }
        else {
            idleTime = Time.time;
        }
    }
    private void HandleMovement() {
        rigidbody2D.velocity = inputState.movementDirection * (walkSpeed * Time.fixedDeltaTime);
    }
    
    private List<Sprite> GetSpriteDirection() {
        List<Sprite> selectedSprites = null;
        
        if (inputState.movementDirection.y > 0) {
            selectedSprites = Mathf.Abs(inputState.movementDirection.x) > 0 ? neSprites : nSprites;
        }
        else if (inputState.movementDirection.y < 0) {
            selectedSprites = Mathf.Abs(inputState.movementDirection.x) > 0 ? seSprites : sSprites;
        }
        else {
            if (Mathf.Abs(inputState.movementDirection.x) > 0) {
                selectedSprites = eSprites;
            }
        }
        return selectedSprites;
    }
    private void HandleFlip() {
        if (!IsLocalPlayer) return;
        
        bool flipX = spriteRenderer.flipX;
        flipX = flipX switch {
            false when inputState.movementDirection.x < 0 => true,
            true when inputState.movementDirection.x > 0 => false,
            _ => flipX
        };
        spriteRenderer.flipX = flipX;
    }
}