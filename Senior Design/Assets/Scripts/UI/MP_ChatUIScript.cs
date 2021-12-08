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

    private string localPlayerName => PlayerPrefs.GetString("Name");

    void Start()
    {
        messages.OnValueChanged += updateUIClientRpc;
        if (!IsOwner)
        {
            panel.SetActive(false);
            return;
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
            sendMessageServerRpc(chatInput.text, localPlayerName);
        }
        else
        {
            messages.Value += "\n" + localPlayerName + ": " + chatInput.text;
        }
        chatInput.text = "";
    }

    [ServerRpc]
    private void sendMessageServerRpc(string text, string name, ServerRpcParams svrParam = default)
    {
        messages.Value += "\n" + name  + ": " + text;
    }
}
