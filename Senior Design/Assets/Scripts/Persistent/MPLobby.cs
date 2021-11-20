using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;
using UnityEngine.UI;
using System;

public class MPLobby : NetworkBehaviour
{
    // Dictionary<int, PlayerData> players;
    public NetworkList<PlayerData> players;
    public Text playerListObject;

    private void Start()
    {
        if(IsServer)
        { 
            players = new NetworkList<PlayerData>();
        }
        players.OnListChanged += UpdatePlayersList;
        NetworkManager.OnClientConnectedCallback += HandleClientConnection;
        players.Add(new PlayerData("Sam"));
    }

    private void HandleClientConnection(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            PlayerPrefs.SetString("Name", "Sam");
            SetPlayerNameServerRpc(clientId, PlayerPrefs.GetString("Name"));
        }
    }

    [ServerRpc]
    public void SetPlayerNameServerRpc(ulong clientId, string name)
    {
        PlayerPrefs.SetString("Name", "Sam");
        print("Setting Player Name");
        players[0] = new PlayerData(name);
    }

    public void UpdatePlayersList(NetworkListEvent<PlayerData> changeEvent)
    {
        playerListObject.text = "";
        print("Setting Text");
        foreach(PlayerData p in players)
        {
            playerListObject.text += p.Name + "\n";
        }
    }
}
