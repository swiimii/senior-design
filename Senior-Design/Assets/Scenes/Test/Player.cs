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
    private bool ClientAndOwner => IsClient && IsOwner;

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

    private void Update() {
        SetOverlay();
        
        if (ClientAndOwner) {
            UpdateClientMovementAndAnimation();
            cd.ActivateTopDownCollision(inputState.movementDirection);
        }
        
        UpdateClientAnimationSpeed();
    }

    private void UpdateClientAnimationSpeed() {
        anim.speed = networkPlayerState.Value == PlayerState.Idle ? 0 : 1;
    }

    private void UpdateClientMovementAndAnimation() {
        rb.velocity = inputState.movementDirection * walkSpeed;

        if (inputState.movementDirection.y > 0) {
            if (inputState.movementDirection.x > 0) {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerNorthEast);
                PlayAnimationClipServerRpc(AnimationStates.PlayerNorthEast);
            }
            else if (inputState.movementDirection.x < 0) {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerNorthWest);
                PlayAnimationClipServerRpc(AnimationStates.PlayerNorthWest);
            }
            else {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerNorth);
                PlayAnimationClipServerRpc(AnimationStates.PlayerNorth);
            }
        }
        else if (inputState.movementDirection.y < 0) {
            if (inputState.movementDirection.x > 0) {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerSouthEast);
                PlayAnimationClipServerRpc(AnimationStates.PlayerSouthEast);
            }
            else if (inputState.movementDirection.x < 0) {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerSouthWest);
                PlayAnimationClipServerRpc(AnimationStates.PlayerSouthWest);
            }
            else {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerSouth);
                PlayAnimationClipServerRpc(AnimationStates.PlayerSouth);
            }
        }
        else {
            if (inputState.movementDirection.x > 0) {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerEast);
                PlayAnimationClipServerRpc(AnimationStates.PlayerEast);
            }
            else if (inputState.movementDirection.x < 0) {
                UpdateAnimationClipNameServerRpc(AnimationStates.PlayerWest);
                PlayAnimationClipServerRpc(AnimationStates.PlayerWest);
            }
            else {
                PauseAnimationsServerRpc();
            }
        }
    }

    private void SetOverlay() {
        if (overlaySet || string.IsNullOrEmpty(playerNetworkName.Value)) return;
        var localPlayerOverlay = gameObject.GetComponentInChildren<TMP_Text>();
        localPlayerOverlay.text = $"{playerNetworkName.Value}";
        overlaySet = true;
    }

    [ServerRpc]
    private void UpdateAnimationClipNameServerRpc(string clipName) => animationClipName.Value = clipName;

    [ServerRpc]
    private void PauseAnimationsServerRpc() => UpdatePlayerStateServerRpc(PlayerState.Idle);

    [ServerRpc]
    private void PlayAnimationClipServerRpc(string clipName) {
        anim.Play(clipName);
        UpdatePlayerStateServerRpc(PlayerState.Walk);
    }
    [ServerRpc]
    private void UpdatePlayerStateServerRpc(PlayerState state) => networkPlayerState.Value = state;
}