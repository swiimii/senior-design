using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum PlayerState {
    Idle,
    Walk,
}

public class Player : NetworkBehaviour {
    private static class AnimationStates {
        public const string PlayerNorthEast = "PlayerNorthEast";
        public const string PlayerNorthWest = "PlayerNorthWest";
        public const string PlayerNorth = "PlayerNorth";
        public const string PlayerSouthEast = "PlayerSouthEast";
        public const string PlayerSouthWest = "PlayerSouthWest";
        public const string PlayerSouth = "PlayerSouth";
        public const string PlayerEast = "PlayerEast";
        public const string PlayerWest = "PlayerWest";
    }
    private static InputState inputState => InputManager.Instance.InputProvider;

    private Rigidbody2D rb;
    private CollisionDetection cd;
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float walkSpeed = 7f;

    // Network Variables //
    [SerializeField] private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
    [SerializeField] private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();
    [SerializeField] private NetworkVariable<NetworkString> animationClipName = new NetworkVariable<NetworkString>();
    private bool overlaySet = false;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CollisionDetection>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            playerNetworkName.Value = $"Player {OwnerClientId}";
        }
    }

    private void Start() {
        if (IsClient && IsOwner) {
            transform.position = new Vector3(0, 0, 0);
        }
    }

    private void Update() {
        SetOverlay();
        
        if (IsClient && IsOwner) {
            ClientMovementAndAnimation();
            cd.ActivateTopDownCollision(inputState.movementDirection);
        }
        
        ClientVisuals();
    }

    private void ClientVisuals() {
        anim.speed = networkPlayerState.Value == PlayerState.Idle ? 0 : 1;
    }

    private void ClientMovementAndAnimation() {
        rb.velocity = inputState.movementDirection * walkSpeed;

        if (inputState.movementDirection.y > 0) {
            if (inputState.movementDirection.x > 0) {
                PlayAnimationClipServerRpc(AnimationStates.PlayerNorthEast);
            }
            else if (inputState.movementDirection.x < 0) {
                PlayAnimationClipServerRpc(AnimationStates.PlayerNorthWest);
            }
            else {
                PlayAnimationClipServerRpc(AnimationStates.PlayerNorth);
            }
        }
        else if (inputState.movementDirection.y < 0) {
            if (inputState.movementDirection.x > 0) {
                PlayAnimationClipServerRpc(AnimationStates.PlayerSouthEast);
            }
            else if (inputState.movementDirection.x < 0) {
                PlayAnimationClipServerRpc(AnimationStates.PlayerSouthWest);
            }
            else {
                PlayAnimationClipServerRpc(AnimationStates.PlayerSouth);
            }
        }
        else {
            if (inputState.movementDirection.x > 0) {
                PlayAnimationClipServerRpc(AnimationStates.PlayerEast);
            }
            else if (inputState.movementDirection.x < 0) {
                PlayAnimationClipServerRpc(AnimationStates.PlayerWest);
            }
            else {
                PauseAnimationsServerRpc();
            }
        }
    }

    public void SetOverlay() {
        if (overlaySet || string.IsNullOrEmpty(playerNetworkName.Value)) return;
        var localPlayerOverlay = gameObject.GetComponentInChildren<TMP_Text>();
        localPlayerOverlay.text = $"{playerNetworkName.Value}";
        overlaySet = true;
    }

    [ServerRpc]
    private void PauseAnimationsServerRpc() {
        UpdatePlayerStateServerRpc(PlayerState.Idle);
    }

    [ServerRpc]
    private void PlayAnimationClipServerRpc(string clipName) {
        anim.Play(clipName);
        UpdatePlayerStateServerRpc(PlayerState.Walk);
    }

    [ServerRpc]
    private void UpdatePlayerStateServerRpc(PlayerState state) {
        networkPlayerState.Value = state;
    }
}