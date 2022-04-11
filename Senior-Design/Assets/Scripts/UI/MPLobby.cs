using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class MPLobby : NetworkBehaviour
{
    // Dictionary<int, PlayerData> players;
    public NetworkList<PlayerData> players = new NetworkList<PlayerData>();
    public TMP_Text playerListObject;
    public GameObject playerPrefab;


    private void Start()
    {
        if (!NetworkManager.Singleton)
        {
            // initial scene was not loaded
            return;
        }

        if (IsServer)
        {
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
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnection;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
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

    [ServerRpc(RequireOwnership = false)]
    public void StartServerRpc()
    {
        foreach (PlayerData p in players)
        {
            var obj = Instantiate(playerPrefab);
            obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(p.ClientId);
            obj.GetComponent<NetworkObject>().ChangeOwnership(p.ClientId);
            print("Spawning object for " + p.Name + " of id " + p.ClientId);
        }
        NetworkManager.SceneManager.LoadScene("main", LoadSceneMode.Single);
    }

    [ClientRpc]
    public void UpdatePlayersListClientRpc(string list)
    {
        if(playerListObject)
            playerListObject.text = list;
    }

    public void LeaveLobby()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnection;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        if (players != null && players.Count > 0 && (!NetworkManager.Singleton || NetworkManager.Singleton.IsServer ))
        {
            try
            {
                players.Dispose();
            }
            catch (ObjectDisposedException)
            {
                Debug.LogWarning("Encountered error when disposing of player list.");
            }

        }
        base.OnDestroy();
    }
}
