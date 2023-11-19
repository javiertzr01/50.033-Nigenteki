using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public struct ArmSelectState : INetworkSerializable, IEquatable<ArmSelectState>
{
    public ulong ClientId;
    public int ArmId;

    public ArmSelectState(ulong clientId, int armId = -1)
    {
        ClientId = clientId;
        ArmId = armId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref ArmId);
    }
    public bool Equals(ArmSelectState other)
    {
            return ClientId == other.ClientId &&
                ArmId == other.ArmId;
    }
}
