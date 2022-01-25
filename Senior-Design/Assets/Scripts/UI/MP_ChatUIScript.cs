using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using System;

public class MP_ChatUIScript : NetworkBehaviour
{
    public GameObject panel;
    public Text chatText = null;
    public InputField chatInput = null;

    static NetworkVariable<FixedString32Bytes> messages = new NetworkVariable<FixedString32Bytes>("Temp");

    private string localPlayerName => PlayerPrefs.GetString("Name");

    void Start()
    {
        if (IsServer)
            messages.OnValueChanged += updateUIClientRpc;
    }

    [ClientRpc]
    private void updateUIClientRpc(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        chatText.text += "\n" + newValue.ToString().Substring(previousValue.Length, newValue.Length - previousValue.Length);
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

    [ServerRpc(RequireOwnership=false)]
    private void sendMessageServerRpc(string text, string name, ServerRpcParams svrParam = default)
    {
        messages.Value += "\n" + name  + ": " + text;
    }
}
