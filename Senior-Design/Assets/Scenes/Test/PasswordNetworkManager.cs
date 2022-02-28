using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PasswordNetworkManager : MonoBehaviour {
    [SerializeField] private TMP_Text playersInGameText;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;


    private void Awake() {
        //playerNameInputField.text = PlayerPrefs.GetString("PlayerName");
    }

    private void Start() {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private void Update() {
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    public void Host() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Client() {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInputField.text);
        NetworkManager.Singleton.StartClient();
    }

    public void Leave() {
        if (NetworkManager.Singleton.IsHost) {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient) {
            NetworkManager.Singleton.Shutdown();
        }

        passwordEntryUI.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void HandleServerStarted() {
        if (NetworkManager.Singleton.IsHost) {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void HandleClientConnected(ulong clientId) {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        passwordEntryUI.SetActive(false);
        leaveButton.SetActive(true);
    }

    private void HandleClientDisconnect(ulong clientId) {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        passwordEntryUI.SetActive(true);
        leaveButton.SetActive(false);
        
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId,
        NetworkManager.ConnectionApprovedDelegate callback) {
        string password = Encoding.ASCII.GetString(connectionData);
        bool approveConnection = password == passwordInputField.text;

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        switch (NetworkManager.Singleton.ConnectedClients.Count) {
            case 1:
                spawnPos = new Vector3(0f, 0f, 0f);
                spawnRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 2:
                spawnPos = new Vector3(5f, 0f, 0f);
                spawnRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
        }

        callback(true, null, approveConnection, spawnPos, spawnRotation);
    }
}