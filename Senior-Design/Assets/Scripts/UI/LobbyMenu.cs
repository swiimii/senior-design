using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyMenu : MonoBehaviour
{
    public GameObject startButton;
    // public GameObject mpLobbyPrefab;
    // public Text playersList;

    void Start()
    {
        if (!NetworkManager.Singleton)
        {
            // Will be reloading; Main menu was not loaded
            return;
        }
        if ( !NetworkManager.Singleton.IsHost)
        {
            startButton.SetActive(false);
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
