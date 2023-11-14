using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

public struct PlayerReadyState : INetworkSerializable, IEquatable<PlayerReadyState>
{
    public ulong ClientId;
    public bool IsReady;

    public PlayerReadyState(ulong clientId, bool isReady = false)
    {
        ClientId = clientId;
        IsReady = isReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref IsReady);
    }

    public bool Equals(PlayerReadyState other)
    {
        return ClientId == other.ClientId &&
            IsReady == other.IsReady;
    }
}
