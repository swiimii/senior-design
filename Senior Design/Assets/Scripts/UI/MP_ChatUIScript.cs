using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using MLAPI;
using System;
using MLAPI.NetworkVariable.Collections;

public class MP_ChatUIScript : NetworkBehaviour
{
    public GameObject panel;
    public Text chatText = null;
    public InputField chatInput = null;

    static NetworkVariableString messages = new NetworkVariableString("Temp");

    public NetworkList<PlayerData> chatPlayers;
    private string playerName = "N/A";

    void Start()
    {
        messages.OnValueChanged += updateUIClientRpc;
        if (!IsOwner)
        {
            panel.SetActive(false);
            return;
        }
        foreach(PlayerData player in chatPlayers)
        {
            if(NetworkManager.LocalClientId == player.ClientId)
            {
                playerName = player.Name;
                break;
            }
        }
    }

    [ClientRpc]
    private void updateUIClientRpc(string previousValue, string newValue)
    {
        chatText.text += "\n" + newValue.Substring(previousValue.Length, newValue.Length - previousValue.Length);
    }

    public void handleSend()
    {
        if (!IsServer)
        {
            sendMessageServerRpc(chatInput.text);
        }
        else
        {
            messages.Value += "\n" + playerName + ": " + chatInput.text;
        }
        chatInput.text = "";
    }

    [ServerRpc]
    private void sendMessageServerRpc(string text, ServerRpcParams svrParam = default)
    {
        var incomingPlayerName = "N/A";
        foreach (PlayerData player in chatPlayers)
        {
            if (svrParam.Receive.SenderClientId == player.ClientId)
            {
                incomingPlayerName = player.Name;
                break;
            }
        }
        messages.Value += "\n" + incomingPlayerName  + ": " + text;
    }
}
