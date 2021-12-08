using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;
using MLAPI.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MPLobby : NetworkBehaviour
{
    // Dictionary<int, PlayerData> players;
    public NetworkList<PlayerData> players;
    public Text playerListObject;
    public GameObject playerPrefab;


    private void Start()
    {
        if (IsServer)
        {
            players = new NetworkList<PlayerData>();
            players.OnListChanged += UpdatePlayersList;
        }

        if (IsHost)
        {
            players.Add(new PlayerData(PlayerPrefs.GetString("Name"), NetworkManager.Singleton.LocalClientId));
        }
        else
        {
            SetPlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetString("Name"));
        }
        NetworkManager.OnClientConnectedCallback += HandleClientConnection;
        NetworkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientConnection(ulong clientId)
    {
        print("Player Connected");
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (IsServer)
        {
            foreach (PlayerData p in players)
            {
                if (p.ClientId == clientId)
                {
                    players.Remove(p);
                    break;
                }
            }
        }
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            print("Disconnected from Server");
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            print("Player Disconnected");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(ulong clientId, string name)
    {
        print("Setting Player Name");
        players.Add(new PlayerData(name, clientId));
    }

    public void UpdatePlayersList(NetworkListEvent<PlayerData> changeEvent)
    {
        var newText = "";
        print("Setting Text");
        foreach (PlayerData p in players)
        {
            newText += p.Name + "\n";
        }
        UpdatePlayersListClientRpc(newText);
    }

    public void StartGame()
    {
        if (IsHost)
        {
            StartServerRpc();
        }
    }

    [ServerRpc]
    public void StartServerRpc()
    {
        foreach (PlayerData p in players)
        {
            var obj = Instantiate(playerPrefab);
            obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(p.ClientId);
            print("Spawning object for " + p.Name + " of id " + p.ClientId);
        }
        NetworkSceneManager.SwitchScene("main");
    }

    [ClientRpc]
    public void UpdatePlayersListClientRpc(string list)
    {
        if(playerListObject)
            playerListObject.text = list;
    }

    public void LeaveLobby()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
        else
        {
            NetworkManager.Singleton.StopClient();
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        NetworkManager.OnClientConnectedCallback -= HandleClientConnection;
        NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
}
