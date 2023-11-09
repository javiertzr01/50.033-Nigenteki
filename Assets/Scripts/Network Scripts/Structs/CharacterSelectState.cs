using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientId;
    public int CharacterId;
    public int LeftArmId;
    public int RightArmId;

    public CharacterSelectState(ulong clientId, int characterId = -1, int leftArmId = 0, int rightArmId = 0)
    {
        ClientId = clientId;
        CharacterId = characterId;
        LeftArmId = leftArmId;
        RightArmId = rightArmId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref CharacterId);
        serializer.SerializeValue(ref LeftArmId);
        serializer.SerializeValue(ref RightArmId);
    }

    public bool Equals(CharacterSelectState other)
    {
        return ClientId == other.ClientId &&
            CharacterId == other.CharacterId;
    }
}
