using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Serialization;

public struct PlayerData : INetworkSerializable
{
    public string Name;
    public ulong ClientId;
    public PlayerData(string name, ulong clientId)
    {
        Name = name;
        ClientId = clientId;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref Name);
        serializer.Serialize(ref ClientId);
    }
}
