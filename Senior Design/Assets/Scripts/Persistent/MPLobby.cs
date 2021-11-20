using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct PlayerData : INetworkSerializable
{
    public string name;

    public PlayerData(string name) => this.name = name;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
    }
}

public class MPLobby : NetworkManager
{
    // Dictionary<int, PlayerData> players;
    public NetworkList<PlayerData> players;

    private void Start()
    {
    }

    public void SetPlayerName(string name)
    {
        players[0] = new PlayerData(name);
    }
}
