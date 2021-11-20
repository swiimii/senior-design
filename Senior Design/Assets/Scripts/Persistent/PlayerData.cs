using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Serialization;

public struct PlayerData : INetworkSerializable
{
    public string Name;

    public PlayerData(string name)
    {
        Name = name;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref Name);
    }
}
