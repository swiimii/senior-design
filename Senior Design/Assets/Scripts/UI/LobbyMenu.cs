using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;

public class LobbyMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject startButton;
    public GameObject mpLobbyPrefab;
    public Text playersList;

    void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            startButton.SetActive(false);
        }
        
        if(NetworkManager.Singleton.IsServer)
        {
            var go = Instantiate(mpLobbyPrefab);
            go.GetComponent<NetworkObject>().Spawn();
            DontDestroyOnLoad(go);
        }
        var lm = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<MPLobby>();
        lm.players.OnListChanged += PlayerListChangeCallback;
        playersList.text = GetPlayerListString();
    }

    public void PlayerListChangeCallback(NetworkListEvent<PlayerData> changeEvent)
    {
        UpdatePlayersList(GetPlayerListString());
    }

    public string GetPlayerListString()
    {
        var newText = "";
        print("Setting Player List Text");
        var lm = GameObject.FindGameObjectWithTag("LobbyManager");
        foreach (PlayerData p in lm.GetComponent<MPLobby>().players)
        {
            newText += p.Name + "\n";
        }

        return newText;
    }

    public void UpdatePlayersList(string text)
    {
        playersList.text = text;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var lm = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<MPLobby>();
            lm.players.OnListChanged -= PlayerListChangeCallback;
        }
    }

    public void OnStartButton()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            var lm = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<MPLobby>();
            lm.StartGame();
        }
    }

    public void OnLeaveLobbyButton()
    {
        var lm = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<MPLobby>();
        lm.LeaveLobby();
    }
}
