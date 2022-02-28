using System;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : NetworkSingleton<PlayersManager> {
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public int PlayersInGame => playersInGame.Value;

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += id => {
            playersInGame.Value++;
            Logger.Instance.LogInfo($"{id} just connected...");
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += id => {
            playersInGame.Value--;
            Logger.Instance.LogInfo($"{id} just connected...");
        };
    }
}