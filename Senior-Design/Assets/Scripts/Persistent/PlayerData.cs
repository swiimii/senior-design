using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public struct PlayerData : INetworkSerializable, System.IEquatable<PlayerData> {
    public FixedString32Bytes Name;
    public ulong ClientId;

    public PlayerData(FixedString32Bytes name, ulong clientId) {
        Name = name;
        ClientId = clientId;
    }

    public bool Equals(PlayerData other) {
        return this.Name == other.Name && this.ClientId == other.ClientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref ClientId);
    }

    public static implicit operator string(PlayerData p) => p.ToString();
    public static implicit operator PlayerData(string s) => new PlayerData {
        Name = new FixedString32Bytes(s)
    };
}